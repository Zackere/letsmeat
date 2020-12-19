using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsMeatAPI.Controllers {
  [Route("[controller]")]
  public class InvitationsController : ControllerBase {
    public InvitationsController(
      UserManager userManager,
      LMDbContext context,
      ILogger<InvitationsController> logger
    ) {
      _userManager = userManager;
      _context = context;
      _logger = logger;
    }
    public class InvitationsGetResponse {
      public string from_id { get; set; }
      public Guid group_id { get; set; }
      public DateTime sent { get; set; }
    }
    [HttpGet]
    [Route("get")]
    public async Task<ActionResult<IEnumerable<InvitationsGetResponse>>> Get(
      string token
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      return await (from inv in _context.Invitations
                    where inv.ToId == userId
                    orderby inv.Sent descending
                    select new InvitationsGetResponse() {
                      from_id = inv.FromId,
                      group_id = inv.GroupId,
                      sent = inv.Sent,
                    }).ToListAsync();
    }
    public class InvitationsSendBody {
      public string to_id { get; set; }
      public Guid group_id { get; set; }
    }
    [HttpPost]
    [Route("send")]
    public async Task<ActionResult> Send(
      string token,
      [FromBody] InvitationsSendBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var grp = await _context.Groups.FindAsync(body.group_id);
      if(grp == null)
        return NotFound();
      if(!grp.Users.Any(u => u.Id == userId))
        return Forbid();
      if(grp.Users.Any(u => u.Id == body.to_id))
        return Conflict();
      var inv = await _context.Invitations.FindAsync(
        body.to_id,
        body.group_id
      );
      if(inv != null)
        return Conflict();
      await _context.Invitations.AddAsync(new Models.Invitation() {
        FromId = userId,
        GroupId = body.group_id,
        Sent = DateTime.UtcNow,
        ToId = body.to_id
      });
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return Ok();
    }
    public class InvitationRejectBody {
      public Guid group_id { get; set; }
    }
    [HttpDelete]
    [Route("reject")]
    public async Task<ActionResult> Reject(
      string token,
      [FromBody] InvitationRejectBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var inv = await _context.Invitations.FindAsync(userId, body.group_id);
      if(inv == null)
        return NotFound();
      _context.Invitations.Remove(inv);
      await _context.SaveChangesAsync();
      return Ok();
    }
    private readonly UserManager _userManager;
    private readonly LMDbContext _context;
    private readonly ILogger<InvitationsController> _logger;
  }
}
