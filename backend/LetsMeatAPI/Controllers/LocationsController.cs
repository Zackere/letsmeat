using LetsMeatAPI.ExternalAPI;
using LetsMeatAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LetsMeatAPI.Controllers {
  [Route("[controller]")]
  [ApiController]
  public class LocationsController : ControllerBase {
    public LocationsController(
      IUserManager userManager,
      LMDbContext context,
      IGooglePlaces googlePlaces,
      IPaidResourceGuard paidResourceGuard,
      ILocationCritic critic,
      ILogger<LocationsController> logger
    ) {
      _userManager = userManager;
      _context = context;
      _googlePlaces = googlePlaces;
      _paidResourceGuard = paidResourceGuard;
      _critic = critic;
      _logger = logger;
    }
    public class Rating {
      public ulong taste { get; set; }
      public ulong taste_votes { get; set; }
      public ulong price { get; set; }
      public ulong price_votes { get; set; }
      public ulong amount_of_food { get; set; }
      public ulong amount_of_food_votes { get; set; }
      public ulong waiting_time { get; set; }
      public ulong waiting_time_votes { get; set; }
      public double overall_score { get; set; }
      public double personalized_score { get; set; }
      public Rating(LocationBase location) {
        _location = location;
        amount_of_food = location.AmountOfFood;
        amount_of_food_votes = location.AmountOfFoodVotes;
        price = location.Price;
        price_votes = location.PriceVotes;
        taste = location.Taste;
        taste_votes = location.TasteVotes;
        waiting_time = location.WaitingTime;
        waiting_time_votes = location.WaitingTimeVotes;
      }
      public Rating FillPersonalizedInfo(User user, ILocationCritic critic) {
        overall_score = critic.AbsoluteScore(_location);
        personalized_score = critic.PersonalScore(_location, user);
        return this;
      }
      private readonly LocationBase _location;
    }
    public class LocationCreateCustomBody {
      public Guid group_id { get; set; }
      [MaxLength(128)]
      public string Address { get; set; }
      [MaxLength(64)]
      public string Name { get; set; }
    }
    [HttpPost]
    [Route("create_custom")]
    public async Task<ActionResult<LocationInformationResponse.CustomLocationInformation>> CreateCustom(
      string token,
      [FromBody] LocationCreateCustomBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var grp = await _context.Groups.FindAsync(body.group_id);
      if(grp == null)
        return NotFound();
      if(!grp.Users.Any(u => u.Id == userId))
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      var location = new CustomLocation() {
        Address = body.Address,
        CreatedForId = body.group_id,
        EventsWithMe = new List<Event>(),
        Name = body.Name,
      };
      await _context.CustomLocations.AddAsync(location);
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return new LocationInformationResponse.CustomLocationInformation {
        address = location.Address,
        created_for_id = location.CreatedForId,
        id = location.Id,
        name = location.Name,
        rating = new Rating(location)
                 .FillPersonalizedInfo(await _context.Users.FindAsync(userId), _critic),
      };
    }
    public class LocationCreateFromGMapsBody {
      public string place_id { get; set; }
      [MinLength(32)]
      public string sessiontoken { get; set; }
    }
    [HttpPost]
    [Route("create_from_gmaps")]
    public async Task<ActionResult<LocationInformationResponse.GoogleMapsLocationInformation>>
      CreateFromGMaps(
      string token,
      [FromBody] LocationCreateFromGMapsBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var user = await _context.Users.FindAsync(userId);
      var canAccess = _paidResourceGuard.CanAccessPaidResource(user);
      var location = await _context.GoogleMapsLocations.FindAsync(body.place_id);
      if(location != null) {
        var basicDetails = await _googlePlaces.BasicPlaceDetails(body.place_id, body.sessiontoken);
        if(basicDetails != null && basicDetails.status == "OK") {
          location.BusinessStatus = basicDetails.result.business_status;
          location.ExpirationDate = DateTime.UtcNow.AddDays(100);
          location.FormattedAddress = basicDetails.result.formatted_address;
          location.Icon = basicDetails.result.icon;
          location.Name = basicDetails.result.name;
          location.Url = basicDetails.result.url;
          location.Vicinity = basicDetails.result.vicinity;
          _context.Entry(location).State = EntityState.Modified;
          try {
            await _context.SaveChangesAsync();
          } catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex.ToString());
            return Conflict();
          }
          return new LocationInformationResponse.GoogleMapsLocationInformation() {
            details = new IGooglePlaces.BasicPlaceDetailsResponse.Result {
              business_status = location.BusinessStatus,
              formatted_address = location.FormattedAddress,
              icon = location.Icon,
              name = location.Name,
              place_id = location.Id,
              url = location.Url,
              vicinity = location.Vicinity,
            },
            rating = new Rating(location).FillPersonalizedInfo(user, _critic),
          };
        }
        return NotFound();
      }
      if(!await canAccess)
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      var advancedDetails = await _googlePlaces.AdvancedPlaceDetails(body.place_id, body.sessiontoken);
      if(advancedDetails == null || advancedDetails.status != "OK")
        return NotFound();
      var score = (ulong)((advancedDetails.result.rating - 1) * 100 / 4);
      score *= advancedDetails.result.user_ratings_total;
      var priceScore = (ulong)((4 - advancedDetails.result.price_level) * 100 / 4);
      priceScore *= advancedDetails.result.user_ratings_total;
      location = new GoogleMapsLocation {
        AmountOfFood = score,
        AmountOfFoodVotes = advancedDetails.result.user_ratings_total,
        BusinessStatus = advancedDetails.result.business_status,
        EventsWithMe = new List<Event>(),
        ExpirationDate = DateTime.UtcNow.AddDays(100),
        FormattedAddress = advancedDetails.result.formatted_address,
        Icon = advancedDetails.result.icon,
        Id = advancedDetails.result.place_id,
        Name = advancedDetails.result.name,
        Price = priceScore,
        PriceVotes = advancedDetails.result.user_ratings_total,
        Taste = score,
        TasteVotes = advancedDetails.result.user_ratings_total,
        Url = advancedDetails.result.url,
        Vicinity = advancedDetails.result.vicinity,
        WaitingTime = score,
        WaitingTimeVotes = advancedDetails.result.user_ratings_total,
      };
      await _context.GoogleMapsLocations.AddAsync(location);
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return new LocationInformationResponse.GoogleMapsLocationInformation() {
        details = new IGooglePlaces.BasicPlaceDetailsResponse.Result {
          business_status = location.BusinessStatus,
          formatted_address = location.FormattedAddress,
          icon = location.Icon,
          name = location.Name,
          place_id = location.Id,
          url = location.Url,
          vicinity = location.Vicinity,
        },
        rating = new Rating(location).FillPersonalizedInfo(user, _critic),
      };
    }
    public class LocationInformationResponse {
      public class GoogleMapsLocationInformation {
        public IGooglePlaces.BasicPlaceDetailsResponse.Result details { get; set; }
        public Rating rating { get; set; }
      }
      public class CustomLocationInformation {
        public Guid id { get; set; }
        public Guid created_for_id { get; set; }
        public string address { get; set; }
        public string name { get; set; }
        public Rating rating { get; set; }
      }
      public IEnumerable<CustomLocationInformation> custom_location_infomation { get; set; }
      public IEnumerable<GoogleMapsLocationInformation> google_maps_location_information { get; set; }
    }
    public class LocationInformationBody {
      public IEnumerable<Guid>? custom_location_ids { get; set; }
      public IEnumerable<string>? google_maps_location_ids { get; set; }
    }
    [HttpPost]
    [Route("info")]
    public async Task<ActionResult<LocationInformationResponse>> Info(
      string token,
      [FromBody] LocationInformationBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var user = await _context.Users.FindAsync(userId);
      if(body.custom_location_ids == null)
        body.custom_location_ids = Enumerable.Empty<Guid>();
      if(body.google_maps_location_ids == null)
        body.google_maps_location_ids = Enumerable.Empty<string>();
      body.google_maps_location_ids = body.google_maps_location_ids.Distinct();
      if(body.google_maps_location_ids.Any(id => !_context.GoogleMapsLocations.Any(l => l.Id == id)))
        return NotFound();
      var updatedDetails = Enumerable.Empty<IGooglePlaces.BasicPlaceDetailsResponse>();
      if(body.google_maps_location_ids.Count() != 0) {
        updatedDetails = (await Task.WhenAll(from l in body.google_maps_location_ids
                                             select _googlePlaces.BasicPlaceDetails(l, null)))
                                             .OrderBy(d => d.result.place_id);
      }
      var placesToBeUpdatedIds = from d in updatedDetails
                                 select d.result.place_id;
      var placesToBeUpdated = (from l in _context.GoogleMapsLocations
                               where placesToBeUpdatedIds.Contains(l.Id)
                               orderby l.Id
                               select l).AsEnumerable();
      foreach(var (p, d) in placesToBeUpdated.Zip(updatedDetails)) {
        if(d.status != "OK")
          continue;
        p.BusinessStatus = d.result.business_status;
        p.ExpirationDate = DateTime.UtcNow.AddDays(100);
        p.FormattedAddress = d.result.formatted_address;
        p.Icon = d.result.icon;
        p.Name = d.result.name;
        p.Url = d.result.url;
        p.Vicinity = d.result.vicinity;
        _context.Entry(p).State = EntityState.Modified;
      }
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateConcurrencyException ex) {
        _logger.LogError(ex.ToString());
        foreach(var entry in ex.Entries)
          await entry.ReloadAsync();
        // This error is not fatal, so continue
      }
      var ret = new LocationInformationResponse {
        custom_location_infomation = await (from l in _context.CustomLocations
                                            where body.custom_location_ids.Contains(l.Id)
                                            select new LocationInformationResponse.CustomLocationInformation() {
                                              address = l.Address,
                                              created_for_id = l.CreatedForId,
                                              id = l.Id,
                                              name = l.Name,
                                              rating = new Rating(l),
                                            }).ToListAsync(),
        google_maps_location_information = await (from l in _context.GoogleMapsLocations
                                                  where body.google_maps_location_ids.Contains(l.Id)
                                                  select new LocationInformationResponse.GoogleMapsLocationInformation() {
                                                    details = new IGooglePlaces.BasicPlaceDetailsResponse.Result {
                                                      business_status = l.BusinessStatus,
                                                      formatted_address = l.FormattedAddress,
                                                      icon = l.Icon,
                                                      name = l.Name,
                                                      place_id = l.Id,
                                                      url = l.Url,
                                                      vicinity = l.Vicinity,
                                                    },
                                                    rating = new Rating(l),
                                                  }).ToListAsync(),
      };
      foreach(var l in ret.custom_location_infomation)
        l.rating.FillPersonalizedInfo(user, _critic);
      foreach(var l in ret.google_maps_location_information)
        l.rating.FillPersonalizedInfo(user, _critic);
      return new LocationInformationResponse {
        custom_location_infomation = ret.custom_location_infomation
                                        .OrderByDescending(l => l.rating.personalized_score),
        google_maps_location_information = ret.google_maps_location_information
                                              .OrderByDescending(l => l.rating.personalized_score),
      };
    }
    public class LocationSearchResponse {
      public class CustomLocationInformation {
        public Guid id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public Rating rating { get; set; }
      }
      public class GoogleMapsLocationInformation {
        public IGooglePlaces.BasicPlaceDetailsResponse.Result details { get; set; }
        public Rating rating { get; set; }
      }
      public IEnumerable<CustomLocationInformation> custom_locations { get; set; }
      public IEnumerable<GoogleMapsLocationInformation> google_maps_locations { get; set; }
      public IEnumerable<IGooglePlaces.PlaceAutocompleteResponse.Prediction> google_maps_locations_predictions { get; set; }
    }
    [HttpGet]
    [Route("search")]
    public async Task<ActionResult<LocationSearchResponse>> Search(
      string token,
      Guid group_id,
      [MinLength(3)] string query_string,
      [MinLength(32)] string sessiontoken
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var user = await _context.Users.FindAsync(userId);
      var canAccess = _paidResourceGuard.CanAccessPaidResource(user);
      var grp = await _context.Groups.FindAsync(group_id);
      if(grp == null)
        return NotFound();
      var ret = new LocationSearchResponse {
        custom_locations = (from l in grp.CustomLocations
                            where l.Name.Contains(query_string) || l.Address.Contains(query_string)
                            select new LocationSearchResponse.CustomLocationInformation {
                              address = l.Address,
                              id = l.Id,
                              name = l.Name,
                              rating = new Rating(l),
                            }).ToList(),
        google_maps_locations = await (from l in _context.GoogleMapsLocations
                                       where l.Name.Contains(query_string) || l.FormattedAddress.Contains(query_string)
                                       select new LocationSearchResponse.GoogleMapsLocationInformation {
                                         details = new IGooglePlaces.BasicPlaceDetailsResponse.Result {
                                           business_status = l.BusinessStatus,
                                           formatted_address = l.FormattedAddress,
                                           icon = l.Icon,
                                           name = l.Name,
                                           place_id = l.Id,
                                           url = l.Url,
                                           vicinity = l.Vicinity,
                                         },
                                         rating = new Rating(l),
                                       }).ToListAsync(),
        google_maps_locations_predictions = Enumerable.Empty<IGooglePlaces.PlaceAutocompleteResponse.Prediction>(),
      };
      foreach(var l in ret.custom_locations)
        l.rating.FillPersonalizedInfo(user, _critic);
      foreach(var l in ret.google_maps_locations)
        l.rating.FillPersonalizedInfo(user, _critic);
      if(await canAccess) {
        var placesResponse = await _googlePlaces.PlaceAutocomplete(query_string, sessiontoken);
        if(placesResponse != null && placesResponse.status == "OK" && placesResponse.predictions != null)
          ret.google_maps_locations_predictions = placesResponse.predictions;
      }
      return new LocationSearchResponse {
        custom_locations = ret.custom_locations.OrderByDescending(l => l.rating.personalized_score),
        google_maps_locations = ret.google_maps_locations.OrderByDescending(l => l.rating.personalized_score),
        google_maps_locations_predictions = ret.google_maps_locations_predictions,
      };
    }
    public class LocationRateBody {
      [Range(0, 100)]
      public ulong? taste { get; set; }
      [Range(0, 100)]
      public ulong? price { get; set; }
      [Range(0, 100)]
      public ulong? amount_of_food { get; set; }
      [Range(0, 100)]
      public ulong? waiting_time { get; set; }
      public string? google_maps_id { get; set; }
      public Guid? custom_location_id { get; set; }
    }
    [HttpPost]
    [Route("rate")]
    public async Task<ActionResult> Rate(
      string token,
      [FromBody] LocationRateBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      if(
        (body.amount_of_food, body.price, body.taste, body.waiting_time) is (null, null, null, null)
      ) {
        return new StatusCodeResult(418);
      }
      if(
        !((body.custom_location_id, body.google_maps_id) is (null, string) or (Guid, null))
      ) {
        return new StatusCodeResult(418);
      }
      void rate(LocationBase location) {
        if(body.amount_of_food != null) {
          location.AmountOfFood += (ulong)body.amount_of_food;
          ++location.AmountOfFoodVotes;
        }
        if(body.price != null) {
          location.Price += (ulong)body.price;
          ++location.PriceVotes;
        }
        if(body.taste != null) {
          location.Taste += (ulong)body.taste;
          ++location.TasteVotes;
        }
        if(body.waiting_time != null) {
          location.WaitingTime += (ulong)body.waiting_time;
          ++location.WaitingTimeVotes;
        }
      }
      if(body.custom_location_id != null) {
        var location = await _context.CustomLocations.FindAsync(body.custom_location_id);
        if(location == null)
          return NotFound();
        rate(location);
        _context.Entry(location).State = EntityState.Modified;
      }
      if(body.google_maps_id != null) {
        var location = await _context.GoogleMapsLocations.FindAsync(body.google_maps_id);
        if(location == null)
          return NotFound();
        rate(location);
        _context.Entry(location).State = EntityState.Modified;
      }
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateConcurrencyException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return Ok();
    }
    private readonly IUserManager _userManager;
    private readonly LMDbContext _context;
    private readonly IGooglePlaces _googlePlaces;
    private readonly IPaidResourceGuard _paidResourceGuard;
    private readonly ILocationCritic _critic;
    private readonly ILogger<LocationsController> _logger;
  }
}
