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
  public class EventsController : ControllerBase {
    public EventsController(
      UserManager userManager,
      LMDbContext context,
      ILogger<EventsController> logger
    ) {
      _userManager = userManager;
      _context = context;
      _logger = logger;
    }
    public class EventCreatedResponse {
      public Guid id { get; set; }
    }
    public class EventCreateBody {
      public Guid group_id { get; set; }
      public string name { get; set; }
      public DateTime deadline { get; set; }
    }
    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<EventCreatedResponse>> Create(
      string token,
      [FromBody] EventCreateBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var grp = await _context.Groups.FindAsync(body.group_id);
      if(grp == null)
        return NotFound();
      if(!grp.Users.Any(u => u.Id == userId))
        return Unauthorized();
      var ev = new Event() {
        CandidateLocations = new List<Location>(),
        CandidateTimes = "[]",
        CreatorId = userId,
        Deadline = body.deadline,
        GroupId = grp.Id,
        Name = body.name,
        Votes = new List<Vote>(),
      };
      grp.Events.Add(ev);
      await _context.Events.AddAsync(ev);
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return new EventCreatedResponse() {
        id = ev.Id,
      };
    }
    public class EventUpdateBody {
      public Guid id { get; set; }
      public string? name { get; set; }
      public DateTime? deadline { get; set; }
    }
    [HttpPatch]
    [Route("update")]
    public async Task<ActionResult> Update(
      string token,
      [FromBody] EventUpdateBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      if(body.name == null && body.deadline == null)
        return new StatusCodeResult(418);
      var ev = await _context.Events.FindAsync(body.id);
      if(body.name != null)
        ev.Name = body.name;
      if(body.deadline != null)
        ev.Deadline = (DateTime)body.deadline;
      _context.Entry(ev).State = EntityState.Modified;
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateConcurrencyException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return Ok();
    }
    public class EventInformationResponse {
      public Guid id { get; set; }
      public Guid group_id { get; set; }
      public string creator_id { get; set; }
      public string name { get; set; }
      public DateTime[] candidate_times { get; set; }
      public Guid[] candidate_locations { get; set; }
      public DateTime deadline { get; set; }
      public Guid[] locations_result { get; set; }
      public DateTime[] times_result { get; set; }
    }
    [HttpGet]
    [Route("info")]
    public async Task<ActionResult<EventInformationResponse>> Info(
      string token,
      Guid id
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var ev = await _context.Events.FindAsync(id);
      if(ev == null)
        return NotFound();
      return new EventInformationResponse() {
        candidate_locations = new Guid[] { },
        candidate_times = new DateTime[] { },
        creator_id = ev.CreatorId,
        deadline = ev.Deadline,
        group_id = ev.GroupId,
        id = ev.Id,
        locations_result = new Guid[] { },
        name = ev.Name,
        times_result = new DateTime[] { },
      };
    }
    private readonly UserManager _userManager;
    private readonly LMDbContext _context;
    private readonly ILogger<EventsController> _logger;
  }
}