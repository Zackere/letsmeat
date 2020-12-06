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
      var user = await _context.Users.FindAsync(userId);
      var grp = new Models.Group() {
        CustomLocations = new List<Models.CustomLocation>(),
        Name = body.name,
        Owner = user,
        Users = new List<Models.User>() { user },
        Events = new List<Models.Event>()
      };
      await _context.Groups.AddAsync(grp);
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return new GroupCreatedResponse() {
        id = grp.Id,
        name = grp.Name
      };
    }
    public class GroupInformationResponse {
      public class UserInformation {
        public string id { get; set; }
        public string picture_url { get; set; }
        public string name { get; set; }
      }
      public class CustomLocationInformation {
        public Guid id { get; set; }
        public Guid created_for_id { get; set; }
        public string address { get; set; }
        public string name { get; set; }
        public string rating { get; set; }
      }
      public class EventInformation {
        public Guid id { get; set; }
        public string name { get; set; }
        public DateTime deadline { get; set; }
      }
      public Guid id { get; set; }
      public string name { get; set; }
      public IEnumerable<UserInformation> users { get; set; }
      public IEnumerable<CustomLocationInformation> custom_locations { get; set; }
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
      return new GroupInformationResponse {
        id = grp.Id,
        name = grp.Name,
        users = from user in grp.Users
                select new GroupInformationResponse.UserInformation {
                  id = user.Id,
                  picture_url = user.PictureUrl,
                  name = user.Name,
                },
        custom_locations = from location in grp.CustomLocations
                           select new GroupInformationResponse.CustomLocationInformation {
                             id = location.Id,
                             address = location.Address,
                             name = location.Name,
                             created_for_id = location.CreatedForId,
                             rating = location.Rating,
                           },
        events = from evnt in grp.Events
                 select new GroupInformationResponse.EventInformation {
                   id = evnt.Id,
                   name = evnt.Name,
                   deadline = evnt.Deadline,
                 }
      };
    }
    public class GroupJoinBody {
      public Guid id { get; set; }
    }
    [HttpPost]
    [Route("join")]
    public async Task<ActionResult> Join(
      string token,
      [FromBody] GroupJoinBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var grp = await _context.Groups.FindAsync(body.id);
      if(grp == null)
        return NotFound();
      var user = await _context.Users.FindAsync(userId);
      grp.Users.Add(user);
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return Ok();
    }
    public class GroupDeleteBody {
      public Guid id { get; set; }
    }
    [HttpDelete]
    [Route("delete")]
    public async Task<ActionResult> Delete(
      string token,
      [FromBody] GroupDeleteBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var grp = await _context.Groups.FindAsync(body.id);
      if(grp == null)
        return NotFound();
      if(grp.OwnerId != userId)
        return Unauthorized();
      _context.Events.RemoveRange(grp.Events);
      _context.CustomLocations.RemoveRange(grp.CustomLocations);
      _context.Groups.Remove(grp);
      await _context.SaveChangesAsync();
      return Ok();
    }
    private readonly UserManager _userManager;
    private readonly LMDbContext _context;
    private readonly ILogger<GroupsController> _logger;
  }
}
