using LetsMeatAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
        CandidateCustomLocations = new List<CustomLocation>(),
        CandidateGoogleMapsLocations = new List<GoogleMapsLocation>(),
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
      public Guid[]? custom_locations_ids { get; set; }
      public string[]? google_maps_locations_ids { get; set; }
      public DateTime[]? candidate_times { get; set; }
    }
    [HttpPatch]
    [Route("update")]
    public async Task<ActionResult<EventInformationResponse>> Update(
      string token,
      [FromBody] EventUpdateBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      if(
        body.name == null &&
        body.deadline == null &&
        body.candidate_times == null &&
        body.custom_locations_ids == null &&
        body.google_maps_locations_ids == null
      ) {
        return new StatusCodeResult(418);
      }
      var ev = await _context.Events.FindAsync(body.id);
      if(
        (body.name != null ||
        body.deadline != null) &&
        userId != ev.CreatorId
      ) {
        return Unauthorized();
      }
      if(ev == null)
        return NotFound();
      if(body.name != null)
        ev.Name = body.name;
      if(body.deadline != null)
        ev.Deadline = (DateTime)body.deadline;
      if(body.custom_locations_ids != null) {
        var uids = body.custom_locations_ids.Distinct().ToArray();
        var diff = from id in uids
                   where !_context.CustomLocations.Any(l => l.Id == id)
                   select id;
        if(diff.Count() != 0)
          return NotFound(diff);
        var locs = from l in _context.CustomLocations
                   where uids.Contains(l.Id) &&
                         !ev.CandidateCustomLocations.Contains(l)
                   select l;
        foreach(var l in locs)
          ev.CandidateCustomLocations.Add(l);
      }
      if(body.google_maps_locations_ids != null) {
        var uids = body.google_maps_locations_ids.Distinct().ToArray();
        var diff = from id in uids
                   where !_context.GoogleMapsLocations.Any(l => l.Id == id)
                   select id;
        if(diff.Count() != 0)
          return NotFound(diff);
        var locs = from l in _context.GoogleMapsLocations
                   where uids.Contains(l.Id) &&
                         !ev.CandidateGoogleMapsLocations.Contains(l)
                   select l;
        foreach(var l in locs)
          ev.CandidateGoogleMapsLocations.Add(l);
      }
      if(body.candidate_times != null) {
        var arr = JsonSerializer.Deserialize<IEnumerable<DateTime>>(ev.CandidateTimes);
        ev.CandidateTimes = JsonSerializer.Serialize(arr.Union(body.candidate_times));
      }
      _context.Entry(ev).State = EntityState.Modified;
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateConcurrencyException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return new EventInformationResponse() {
        candidate_custom_locations = (from location in ev.CandidateCustomLocations
                                      select location.Id).ToArray(),
        candidate_google_maps_locations = (from location in ev.CandidateGoogleMapsLocations
                                           select location.Id).ToArray(),
        candidate_times = JsonSerializer.Deserialize<DateTime[]>(ev.CandidateTimes),
        creator_id = ev.CreatorId,
        deadline = ev.Deadline,
        group_id = ev.GroupId,
        id = ev.Id,
        locations_result = null,
        name = ev.Name,
        times_result = null,
      };
    }
    public class EventInformationResponse {
      public Guid id { get; set; }
      public Guid group_id { get; set; }
      public string creator_id { get; set; }
      public string name { get; set; }
      public DateTime[] candidate_times { get; set; }
      public Guid[] candidate_custom_locations { get; set; }
      public string[] candidate_google_maps_locations { get; set; }
      public DateTime deadline { get; set; }
      public Guid[]? locations_result { get; set; }
      public DateTime[]? times_result { get; set; }
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
        candidate_custom_locations = (from location in ev.CandidateCustomLocations
                                      select location.Id).ToArray(),
        candidate_google_maps_locations = (from location in ev.CandidateGoogleMapsLocations
                                           select location.Id).ToArray(),
        candidate_times = JsonSerializer.Deserialize<DateTime[]>(ev.CandidateTimes),
        creator_id = ev.CreatorId,
        deadline = ev.Deadline,
        group_id = ev.GroupId,
        id = ev.Id,
        locations_result = null,
        name = ev.Name,
        times_result = null,
      };
    }
    public class EventDeleteBody {
      public Guid id { get; set; }
    }
    [HttpDelete]
    [Route("delete")]
    public async Task<ActionResult> Delete(
      string token,
      [FromBody] EventDeleteBody body
      ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var ev = await _context.Events.FindAsync(body.id);
      if(ev == null)
        return NotFound();
      if(ev.CreatorId != userId)
        return Unauthorized();
      _context.Votes.RemoveRange(ev.Votes);
      _context.Entry(ev).State = EntityState.Deleted;
      await _context.SaveChangesAsync();
      return Ok();
    }
    private readonly UserManager _userManager;
    private readonly LMDbContext _context;
    private readonly ILogger<EventsController> _logger;
  }
}
