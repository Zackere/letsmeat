using LetsMeatAPI.Models;
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
    public class DebtAddBody {
      public Guid? group_id { get; set; }
      public Guid? event_id { get; set; }
      public string from_id { get; set; }
      public string to_id { get; set; }
      public uint? amount { get; set; }
      [MaxLength(250)]
      public string? description { get; set; }
      public Guid? image_debt_id { get; set; }
    }
    [HttpPost]
    [Route("add")]
    public async Task<ActionResult> Add(
      string token,
      [FromBody] DebtAddBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      if(body.group_id == null && body.event_id == null)
        return new StatusCodeResult(418);
      if(body.from_id == body.to_id)
        return new StatusCodeResult(418);
      if((body.amount, body.image_debt_id) is (null, null))
        return new StatusCodeResult(418);
      if((body.description, body.image_debt_id) is (null, null))
        return new StatusCodeResult(418);
      if(!(await _context.Users.AnyAsync(u => u.Id == body.from_id) &&
           await _context.Users.AnyAsync(u => u.Id == body.to_id) &&
           (body.image_debt_id == null ||
           await _context.DebtsFromImages.AnyAsync(i => i.Id == (Guid)body.image_debt_id))
        )) {
        return NotFound();
      }
      if(body.event_id != null) {
        var ev = await _context.Events.FindAsync(body.event_id);
        if(ev == null)
          return NotFound();
        body.group_id = ev.GroupId;
      } else if(!await _context.Groups.AnyAsync(g => g.Id == body.group_id)) {
        return NotFound();
      }
      var imageDebt = body.image_debt_id == null
                      ? null
                      : await _context.DebtsFromImages.FindAsync(body.image_debt_id);
      if(userId == body.from_id) {
        var switchSign = body.from_id.CompareTo(body.to_id) < 0;
        if(switchSign)
          (body.from_id, body.to_id) = (body.to_id, body.from_id);
        var debt = await _context.Debts.FindAsync(body.from_id, body.to_id, body.group_id);
        if(debt == null) {
          await _context.Debts.AddAsync(new() {
            Amount = (switchSign ? -1 : 1) * (int)(imageDebt?.Amount ?? (uint)body.amount!),
            FromId = body.from_id,
            GroupId = (Guid)body.group_id!,
            ToId = body.to_id,
          });
        } else {
          debt.Amount += (switchSign ? -1 : 1) * (int)(imageDebt?.Amount ?? (uint)body.amount!);
          _context.Entry(debt).State = EntityState.Modified;
        }
        if(imageDebt != null) {
          imageDebt.Satisfied = true;
          _context.Entry(imageDebt).State = EntityState.Modified;
        }
        try {
          await _context.SaveChangesAsync();
        } catch(Exception ex)
            when(ex is DbUpdateConcurrencyException ||
                 ex is DbUpdateException
          ) {
          _logger.LogError(ex.ToString());
          return Conflict();
        }
        await _debtReducer.ReduceDebts((Guid)body.group_id!);
        return Ok();
      } else {
        var debt = new PendingDebt {
          Amount = imageDebt?.Amount ?? (uint)body.amount!,
          Description = imageDebt?.Description ?? body.description!,
          EventId = body.event_id,
          FromId = body.from_id,
          GroupId = (Guid)body.group_id!,
          ImageId = imageDebt?.ImageId,
          Timestamp = DateTime.UtcNow,
          ToId = body.to_id,
        };
        if(imageDebt != null) {
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
      }
      public IEnumerable<PendingDebtInformation> pending_debts { get; set; }
    }
    [HttpGet]
    [Route("pending")]
    public async Task<ActionResult<DebtPendingResponse>> Pending(
      string token
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      return new DebtPendingResponse {
        pending_debts = from debt in _context.PendingDebts
                        where debt.FromId == userId
                        orderby debt.Timestamp descending
                        select new DebtPendingResponse.PendingDebtInformation {
                          amount = debt.Amount,
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
      Guid id
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var grp = await _context.Groups.FindAsync(id);
      if(grp == null)
        return NotFound();
      if(!grp.Users.Any(u => u.Id == userId))
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      var debts = from debt in _context.Debts
                  where debt.GroupId == id
                  select new { debt.Amount, debt.FromId, debt.ToId };
      var ret = new DebtGroupInformationResponse { debts = new() };
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
      if(userId == null)
        return Unauthorized();
      var pendingDebt = await _context.PendingDebts.FindAsync(body.debt_id);
      if(pendingDebt == null)
        return NotFound();
      if(!pendingDebt.Group.Users.Any(u => u.Id == userId))
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      if(
        !pendingDebt.Group.Users.Any(u => u.Id == pendingDebt.FromId) ||
        !pendingDebt.Group.Users.Any(u => u.Id == pendingDebt.ToId)
      ) {
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      }
      var switchSign = pendingDebt.FromId.CompareTo(pendingDebt.ToId) < 0;
      if(switchSign)
        (pendingDebt.FromId, pendingDebt.ToId) = (pendingDebt.ToId, pendingDebt.FromId);
      var debt = await _context.Debts.FindAsync(pendingDebt.FromId, pendingDebt.ToId, pendingDebt.GroupId);
      if(debt == null) {
        await _context.Debts.AddAsync(new() {
          Amount = (switchSign ? -1 : 1) * (int)pendingDebt.Amount,
          FromId = pendingDebt.FromId,
          GroupId = pendingDebt.GroupId,
          ToId = pendingDebt.ToId,
        });
      } else {
        debt.Amount += (switchSign ? -1 : 1) * (int)pendingDebt.Amount;
        _context.Entry(debt).State = EntityState.Modified;
      }
      if(pendingDebt.Bound != null) {
        pendingDebt.Bound.DebtFromImage.Satisfied = true;
        _context.Entry(pendingDebt.Bound.DebtFromImage).State = EntityState.Modified;
      }
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
      if(userId == null)
        return Unauthorized();
      var debt = await _context.PendingDebts.FindAsync(body.debt_id);
      if(debt == null)
        return NotFound();
      _context.Remove(debt);
      await _context.SaveChangesAsync();
      return Ok();
    }
    private readonly IUserManager _userManager;
    private readonly LMDbContext _context;
    private readonly IDebtReducer _debtReducer;
    private readonly ILogger<DebtsController> _logger;
  }
}
