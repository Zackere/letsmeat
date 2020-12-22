using LetsMeatAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LetsMeatAPI.Controllers {
  [Route("[controller]")]
  [ApiController]
  public class LocationsController : ControllerBase {
    public LocationsController(
      UserManager userManager,
      LMDbContext context,
      ILogger<LocationsController> logger
    ) {
      _userManager = userManager;
      _context = context;
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
      public void FillPersonalizedInfo(User user) {
        // TODO(wreplin): Fill in a meaningful way based on user preferences
        var rnd = new Random();
        overall_score = rnd.NextDouble() * 100;
        personalized_score = rnd.NextDouble() * 100;
      }
    }
    public class LocationCreatedCustomResponse {
      public Guid id { get; set; }
      public Guid created_for_id { get; set; }
      public string address { get; set; }
      public string name { get; set; }
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
    public async Task<ActionResult<LocationCreatedCustomResponse>> CreateCustom(
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
        return Forbid();
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
      return new LocationCreatedCustomResponse() {
        address = location.Address,
        created_for_id = location.CreatedForId,
        id = location.Id,
        name = location.Name,
      };
    }
    public class LocationInformationResponse {
      public class GoogleMapsLocationInformation {
        public string id { get; set; }
        public string address { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public string name { get; set; }
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
      var ret = new LocationInformationResponse {
        custom_location_infomation = await (from l in _context.CustomLocations
                                            where body.custom_location_ids.Contains(l.Id)
                                            select new LocationInformationResponse.CustomLocationInformation() {
                                              address = l.Address,
                                              created_for_id = l.CreatedForId,
                                              id = l.Id,
                                              name = l.Name,
                                              rating = new Rating {
                                                amount_of_food = l.AmountOfFood,
                                                amount_of_food_votes = l.AmountOfFoodVotes,
                                                price = l.Price,
                                                price_votes = l.PriceVotes,
                                                taste = l.Taste,
                                                taste_votes = l.TasteVotes,
                                                waiting_time = l.WaitingTime,
                                                waiting_time_votes = l.WaitingTimeVotes,
                                              }
                                            }).ToListAsync(),
        google_maps_location_information = await (from l in _context.GoogleMapsLocations
                                                  where body.google_maps_location_ids.Contains(l.Id)
                                                  select new LocationInformationResponse.GoogleMapsLocationInformation() {
                                                    address = l.Address,
                                                    id = l.Id,
                                                    latitude = l.Latitude,
                                                    longitude = l.Longitude,
                                                    name = l.Name,
                                                    rating = new Rating {
                                                      amount_of_food = l.AmountOfFood,
                                                      amount_of_food_votes = l.AmountOfFoodVotes,
                                                      price = l.Price,
                                                      price_votes = l.PriceVotes,
                                                      taste = l.Taste,
                                                      taste_votes = l.TasteVotes,
                                                      waiting_time = l.WaitingTime,
                                                      waiting_time_votes = l.WaitingTimeVotes,
                                                    }
                                                  }).ToListAsync(),
      };
      foreach(var l in ret.custom_location_infomation)
        l.rating.FillPersonalizedInfo(user);
      foreach(var l in ret.google_maps_location_information)
        l.rating.FillPersonalizedInfo(user);
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
        public string id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public Rating rating { get; set; }
      }
      public IEnumerable<CustomLocationInformation> custom_locations { get; set; }
      public IEnumerable<GoogleMapsLocationInformation> google_maps_locations { get; set; }
    }
    [HttpGet]
    [Route("search")]
    public async Task<ActionResult<LocationSearchResponse>> Search(
      string token,
      Guid group_id,
      [MinLength(3)] string query_string
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var user = await _context.Users.FindAsync(userId);
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
                              rating = new Rating {
                                amount_of_food = l.AmountOfFood,
                                amount_of_food_votes = l.AmountOfFoodVotes,
                                price = l.Price,
                                price_votes = l.PriceVotes,
                                taste = l.Taste,
                                taste_votes = l.TasteVotes,
                                waiting_time = l.WaitingTime,
                                waiting_time_votes = l.WaitingTimeVotes,
                              },
                            }).ToList(),
        google_maps_locations = await (from l in _context.GoogleMapsLocations
                                       where l.Name.Contains(query_string) || l.Address.Contains(query_string)
                                       select new LocationSearchResponse.GoogleMapsLocationInformation {
                                         address = l.Address,
                                         id = l.Id,
                                         name = l.Name,
                                         rating = new Rating {
                                           amount_of_food = l.AmountOfFood,
                                           amount_of_food_votes = l.AmountOfFoodVotes,
                                           price = l.Price,
                                           price_votes = l.PriceVotes,
                                           taste = l.Taste,
                                           taste_votes = l.TasteVotes,
                                           waiting_time = l.WaitingTime,
                                           waiting_time_votes = l.WaitingTimeVotes,
                                         },
                                       }).ToListAsync(),
      };
      foreach(var l in ret.custom_locations)
        l.rating.FillPersonalizedInfo(user);
      foreach(var l in ret.google_maps_locations)
        l.rating.FillPersonalizedInfo(user);
      return new LocationSearchResponse {
        custom_locations = ret.custom_locations.OrderByDescending(l => l.rating.personalized_score),
        google_maps_locations = ret.google_maps_locations.OrderByDescending(l => l.rating.personalized_score),
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
      if(body.custom_location_id != null) {
        var location = await _context.CustomLocations.FindAsync(body.custom_location_id);
        if(location == null)
          return NotFound();
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
        _context.Entry(location).State = EntityState.Modified;
      }
      if(body.google_maps_id != null) {
        var location = await _context.GoogleMapsLocations.FindAsync(body.google_maps_id);
        if(location == null)
          return NotFound();
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
    private readonly UserManager _userManager;
    private readonly LMDbContext _context;
    private readonly ILogger<LocationsController> _logger;
  }
}