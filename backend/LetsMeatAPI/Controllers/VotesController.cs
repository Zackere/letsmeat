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
  public class VotesController : ControllerBase {
    public VotesController(
      IUserManager userManager,
      LMDbContext context,
      ILogger<VotesController> logger
    ) {
      _userManager = userManager;
      _context = context;
      _logger = logger;
    }
    public class VoteInformation {
      public class LocationInformation {
        public string? google_maps_location_id { get; set; }
        public Guid? custom_location_id { get; set; }
      }
      public DateTime[]? times { get; set; }
      public LocationInformation[]? locations { get; set; }
    }
    [HttpGet]
    [Route("result")]
    public async Task<ActionResult<VoteInformation>> Result(
      string token,
      Guid event_id
    ) {
      return new StatusCodeResult(501);
    }
    [HttpGet]
    [Route("get")]
    public async Task<ActionResult<VoteInformation>> Get(
      string token,
      Guid event_id
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var ev = await _context.Events.FindAsync(event_id);
      if(ev == null)
        return NotFound();
      var vote = await _context.Votes.FindAsync(event_id, userId);
      if(vote == null) {
        return new VoteInformation {
          locations = ev.CandidateGoogleMapsLocations
                        .Select(l => new VoteInformation.LocationInformation {
                          google_maps_location_id = l.Id,
                        })
                        .Union(ev.CandidateCustomLocations
                        .Select(l => new VoteInformation.LocationInformation {
                          custom_location_id = l.Id,
                        }))
                        .ToArray(),
          times = JsonSerializer.Deserialize<IEnumerable<DateTime>>(ev.CandidateTimes)
                  .Select(t => DateTime.SpecifyKind(t, DateTimeKind.Utc))
                  .ToArray(),
        };
      }
      var ret = JsonSerializer.Deserialize<VoteInformation>(vote.Order);
      // Union guarantees to preserve order
      ret.locations = ret.locations
        .Where(l => {
          if(l.custom_location_id != null)
            return _context.CustomLocations.Any(c => c.Id == l.custom_location_id);
          if(l.google_maps_location_id != null)
            return _context.GoogleMapsLocations.Any(c => c.Id == l.google_maps_location_id);
          return false;
        })
        .Union(ev.CandidateCustomLocations
          .Select(l =>
            new VoteInformation.LocationInformation() {
              custom_location_id = l.Id
            })
          )
        .Union(ev.CandidateGoogleMapsLocations
          .Select(l =>
            new VoteInformation.LocationInformation() {
              google_maps_location_id = l.Id
            })
          )
        .ToArray();
      ret.times = ret.times
        .Union(JsonSerializer.Deserialize<IEnumerable<DateTime>>(ev.CandidateTimes))
        .Select(t => DateTime.SpecifyKind(t, DateTimeKind.Utc))
        .ToArray();
      var newOrder = JsonSerializer.Serialize(ret);
      if(newOrder != vote.Order) {
        vote.Order = newOrder;
        _context.Entry(vote).State = EntityState.Modified;
        try {
          await _context.SaveChangesAsync();
        } catch(DbUpdateConcurrencyException ex) {
          _logger.LogError(ex.ToString());
          // Ignore this error, it won't mess up voting anyways
        }
      }
      return ret;
    }
    public class VoteCastBody {
      public VoteInformation vote_information { get; set; }
      public Guid event_id { get; set; }
    }
    [HttpPost]
    [Route("cast")]
    public async Task<ActionResult> Cast(
      string token,
      [FromBody] VoteCastBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var ev = await _context.Events.FindAsync(body.event_id);
      if(ev == null)
        return NotFound();
      if(DateTime.SpecifyKind(ev.Deadline, DateTimeKind.Utc) < DateTime.UtcNow)
        return new StatusCodeResult(418);
      if(body.vote_information.locations == null)
        body.vote_information.locations = new VoteInformation.LocationInformation[] { };
      if(body.vote_information.times == null)
        body.vote_information.times = new DateTime[] { };
      if(body.vote_information.locations
        .Any(l => l.google_maps_location_id == null && l.custom_location_id == null)) {
        return new StatusCodeResult(418);
      }
      if(body.vote_information.locations
        .Where(l => l.custom_location_id != null)
        .Any(l => !_context.CustomLocations.Any(c => c.Id == l.custom_location_id))) {
        return NotFound();
      }
      if(body.vote_information.locations
        .Where(l => l.google_maps_location_id != null)
        .Any(l => !_context.GoogleMapsLocations.Any(c => c.Id == l.google_maps_location_id))) {
        return NotFound();
      }
      var candidateTimes = JsonSerializer.Deserialize<IEnumerable<DateTime>>(ev.CandidateTimes);
      var timesDiff = body.vote_information.times.Except(candidateTimes);
      if(timesDiff.Count() > 0)
        return NotFound(timesDiff);
      body.vote_information.locations =
        body.vote_information.locations
        .Distinct()
        .ToArray();
      body.vote_information.times =
        body.vote_information.times
        .Select(t => DateTime.SpecifyKind(t, DateTimeKind.Utc))
        .Distinct()
        .ToArray();
      var vote = await _context.Votes.FindAsync(body.event_id, userId);
      if(vote == null) {
        vote = new() {
          EventId = body.event_id,
          Order = JsonSerializer.Serialize(body.vote_information),
          UserId = userId,
        };
        await _context.Votes.AddAsync(vote);
      } else {
        vote.Order = JsonSerializer.Serialize(body.vote_information);
        _context.Entry(vote).State = EntityState.Modified;
      }
      try {
        await _context.SaveChangesAsync();
      } catch(Exception ex)
        when(
        ex is DbUpdateConcurrencyException ||
        ex is DbUpdateException
      ) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return Ok();
    }
    private readonly IUserManager _userManager;
    private readonly LMDbContext _context;
    private readonly ILogger<VotesController> _logger;
  }
}