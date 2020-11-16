using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsMeatAPI.Controllers {
  [Route("[controller]")]
  [ApiController]
  public class UsersController : ControllerBase {
    public UsersController(
      UserManager userManager,
      LMDbContext context,
      ILogger<UsersController> logger
    ) {
      _userManager = userManager;
      _context = context;
      _logger = logger;
    }
    public class UserInformationResponse {
      public class GroupInformation {
        public Guid id { get; set; }
        public string name { get; set; }
      }
      public string id { get; set; }
      public string picture_url { get; set; }
      public string email { get; set; }
      public string name { get; set; }
      public IEnumerable<GroupInformation>? groups { get; set; }
    }
    [HttpGet]
    [Route("info")]
    public async Task<ActionResult<UserInformationResponse>> Info(
      string token,
      string id
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var user = await _context.Users.FindAsync(id);
      if(user == null)
        return NotFound();
      if(id == user.Id) {
        return Ok(new UserInformationResponse() {
          id = user.Id,
          picture_url = user.PictureUrl,
          email = user.Email,
          name = user.Name,
          groups = from grp in user.Groups
                   select new UserInformationResponse.GroupInformation() {
                     id = grp.Id,
                     name = grp.Name
                   }
        });
      }

      return Ok(new UserInformationResponse {
        id = user.Id,
        picture_url = user.PictureUrl,
        email = user.Email,
        name = user.Name,
      });
    }
    private readonly UserManager _userManager;
    private readonly LMDbContext _context;
    private readonly ILogger<UsersController> _logger;
  }
}
