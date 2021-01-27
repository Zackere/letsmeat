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
using System.Threading.Tasks;

namespace LetsMeatAPI.Controllers {
  [Route("[controller]")]
  [ApiController]
  public class DebtsController : ControllerBase {
    public DebtsController(
      IUserManager userManager,
      LMDbContext context,
      IDebtReducer debtReducer,
      ILogger<DebtsController> logger
    ) {
      _userManager = userManager;
      _context = context;
      _debtReducer = debtReducer;
      _logger = logger;
    }
    public enum DebtType {
      FromEvent,
      Transfer,
    }
    public class DebtAddBody {
      public Guid? group_id { get; set; }
      public Guid? event_id { get; set; }
      public string from_id { get; set; }
      public string to_id { get; set; }
      public uint? amount { get; set; }
      [MaxLength(250)]
      public string? description { get; set; }
      public Guid? image_debt_id { get; set; }
      public DebtType debt_type { get; set; } = DebtType.FromEvent;
    }
    [HttpPost]
    [Route("add")]
    public async Task<ActionResult> Add(
      string token,
      [FromBody] DebtAddBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId is null)
        return Unauthorized();
      if((body.group_id, body.event_id) is (null, null))
        return new StatusCodeResult(StatusCodes.Status418ImATeapot);
      if(body.from_id == body.to_id)
        return new StatusCodeResult(StatusCodes.Status418ImATeapot);
      if((body.amount, body.image_debt_id) is (null, null))
        return new StatusCodeResult(StatusCodes.Status418ImATeapot);
      if((body.description, body.image_debt_id) is (null, null))
        return new StatusCodeResult(StatusCodes.Status418ImATeapot);
      var debtHistoryEntry = new DebtHistory {
        HistoryEntryCreatedOn = DateTime.UtcNow,
        FromId = body.from_id,
        ToId = body.to_id,
        Timestamp = DateTime.UtcNow,
        Type = (int)body.debt_type,
      };
      if(body.event_id is not null) {
        var ev = await _context.Events.FindAsync(body.event_id);
        if(ev is null)
          return NotFound();
        body.group_id = ev.GroupId;
        debtHistoryEntry.EventId = ev.Id;
      }
      var grp = await (from g in _context.Groups.Include(g => g.Users)
                       where g.Id == body.group_id
                       select g).SingleOrDefaultAsync();
      if(grp is null)
        return NotFound();
      debtHistoryEntry.GroupId = grp.Id;
      if(!(body.image_debt_id is null ||
           await _context.DebtsFromImages.AnyAsync(
              i => i.Id == (Guid)body.image_debt_id && i.Image.GroupId == body.group_id)
      )) {
        return NotFound();
      }
      if(
        !grp.Users.Any(u => u.Id == body.from_id) ||
        !grp.Users.Any(u => u.Id == body.to_id)
      ) {
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      }
      var imageDebt = body.image_debt_id is null
                      ? null
                      : await _context.DebtsFromImages.FindAsync(body.image_debt_id);
      if(imageDebt != null && imageDebt.Satisfied)
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);

      debtHistoryEntry.Amount = imageDebt?.Amount ?? (uint)body.amount!;
      debtHistoryEntry.Description = imageDebt?.Description ?? body.description!;
      debtHistoryEntry.ImageId = imageDebt?.ImageId;

