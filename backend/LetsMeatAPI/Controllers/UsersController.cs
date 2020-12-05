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
    public class UsersInfoBody {
      public List<string> ids { get; set; }
    }
    [HttpPost]
    [Route("info")]
    public async Task<ActionResult<IEnumerable<UserInformationResponse>>> Info(
      string token,
      [FromBody] UsersInfoBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      body.ids.Sort();
      var ret = await (from user in _context.Users
                       where body.ids.BinarySearch(user.Id) >= 0
                       select new UserInformationResponse {
                         id = user.Id,
                         picture_url = user.PictureUrl,
                         name = user.Name
                       }).ToListAsync();
      return ret;
    }
    [HttpGet]
    [Route("info")]
    public async Task<ActionResult<UserInformationResponse>> Info(
      string token
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var user = await _context.Users.FindAsync(userId);
      return new UserInformationResponse {
        id = user.Id,
        picture_url = user.PictureUrl,
        email = user.Email,
        name = user.Name,
        groups = from grp in user.Groups
                 select new UserInformationResponse.GroupInformation {
                   id = grp.Id,
                   name = grp.Name
                 }
      };
    }
    [HttpGet]
    [Route("search")]
    public async Task<ActionResult<IEnumerable<UserInformationResponse>>> Search(
      string token,
      string name
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      return await (from user in _context.Users
                    where user.Name.Contains(name) &&
                              user.Id != userId
                    select new UserInformationResponse() {
                      id = user.Id,
                      picture_url = user.PictureUrl,
                      name = user.Name
                    }).ToListAsync();
    }
    private readonly UserManager _userManager;
    private readonly LMDbContext _context;
    private readonly ILogger<UsersController> _logger;
  }
}
