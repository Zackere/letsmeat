using LetsMeatAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace LetsMeatAPI.Controllers {
  [Route("[controller]")]
  [ApiController]
  public class EventsController : ControllerBase {
    public EventsController(
      IUserManager userManager,
      LMDbContext context,
      ILogger<EventsController> logger
    ) {
      _userManager = userManager;
      _context = context;
      _logger = logger;
    }
    public class EventCreateBody {
      public Guid group_id { get; set; }
      [MaxLength(64)]
      public string name { get; set; }
      public DateTime deadline { get; set; }
    }
    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<EventInformationResponse>> Create(
      string token,
      [FromBody] EventCreateBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId is null)
        return Unauthorized();
      var grp = await (from g in _context.Groups.Include(g => g.Users)
                       where g.Id == body.group_id
                       select g).SingleOrDefaultAsync();
      if(grp is null)
        return NotFound();
      if(!grp.Users.Any(u => u.Id == userId))
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      var ev = new Event {
        CandidateTimes = "[]",
        CreatorId = userId,
        Deadline = body.deadline.ToUniversalTime(),
        GroupId = grp.Id,
        Name = body.name,
      };
      await _context.Events.AddAsync(ev);
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return new EventInformationResponse {
        candidate_custom_locations = Enumerable.Empty<Guid>(),
        candidate_google_maps_locations = Enumerable.Empty<string>(),
        candidate_times = Enumerable.Empty<DateTime>(),
        creator_id = ev.CreatorId,
        deadline = ev.Deadline,
        group_id = ev.GroupId,
        id = ev.Id,
        images = Enumerable.Empty<Guid>(),
        name = ev.Name,
      };
    }
    public class EventUpdateBody {
      public Guid id { get; set; }
      public string? name { get; set; }
      public DateTime? deadline { get; set; }
      public IEnumerable<Guid>? custom_locations_ids { get; set; }
      public IEnumerable<string>? google_maps_locations_ids { get; set; }
      public IEnumerable<DateTime>? candidate_times { get; set; }
    }
    [HttpPatch]
    [Route("update")]
    public async Task<ActionResult<EventInformationResponse>> Update(
      string token,
      [FromBody] EventUpdateBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId is null)
        return Unauthorized();
      if((body.name,
          body.deadline,
          body.candidate_times,
          body.custom_locations_ids,
          body.google_maps_locations_ids)
        is (null, null, null, null, null)
      ) {
        return new StatusCodeResult(StatusCodes.Status418ImATeapot);
      }
      var ev = await _context.Events.FindAsync(body.id);
      if(ev is null)
        return NotFound();
      if(
        (body.name is not null || body.deadline is not null) &&
        userId != ev.CreatorId
      ) {
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      }
      if(DateTime.SpecifyKind(ev.Deadline, DateTimeKind.Utc) < DateTime.UtcNow)
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      if(body.name is not null)
        ev.Name = body.name;
      if(body.deadline is not null)
        ev.Deadline = ((DateTime)body.deadline).ToUniversalTime();
      if(body.custom_locations_ids is not null) {
        var uids = body.custom_locations_ids.Distinct().ToArray();
        var ids = await _context.CustomLocations.Select(l => l.Id).ToListAsync();
        if(!uids.All(id => ids.Contains(id)))
          return NotFound();
        var locs = from l in _context.CustomLocations
                   where uids.Contains(l.Id) &&
                         !ev.CandidateCustomLocations.Contains(l)
                   select l;
        foreach(var l in locs)
          ev.CandidateCustomLocations.Add(l);
      }
      if(body.google_maps_locations_ids is not null) {
        var uids = body.google_maps_locations_ids.Distinct().ToArray();
        var ids = await _context.GoogleMapsLocations.Select(l => l.Id).ToListAsync();
        if(!uids.All(id => ids.Contains(id)))
          return NotFound();
        var locs = from l in _context.GoogleMapsLocations
                   where uids.Contains(l.Id) &&
                         !ev.CandidateGoogleMapsLocations.Contains(l)
                   select l;
        foreach(var l in locs)
          ev.CandidateGoogleMapsLocations.Add(l);
      }
      if(body.candidate_times is not null) {
        var arr = JsonSerializer.Deserialize<IEnumerable<DateTime>>(ev.CandidateTimes)
                  .Select(t => DateTime.SpecifyKind(t, DateTimeKind.Utc));
        ev.CandidateTimes = JsonSerializer.Serialize(
          arr.Union(body.candidate_times.Select(t => t.ToUniversalTime()))
        );
      }
      _context.Entry(ev).State = EntityState.Modified;
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateConcurrencyException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return new EventInformationResponse {
        candidate_custom_locations = from location in ev.CandidateCustomLocations
                                     select location.Id,
        candidate_google_maps_locations = from location in ev.CandidateGoogleMapsLocations
                                          select location.Id,
        candidate_times = JsonSerializer.Deserialize<IEnumerable<DateTime>>(ev.CandidateTimes)
                          .Select(t => DateTime.SpecifyKind(t, DateTimeKind.Utc)).ToArray(),
        creator_id = ev.CreatorId,
        deadline = DateTime.SpecifyKind(ev.Deadline, DateTimeKind.Utc),
        group_id = ev.GroupId,
        id = ev.Id,
        images = from image in ev.Images
                 select image.Id,
        name = ev.Name,
      };
    }
    public class EventInformationResponse {
      public Guid id { get; set; }
      public Guid group_id { get; set; }
      public string creator_id { get; set; }
      public string name { get; set; }
      public IEnumerable<DateTime> candidate_times { get; set; }
      public IEnumerable<Guid> candidate_custom_locations { get; set; }
      public IEnumerable<string> candidate_google_maps_locations { get; set; }
      public DateTime deadline { get; set; }
      public IEnumerable<Guid> images { get; set; }
    }
    [HttpGet]
    [Route("info")]
    public async Task<ActionResult<EventInformationResponse>> Info(
      string token,
      Guid id
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId is null)
        return Unauthorized();
      var ev = await _context.Events.FindAsync(id);
      if(ev is null)
        return NotFound();
      if(!ev.Group.Users.Any(u => u.Id == userId))
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      return new EventInformationResponse {
        candidate_custom_locations = from location in ev.CandidateCustomLocations
                                     select location.Id,
        candidate_google_maps_locations = from location in ev.CandidateGoogleMapsLocations
                                          select location.Id,
        candidate_times = JsonSerializer.Deserialize<IEnumerable<DateTime>>(ev.CandidateTimes)
                          .Select(t => DateTime.SpecifyKind(t, DateTimeKind.Utc)).ToArray(),
        creator_id = ev.CreatorId,
        deadline = DateTime.SpecifyKind(ev.Deadline, DateTimeKind.Utc),
        group_id = ev.GroupId,
        images = from image in ev.Images
                 select image.Id,
        id = ev.Id,
        name = ev.Name,
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
      if(userId is null)
        return Unauthorized();
      var ev = await _context.Events.FindAsync(body.id);
      if(ev is null)
        return NotFound();
      if(ev.CreatorId != userId)
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      _context.Votes.RemoveRange(ev.Votes);
      _context.Events.Remove(ev);
      await _context.SaveChangesAsync();
      return Ok();
    }
    public class EventsImageDebtsResult {
      public class PendingDebtInformation {
        public Guid? id { get; set; }
        public Guid group_id { get; set; }
        public Guid? event_id { get; set; }
        public string from_id { get; set; }
        public string to_id { get; set; }
        public uint amount { get; set; }
        public string description { get; set; }
        public Guid? image_id { get; set; }
        public DateTime timestamp { get; set; }
        public DateTime? approved_on { get; set; }
        public Guid? image_debt_id { get; set; }
      }
      public class DebtFromImageInformation {
        public Guid id { get; set; }
        public uint amount { get; set; }
        [MaxLength(128)]
        public string description { get; set; }
        public bool satisfied { get; set; }
        public Guid event_id { get; set; }
        public Guid image_id { get; set; }
        public string image_uploaded_by_id { get; set; }
        public PendingDebtInformation? pending_debt { get; set; }
      }
      public IEnumerable<DebtFromImageInformation> image_debts { get; set; }
    }
    [HttpGet]
    [Route("image_debts")]
    public async Task<ActionResult<EventsImageDebtsResult>> ImageDebts(
      string token,
      Guid event_id
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId is null)
        return Unauthorized();
      var ev = await _context.Events.FindAsync(event_id);
      if(ev is null)
        return NotFound();
      return new EventsImageDebtsResult {
        image_debts = from d in _context.DebtsFromImages
                      where d.Image.EventId == event_id
                      orderby d.Image.UploadTime descending
                      select new EventsImageDebtsResult.DebtFromImageInformation {
                        amount = d.Amount,
                        description = d.Description,
                        event_id = (Guid)d.Image.EventId!,
                        id = d.Id,
                        image_id = d.ImageId,
                        image_uploaded_by_id = d.Image.UploadedById,
                        satisfied = d.Satisfied,
                        pending_debt = d.Bound == null
                        ? (from h in _context.DebtHistory
                           where h.ImageDebtId == d.Id
                           select new EventsImageDebtsResult.PendingDebtInformation {
                             amount = h.Amount,
                             approved_on = h.HistoryEntryCreatedOn,
                             description = h.Description,
                             event_id = h.EventId,
                             from_id = h.FromId,
                             group_id = h.GroupId,
                             image_debt_id = h.ImageDebtId,
                             image_id = h.ImageId,
                             timestamp = h.Timestamp,
                             to_id = h.ToId,
                           }).SingleOrDefault()
                        : new EventsImageDebtsResult.PendingDebtInformation {
                          amount = d.Bound.PendingDebt.Amount,
                          description = d.Bound.PendingDebt.Description,
                          event_id = d.Bound.PendingDebt.EventId,
                          from_id = d.Bound.PendingDebt.FromId,
                          group_id = d.Bound.PendingDebt.GroupId,
                          id = d.Bound.PendingDebt.Id,
                          image_debt_id = d.Id,
                          image_id = d.Bound.PendingDebt.ImageId,
                          timestamp = d.Bound.PendingDebt.Timestamp,
                          to_id = d.Bound.PendingDebt.ToId,
                        }
                      }
      };
    }
    private readonly IUserManager _userManager;
    private readonly LMDbContext _context;
    private readonly ILogger<EventsController> _logger;
  }
}
