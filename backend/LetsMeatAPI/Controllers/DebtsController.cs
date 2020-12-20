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
  public class DebtsController : ControllerBase {
    public DebtsController(
      UserManager userManager,
      LMDbContext context,
      ILogger<DebtsController> logger
    ) {
      _userManager = userManager;
      _context = context;
      _logger = logger;
    }
    public class DebtAddBody {
      public Guid group_id { get; set; }
      public string from_id { get; set; }
      public string to_id { get; set; }
      public int amount { get; set; }
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
      var grp = await _context.Groups.FindAsync(body.group_id);
      if(grp == null)
        return NotFound();
      if(!grp!.Users.Any(u => u.Id == userId))
        return Forbid();
      if(!grp!.Users.Any(u => u.Id == body.from_id) || !grp!.Users.Any(u => u.Id == body.to_id))
        return Forbid();
      if(body.to_id == body.from_id)
        return new StatusCodeResult(418);
      if(body.from_id.CompareTo(body.to_id) < 0)
        (body.from_id, body.to_id, body.amount) = (body.to_id, body.from_id, -body.amount);
      var debt = await _context.Debts.FindAsync(body.from_id, body.to_id, body.group_id);
      if(debt == null) {
        await _context.Debts.AddAsync(new Debt() {
          Amount = body.amount,
          FromId = body.from_id,
          GroupId = body.group_id,
          ToId = body.to_id,
        });
      } else {
        debt.Amount += body.amount;
        _context.Entry(debt).State = EntityState.Modified;
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
      return Ok();
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
    private readonly UserManager _userManager;
    private readonly LMDbContext _context;
    private readonly ILogger<DebtsController> _logger;
  }
}
