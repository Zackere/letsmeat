using Microsoft.AspNetCore.Http;
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
  public class UsersController : ControllerBase {
    public UsersController(
      IUserManager userManager,
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
        public string owner_id { get; set; }
      }
      public class Prefs {
        [Range(0, 100)]
        public int taste { get; set; }
        [Range(0, 100)]
        public int price { get; set; }
        [Range(0, 100)]
        public int amount_of_food { get; set; }
        [Range(0, 100)]
        public int waiting_time { get; set; }
      }
      public string id { get; set; }
      public string picture_url { get; set; }
      public string email { get; set; }
      public string name { get; set; }
      public Prefs? prefs { get; set; }
      public IEnumerable<GroupInformation>? groups { get; set; }
    }
    public class UsersInfoBody {
      public string[] ids { get; set; }
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
      return Ok(from user in _context.Users
                where body.ids.Contains(user.Id)
                select new UserInformationResponse {
                  id = user.Id,
                  picture_url = user.PictureUrl,
                  name = user.Name,
                  prefs = new() {
                    amount_of_food = user.AmountOfFoodPref,
                    price = user.PricePref,
                    taste = user.TastePref,
                    waiting_time = user.WaitingTimePref,
                  },
                });
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
                   name = grp.Name,
                   owner_id = grp.OwnerId,
                 },
        prefs = new() {
          amount_of_food = user.AmountOfFoodPref,
          price = user.PricePref,
          taste = user.TastePref,
          waiting_time = user.WaitingTimePref,
        },
      };
    }
    [HttpGet]
    [Route("search")]
    public async Task<ActionResult<IEnumerable<UserInformationResponse>>> Search(
      string token,
      [MinLength(3)] string name
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      return Ok(from user in _context.Users
                where user.Name.Contains(name) &&
                      user.Id != userId
                select new UserInformationResponse {
                  id = user.Id,
                  picture_url = user.PictureUrl,
                  name = user.Name,
                  prefs = new() {
                    amount_of_food = user.AmountOfFoodPref,
                    price = user.PricePref,
                    taste = user.TastePref,
                    waiting_time = user.WaitingTimePref,
                  },
                });
    }
    public class UserUpdatePrefsBody {
      [Range(0, 100)]
      public int? taste { get; set; }
      [Range(0, 100)]
      public int? price { get; set; }
      [Range(0, 100)]
      public int? amount_of_food { get; set; }
      [Range(0, 100)]
      public int? waiting_time { get; set; }
    }
    [HttpPost]
    [Route("update_prefs")]
    public async Task<ActionResult> UpdatePrefs(
      string token,
      [FromBody] UserUpdatePrefsBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      if(
        body.taste == null &&
        body.price == null &&
        body.amount_of_food == null &&
        body.waiting_time == null
      ) {
        return new StatusCodeResult(StatusCodes.Status418ImATeapot);
      }
      var user = await _context.Users.FindAsync(userId);
      if(body.taste != null)
        user.TastePref = (int)body.taste;
      if(body.price != null)
        user.PricePref = (int)body.price;
      if(body.amount_of_food != null)
        user.AmountOfFoodPref = (int)body.amount_of_food;
      if(body.waiting_time != null)
        user.WaitingTimePref = (int)body.waiting_time;
      if(user.AmountOfFoodPref == user.PricePref &&
        user.PricePref == user.TastePref &&
        user.TastePref == user.WaitingTimePref) {
        user.AmountOfFoodPref = user.PricePref = user.TastePref = user.WaitingTimePref = 100;
      } else {
        var p = new[] { user.AmountOfFoodPref, user.PricePref, user.TastePref, user.WaitingTimePref }
                .Select(p => p == 0 ? 0 : 100.0 / p).Min();
        user.AmountOfFoodPref = (int)(p * user.AmountOfFoodPref);
        user.PricePref = (int)(p * user.PricePref);
        user.TastePref = (int)(p * user.TastePref);
        user.WaitingTimePref = (int)(p * user.WaitingTimePref);
      }
      _context.Entry(user).State = EntityState.Modified;
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateConcurrencyException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return Ok();
    }
    private readonly IUserManager _userManager;
    private readonly LMDbContext _context;
    private readonly ILogger<UsersController> _logger;
  }
}
