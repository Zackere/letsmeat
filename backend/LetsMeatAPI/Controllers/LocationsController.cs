using LetsMeatAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
    public class LocationCreatedCustomResponse {
      public Guid id { get; set; }
      public Guid created_for_id { get; set; }
      public string address { get; set; }
      public string name { get; set; }
      public string rating { get; set; }
    }
    public class LocationCreateCustomBody {
      public Guid group_id { get; set; }
      public string Address { get; set; }
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
        Rating = "{}",
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
        rating = location.Rating,
      };
    }
    public class LocationInformationResponse {
      public class GoogleMapsLocationInformation {
        public string id { get; set; }
        public string address { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public string name { get; set; }
        public string rating { get; set; }
      }
      public class CustomLocationInformation {
        public Guid id { get; set; }
        public Guid created_for_id { get; set; }
        public string address { get; set; }
        public string name { get; set; }
        public string rating { get; set; }
      }
      public CustomLocationInformation[] custom_location_infomation { get; set; }
      public GoogleMapsLocationInformation[] google_maps_location_information { get; set; }
    }
    public class LocationInformationBody {
      public Guid[] custom_location_ids { get; set; }
      public string[] google_maps_location_ids { get; set; }
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

      return new LocationInformationResponse() {
        custom_location_infomation = await (from l in _context.CustomLocations
                                            where body.custom_location_ids.Contains(l.Id)
                                            select new LocationInformationResponse.CustomLocationInformation() {
                                              address = l.Address,
                                              created_for_id = l.CreatedForId,
                                              id = l.Id,
                                              name = l.Name,
                                              rating = l.Rating,
                                            }).ToArrayAsync(),
        google_maps_location_information = await (from l in _context.GoogleMapsLocations
                                                  where body.google_maps_location_ids.Contains(l.Id)
                                                  select new LocationInformationResponse.GoogleMapsLocationInformation() {
                                                    address = l.Address,
                                                    id = l.Id,
                                                    latitude = l.Latitude,
                                                    longitude = l.Longitude,
                                                    name = l.Name,
                                                    rating = l.Rating,
                                                  }).ToArrayAsync(),
      };
    }
    private readonly UserManager _userManager;
    private readonly LMDbContext _context;
    private readonly ILogger<LocationsController> _logger;
  }
}