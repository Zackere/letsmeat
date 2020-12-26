using LetsMeatAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
      public uint amount { get; set; }
      [MaxLength(250)]
      public string description { get; set; }
      public Guid? image_id { get; set; }
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
      if(!(await _context.Users.AnyAsync(u => u.Id == body.from_id) &&
           await _context.Users.AnyAsync(u => u.Id == body.to_id) &&
           (body.image_id == null || await _context.Images.AnyAsync(i => i.Id == (Guid)body.image_id))
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
      var debt = new PendingDebt {
        Amount = body.amount,
        Description = body.description,
        EventId = body.event_id,
        FromId = body.from_id,
        GroupId = (Guid)body.group_id!,
        ImageId = body.image_id,
        Timestamp = DateTime.UtcNow,
        ToId = body.to_id,
      };
      await _context.PendingDebts.AddAsync(debt);
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      await _debtReducer.ReduceDebts((Guid)body.group_id);
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
                        where debt.ToId == userId
                        select new DebtPendingResponse.PendingDebtInformation {
                          amount = debt.Amount,
                          description = debt.Description,
                          event_id = debt.EventId,
                          from_id = debt.FromId,
                          group_id = debt.GroupId,
                          id = debt.Id,
                          image_id = debt.ImageId,
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
        return Forbid();
      var debts = from debt in _context.Debts
                  where debt.GroupId == id
                  select new { debt.Amount, debt.FromId, debt.ToId };
      var ret = new DebtGroupInformationResponse() { debts = new() };
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
        return Forbid();
      if(
        !pendingDebt.Group.Users.Any(u => u.Id == pendingDebt.FromId) ||
        !pendingDebt.Group.Users.Any(u => u.Id == pendingDebt.ToId)
      ) {
        return Forbid();
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
