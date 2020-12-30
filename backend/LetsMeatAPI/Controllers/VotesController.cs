using LetsMeatAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
      ILocationCritic critic,
      IElectionHolder electionHolder,
      ILogger<VotesController> logger
    ) {
      _userManager = userManager;
      _context = context;
      _critic = critic;
      _electionHolder = electionHolder;
      _logger = logger;
    }
    public class VoteInformation {
      public class LocationInformation : IEquatable<LocationInformation> {
        public string? google_maps_location_id { get; set; }
        public Guid? custom_location_id { get; set; }
        public bool Equals([AllowNull] LocationInformation other) {
          return other != null &&
                 google_maps_location_id == other.google_maps_location_id &&
                 custom_location_id == other.custom_location_id;
        }
        public override bool Equals([AllowNull] object other) {
          return Equals(other as LocationInformation);
        }
        public override int GetHashCode() {
          if(google_maps_location_id != null)
            return google_maps_location_id.GetHashCode();
          return custom_location_id.GetHashCode();
        }
      }
      public IEnumerable<DateTime>? times { get; set; }
      public IEnumerable<LocationInformation>? locations { get; set; }
    }
    [HttpGet]
    [Route("result")]
    public async Task<ActionResult<VoteInformation>> Result(
      string token,
      Guid event_id
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var user = await _context.Users.FindAsync(userId);
      var ev = await _context.Events.FindAsync(event_id);
      var candidateLocationToOrdinal = new Dictionary<VoteInformation.LocationInformation, int>();
      var ordinalToCandidateLocation = new Dictionary<int, VoteInformation.LocationInformation>();
      var ordinal = 0;
      foreach(var l in ev.CandidateGoogleMapsLocations) {
        var candidate = new VoteInformation.LocationInformation {
          google_maps_location_id = l.Id,
        };
        candidateLocationToOrdinal[candidate] = ordinal;
        ordinalToCandidateLocation[ordinal] = candidate;
        ++ordinal;
      }
      foreach(var l in ev.CandidateCustomLocations) {
        var candidate = new VoteInformation.LocationInformation {
          custom_location_id = l.Id,
        };
        candidateLocationToOrdinal[candidate] = ordinal;
        ordinalToCandidateLocation[ordinal] = candidate;
        ++ordinal;
      }
      var votes = from vote in ev.Votes
                  select CompleteVoteIfNotEmpty(
                    JsonSerializer.Deserialize<VoteInformation>(vote.Order),
                    ev,
                    user
                  );
      var locationVotes = from vote in votes
                          where vote.locations.Any()
                          select from location in vote.locations
                                 select candidateLocationToOrdinal[location];
      var locationOrder = _electionHolder.DecideWinner(ordinal, locationVotes);
      ordinal = 0;
      var candidateTimeToOrdinal = new Dictionary<DateTime, int>();
      var ordinalToCandidateTime = new Dictionary<int, DateTime>();
      foreach(var time in from time in JsonSerializer.Deserialize<IEnumerable<DateTime>>(ev.CandidateTimes)
                          select DateTime.SpecifyKind(time, DateTimeKind.Utc)) {
        candidateTimeToOrdinal[time] = ordinal;
        ordinalToCandidateTime[ordinal] = time;
        ++ordinal;
      }
      var timesVotes = from vote in votes
                       where vote.times.Any()
                       select from time in vote.times
                              select candidateTimeToOrdinal[time];
      var timeOrder = _electionHolder.DecideWinner(ordinal, timesVotes);
      return new VoteInformation {
        locations = from locationOrdinal in locationOrder
                    select ordinalToCandidateLocation[locationOrdinal],
        times = from timeOrdinal in timeOrder
                select ordinalToCandidateTime[timeOrdinal],
      };
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
      var user = await _context.Users.FindAsync(userId);
      var ev = await _context.Events.FindAsync(event_id);
      if(ev == null)
        return NotFound();
      var vote = await _context.Votes.FindAsync(event_id, userId);
      if(vote == null) {
        return new VoteInformation {
          locations = ev.CandidateGoogleMapsLocations
                        .OrderByDescending(l => _critic.PersonalScore(l, user))
                        .Select(l => new VoteInformation.LocationInformation {
                          google_maps_location_id = l.Id,
                        })
                        .Union(ev.CandidateCustomLocations
                        .OrderByDescending(l => _critic.PersonalScore(l, user))
                        .Select(l => new VoteInformation.LocationInformation {
                          custom_location_id = l.Id,
                        }))
                        .ToArray(),
          times = JsonSerializer.Deserialize<IEnumerable<DateTime>>(ev.CandidateTimes)
                  .Select(t => DateTime.SpecifyKind(t, DateTimeKind.Utc))
                  .ToArray(),
        };
      }
      var ret = CompleteVote(
        JsonSerializer.Deserialize<VoteInformation>(vote.Order),
        ev,
        user
      );
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
      var vote = await _context.Votes.FindAsync(body.event_id, userId);
      var deserializedVote = vote == null
        ? new VoteInformation {
          locations = Enumerable.Empty<VoteInformation.LocationInformation>(),
          times = Enumerable.Empty<DateTime>(),
        }
        : JsonSerializer.Deserialize<VoteInformation>(vote.Order);
      if(body.vote_information.times != null) {
        body.vote_information.times =
          body.vote_information.times
          .Select(t => DateTime.SpecifyKind(t, DateTimeKind.Utc))
          .Distinct();
        var candidateTimes = JsonSerializer.Deserialize<IEnumerable<DateTime>>(ev.CandidateTimes);
        candidateTimes = candidateTimes.Select(t => DateTime.SpecifyKind(t, DateTimeKind.Utc));
        var timesDiff = body.vote_information.times.Except(candidateTimes);
        if(timesDiff.Count() > 0)
          return NotFound(timesDiff);
        deserializedVote.times = body.vote_information.times;
      }
      if(body.vote_information.locations != null) {
        if(body.vote_information.locations
        .Where(l => l.custom_location_id != null)
        .Any(l => !ev.CandidateCustomLocations.Any(c => c.Id == l.custom_location_id))) {
          return NotFound();
        }
        if(body.vote_information.locations
          .Where(l => l.google_maps_location_id != null)
          .Any(l => !ev.CandidateGoogleMapsLocations.Any(c => c.Id == l.google_maps_location_id))) {
          return NotFound();
        }
        body.vote_information.locations =
          body.vote_information.locations
            .Where(l => l.custom_location_id != null ^ l.google_maps_location_id != null)
            .Distinct();
        deserializedVote.locations = body.vote_information.locations;
      }
      if(vote == null) {
        vote = new() {
          EventId = body.event_id,
          Order = JsonSerializer.Serialize(deserializedVote),
          UserId = userId,
        };
        await _context.Votes.AddAsync(vote);
      } else {
        vote.Order = JsonSerializer.Serialize(deserializedVote);
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
    private VoteInformation CompleteVoteIfNotEmpty(
      VoteInformation incomplete,
      Event ev,
      User user
    ) {
      var ret = CompleteVote(incomplete, ev, user);
      if(!incomplete.locations.Any())
        ret.locations = incomplete.locations;
      if(!incomplete.times.Any())
        ret.times = incomplete.times;
      return ret;
    }
    private VoteInformation CompleteVote(
      VoteInformation incomplete,
      Event ev,
      User user
    ) {
      var candidateTimes = JsonSerializer.Deserialize<IEnumerable<DateTime>>(ev.CandidateTimes);
      var ret = new VoteInformation {
        locations = incomplete.locations
        .Where(l => {
          if(l.custom_location_id != null)
            return ev.CandidateCustomLocations.Any(c => c.Id == l.custom_location_id);
          if(l.google_maps_location_id != null)
            return ev.CandidateGoogleMapsLocations.Any(c => c.Id == l.google_maps_location_id);
          return false;
        })
        .Union(ev.CandidateCustomLocations
        .OrderByDescending(l => _critic.PersonalScore(l, user))
        .Select(l =>
          new VoteInformation.LocationInformation {
            custom_location_id = l.Id
          })
        )
        .Union(ev.CandidateGoogleMapsLocations
        .OrderByDescending(l => _critic.PersonalScore(l, user))
        .Select(l =>
          new VoteInformation.LocationInformation {
            google_maps_location_id = l.Id
          })
        ),
        times = incomplete.times
        .Union(candidateTimes)
        .Select(t => DateTime.SpecifyKind(t, DateTimeKind.Utc))
      };
      return ret;
    }
    private readonly IUserManager _userManager;
    private readonly LMDbContext _context;
    private readonly ILocationCritic _critic;
    private readonly IElectionHolder _electionHolder;
    private readonly ILogger<VotesController> _logger;
  }
}