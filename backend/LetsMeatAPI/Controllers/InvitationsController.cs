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
                    select new InvitationsGetResponse() {
                      from_id = inv.FromId,
                      group_id = inv.GroupId
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
      if(!grp.Users.Contains(await _context.Users.FindAsync(userId)))
        return Unauthorized();
      if(grp.Users.Contains(await _context.Users.FindAsync(body.to_id)))
        return Conflict();
      var inv = await _context.Invitations.FindAsync(new object[] {
        body.to_id,
        body.group_id
      });
      if(inv != null)
        return Conflict();
      await _context.Invitations.AddAsync(new Models.Invitation() {
        FromId = userId,
        GroupId = body.group_id,
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
    private readonly UserManager _userManager;
    private readonly LMDbContext _context;
    private readonly ILogger<InvitationsController> _logger;
  }
}
