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
  public class GroupsController : ControllerBase {
    public GroupsController(
      UserManager userManager,
      LMDbContext context,
      ILogger<GroupsController> logger
    ) {
      _userManager = userManager;
      _context = context;
      _logger = logger;
    }
    public class GroupCreatedResponse {
      public Guid id { get; set; }
      public string name { get; set; }
    }
    public class GroupCreateBody {
      public string name { get; set; }
    }
    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<GroupCreatedResponse>> Create(
      string token,
      [FromBody] GroupCreateBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var grp = new Models.Group() {
        Name = body.name,
        Users = new List<Models.User>() { _context.Users.Find(userId) },
        Locations = new List<Models.Location>(),
        Events = new List<Models.Event>()
      };
      _context.Groups.Add(grp);
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return Ok(new GroupCreatedResponse() {
        id = grp.Id,
        name = grp.Name
      });
    }
    public class GroupInformationResponse {
      public class UserInformation {
        public string id { get; set; }
        public string picture_url { get; set; }
        public string email { get; set; }
        public string name { get; set; }
      }
      public class LocationInformation {
        public Guid id { get; set; }
        public string info { get; set; }
      }
      public class EventInformation {
        public Guid id { get; set; }
        public string name { get; set; }
        public DateTime deadline { get; set; }
      }
      public Guid id { get; set; }
      public string name { get; set; }
      public IEnumerable<UserInformation> users { get; set; }
      public IEnumerable<LocationInformation> locations { get; set; }
      public IEnumerable<EventInformation> events { get; set; }
    }
    [HttpGet]
    [Route("info")]
    public async Task<ActionResult<GroupInformationResponse>> Info(
      string token,
      Guid id
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var grp = await _context.Groups.FindAsync(id);
      if(grp == null)
        return NotFound();
      return Ok(new GroupInformationResponse {
        id = grp.Id,
        name = grp.Name,
        users = from user in grp.Users
                select new GroupInformationResponse.UserInformation {
                  id = user.Id,
                  picture_url = user.PictureUrl,
                  email = user.Email,
                  name = user.Name
                },
        locations = from location in grp.Locations
                    select new GroupInformationResponse.LocationInformation {
                      id = location.Id,
                      info = location.Info
                    },
        events = from evnt in grp.Events
                 select new GroupInformationResponse.EventInformation {
                   id = evnt.Id,
                   name = evnt.Name,
                   deadline = evnt.Deadline
                 }
      });
    }
    private readonly UserManager _userManager;
    private readonly LMDbContext _context;
    private readonly ILogger<GroupsController> _logger;
  }
}