      if(imageDebt != null && imageDebt.Bound != null) {
        debtHistoryEntry.PendingDebtId = imageDebt.Bound.PendingDebtId;
        debtHistoryEntry.Timestamp = imageDebt.Bound.PendingDebt.Timestamp;
        _context.PendingDebts.Remove(imageDebt.Bound.PendingDebt);
        _context.PendingDebtFromImageBounds.Remove(imageDebt.Bound);
      }
      if(userId == body.from_id) {
        await ApplyDebtToDb(
          body.from_id,
          body.to_id,
          (Guid)body.group_id!,
          (int)(imageDebt?.Amount ?? (uint)body.amount!),
          imageDebt,
          debtHistoryEntry
        );
        await _debtReducer.ReduceDebts((Guid)body.group_id!);
        try {
          await _context.SaveChangesAsync();
        } catch(Exception ex)
            when(ex is DbUpdateConcurrencyException ||
                 ex is DbUpdateException
          ) {
          _logger.LogError(ex.ToString());
          return Conflict();
        }
        return Ok();
      }
      var debt = new PendingDebt {
        Amount = imageDebt?.Amount ?? (uint)body.amount!,
        Description = imageDebt?.Description ?? body.description!,
        EventId = body.event_id,
        FromId = body.from_id,
        GroupId = (Guid)body.group_id!,
        ImageId = imageDebt?.ImageId,
        Timestamp = DateTime.UtcNow,
        ToId = body.to_id,
        Type = (int)body.debt_type,
      };
      if(imageDebt is not null) {
        var bound = new PendingDebtFromImageBound {
          DebtFromImage = imageDebt,
          PendingDebt = debt,
        };
        debt.Bound = imageDebt.Bound = bound;
        await _context.PendingDebtFromImageBounds.AddAsync(bound);
      }
      await _context.PendingDebts.AddAsync(debt);
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return Ok();
    }
    public class DebtPendingResponse {
      public class PendingDebtInformation {
        public Guid id { get; set; }
        public Guid group_id { get; set; }
        public Guid? event_id { get; set; }
        public string from_id { get; set; }
        public string to_id { get; set; }
        public uint amount { get; set; }
        public string description { get; set; }
        public Guid? image_id { get; set; }
        public DateTime timestamp { get; set; }
        public Guid? image_debt_id { get; set; }
        public DebtType debt_type { get; set; }
      }
      public IEnumerable<PendingDebtInformation> pending_debts { get; set; }
    }
    [HttpGet]
    [Route("pending")]
    public async Task<ActionResult<DebtPendingResponse>> Pending(
      string token
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId is null)
        return Unauthorized();
      return new DebtPendingResponse {
        pending_debts = from debt in _context.PendingDebts
                        where debt.FromId == userId
                        orderby debt.Timestamp descending
                        select new DebtPendingResponse.PendingDebtInformation {
                          amount = debt.Amount,
                          debt_type = (DebtType)debt.Type,
                          description = debt.Description,
                          event_id = debt.EventId,
                          from_id = debt.FromId,
                          group_id = debt.GroupId,
                          id = debt.Id,
                          image_debt_id = debt.Bound == null ? null : debt.Bound.DebtFromImageId,
                          image_id = debt.ImageId,
                          timestamp = DateTime.SpecifyKind(debt.Timestamp, DateTimeKind.Utc),
                          to_id = debt.ToId,
                        },
      };
    }
    public class DebtGroupInformationResponse {
      public Dictionary<string, Dictionary<string, int>> debts { get; set; }
    }
    [HttpGet]
    [Route("groupinfo")]
    public async Task<ActionResult<DebtGroupInformationResponse>> GroupInfo(
      string token,
      Guid id,
      bool normalize = false
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId is null)
        return Unauthorized();
      var grp = await _context.Groups.FindAsync(id);
      if(grp is null)
        return NotFound();
      if(!grp.Users.Any(u => u.Id == userId))
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      var debts = from debt in _context.Debts
                  where debt.GroupId == id && debt.Amount != 0
                  select new { debt.Amount, debt.FromId, debt.ToId };
      var ret = new DebtGroupInformationResponse { debts = new() };
      if(normalize) {
        foreach(var debt in debts) {
          if(debt.Amount < 0) {
            if(!ret.debts.ContainsKey(debt.ToId))
              ret.debts[debt.ToId] = new();
            ret.debts[debt.ToId][debt.FromId] = -debt.Amount;
          } else {
            if(!ret.debts.ContainsKey(debt.FromId))
              ret.debts[debt.FromId] = new();
            ret.debts[debt.FromId][debt.ToId] = debt.Amount;
          }
        }
        return ret;
      }
      foreach(var debt in debts) {
        if(!ret.debts.ContainsKey(debt.FromId))
          ret.debts[debt.FromId] = new();
        ret.debts[debt.FromId][debt.ToId] = debt.Amount;
      }
      return ret;
    }
    public class DebtApproveRejectBody {
      public Guid debt_id { get; set; }
    }
    [HttpPost]
    [Route("approve")]
    public async Task<ActionResult> Approve(
      string token,
      [FromBody] DebtApproveRejectBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId is null)
        return Unauthorized();
      var pendingDebt = await _context.PendingDebts.FindAsync(body.debt_id);
      if(pendingDebt is null)
        return NotFound();
      if(pendingDebt.FromId != userId)
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      await ApplyDebtToDb(
        pendingDebt.FromId,
        pendingDebt.ToId,
        pendingDebt.GroupId,
        (int)pendingDebt.Amount,
        pendingDebt.Bound?.DebtFromImage,
        new() {
          Amount = pendingDebt.Amount,
          Description = pendingDebt.Description,
          EventId = pendingDebt.EventId,
          FromId = pendingDebt.FromId,
          GroupId = pendingDebt.GroupId,
          HistoryEntryCreatedOn = DateTime.UtcNow,
          ImageDebtId = pendingDebt.Bound?.DebtFromImageId,
          ImageId = pendingDebt.ImageId,
          PendingDebtId = pendingDebt.Id,
          Timestamp = pendingDebt.Timestamp,
          ToId = pendingDebt.ToId,
          Type = pendingDebt.Type,
        }
      );
      _context.Remove(pendingDebt);
      try {
        await _context.SaveChangesAsync();
      } catch(Exception ex)
          when(ex is DbUpdateConcurrencyException ||
               ex is DbUpdateException
        ) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      await _debtReducer.ReduceDebts(pendingDebt.GroupId);
      return Ok();
    }
    [HttpPost]
    [Route("reject")]
    public async Task<ActionResult> Reject(
      string token,
      [FromBody] DebtApproveRejectBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId is null)
        return Unauthorized();
      var debt = await _context.PendingDebts.FindAsync(body.debt_id);
      if(debt is null)
        return NotFound();
      if(debt.FromId != userId && debt.ToId != userId)
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      _context.Remove(debt);
      await _context.SaveChangesAsync();
      return Ok();
    }
    private async Task ApplyDebtToDb(
      string fromId,
      string toId,
      Guid groupId,
      int amount,
      DebtFromImage? imageDebt,
      DebtHistory history
    ) {
      if(fromId.CompareTo(toId) < 0)
        (fromId, toId, amount) = (toId, fromId, -amount);
      var debt = await _context.Debts.FindAsync(fromId, toId, groupId);
      if(debt is null) {
        await _context.Debts.AddAsync(new() {
          Amount = amount,
          FromId = fromId,
          GroupId = groupId,
          ToId = toId,
        });
      } else {
        debt.Amount += amount;
        _context.Entry(debt).State = EntityState.Modified;
      }
      if(imageDebt is not null) {
        history.ImageDebtId = imageDebt.Id;
        imageDebt.Satisfied = true;
        _context.Entry(imageDebt).State = EntityState.Modified;
      }
      await _context.DebtHistory.AddAsync(history);
    }
    private readonly IUserManager _userManager;
    private readonly LMDbContext _context;
    private readonly IDebtReducer _debtReducer;
    private readonly ILogger<DebtsController> _logger;
  }
}
