using Azure.Storage.Blobs.Models;
using LetsMeatAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LetsMeatAPI.Controllers {
  [Route("[controller]")]
  [ApiController]
  public class GroupsController : ControllerBase {
    public GroupsController(
      IUserManager userManager,
      LMDbContext context,
      IBlobClientFactory blobClientFactory,
      ILogger<GroupsController> logger
    ) {
      _userManager = userManager;
      _context = context;
      _blobClientFactory = blobClientFactory;
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
      if(userId is null)
        return Unauthorized();
      var user = await _context.Users.FindAsync(userId);
      var grp = new Group {
        Name = body.name,
        Owner = user,
        Users = new List<User> { user },
      };
      await _context.Groups.AddAsync(grp);
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return new GroupCreatedResponse {
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
      }
      public class EventInformation {
        public Guid id { get; set; }
        public string name { get; set; }
        public DateTime deadline { get; set; }
        public string creator_id { get; set; }
      }
      public Guid id { get; set; }
      public string name { get; set; }
      public string owner_id { get; set; }
      public IEnumerable<UserInformation> users { get; set; }
      public IEnumerable<CustomLocationInformation> custom_locations { get; set; }
      public IEnumerable<EventInformation> events { get; set; }
      public IEnumerable<Guid> images { get; set; }
    }
    [HttpGet]
    [Route("info")]
    public async Task<ActionResult<GroupInformationResponse>> Info(
      string token,
      Guid id
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId is null)
        return Unauthorized();
      var grp = await _context.Groups.FindAsync(id);
      if(grp is null)
        return NotFound();
      return new GroupInformationResponse {
        id = grp.Id,
        images = from image in grp.Images
                 select image.Id,
        name = grp.Name,
        owner_id = grp.OwnerId,
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
                           },
        events = from ev in grp.Events
                 orderby ev.Deadline descending
                 select new GroupInformationResponse.EventInformation {
                   id = ev.Id,
                   name = ev.Name,
                   deadline = DateTime.SpecifyKind(ev.Deadline, DateTimeKind.Utc),
                   creator_id = ev.CreatorId,
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
      if(userId is null)
        return Unauthorized();
      var grp = await _context.Groups.FindAsync(body.id);
      if(grp is null)
        return NotFound();
      var user = await _context.Users.FindAsync(userId);
      grp.Users.Add(user);
      var inv = await _context.Invitations.FindAsync(userId, grp.Id);
      if(inv is not null)
        _context.Invitations.Remove(inv);
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
      if(userId is null)
        return Unauthorized();
      var grp = await _context.Groups.FindAsync(body.id);
      if(grp is null)
        return NotFound();
      if(grp.OwnerId != userId)
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      _context.Events.RemoveRange(grp.Events);
      _context.CustomLocations.RemoveRange(grp.CustomLocations);
      foreach(var image in grp.Images) {
        var client = _blobClientFactory.GetClientFromUri(new Uri(image.Url));
        try {
          await client.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        } catch(Azure.RequestFailedException ex) {
          _logger.LogError(ex.ToString());
        }
      }
      _context.Images.RemoveRange(grp.Images);
      _context.PendingDebts.RemoveRange(grp.PendingDebts);
      _context.Groups.Remove(grp);
      await _context.SaveChangesAsync();
      return Ok();
    }
    public class GroupLeaveBody {
      public Guid id { get; set; }
    }
    [HttpPost]
    [Route("leave")]
    public async Task<ActionResult> Leave(
      string token,
      [FromBody] GroupLeaveBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId is null)
        return Unauthorized();
      var grp = await _context.Groups.FindAsync(body.id);
      if(grp is null)
        return NotFound();
      var user = await _context.Users.FindAsync(userId);
      var c = grp.Users.Count();
      grp.Users.Remove(user);
      var d = grp.Users.Count();
      if(user.Id == grp.OwnerId) {
        // Try to transfer ownership of the group
        var candidateOwner = grp.Users.SingleOrDefault();
        if(candidateOwner is null)
          return await Delete(token, new() { id = grp.Id });
        grp.OwnerId = candidateOwner.Id;
        _context.Entry(grp).State = EntityState.Modified;
        foreach(var ev in from ev in grp.Events
                          where ev.CreatorId == userId
                          select ev) {
          ev.CreatorId = candidateOwner.Id;
          _context.Entry(ev).State = EntityState.Modified;
        }
      }
      _context.RemoveRange(from debt in _context.PendingDebts
                           where debt.GroupId == grp.Id &&
                           (debt.FromId == user.Id ||
                           debt.ToId == user.Id)
                           select debt);
      _context.RemoveRange(from debt in _context.Debts
                           where debt.GroupId == grp.Id &&
                           (debt.FromId == user.Id ||
                           debt.ToId == user.Id)
                           select debt);
      _context.RemoveRange(from inv in _context.Invitations
                           where inv.GroupId == grp.Id &&
                           (inv.FromId == user.Id ||
                           inv.ToId == user.Id)
                           select inv);
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
    private readonly IBlobClientFactory _blobClientFactory;
    private readonly ILogger<GroupsController> _logger;
  }
}
