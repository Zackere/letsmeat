using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LetsMeatAPI.Controllers {
  [Route("[controller]")]
  [ApiController]
  public class LoginController : ControllerBase {
    public delegate Task<GoogleJsonWebSignature.Payload>
      GoogleTokenIdValidator(
        string jwt,
        GoogleJsonWebSignature.ValidationSettings validationSettings
      );
    public class GoogleAudiences {
      public IEnumerable<string>? Auds { get; set; }
    }
    public LoginController(
      GoogleTokenIdValidator googleTokenIdValidator,
      IUserManager userManager,
      GoogleAudiences? expectedGoogleAudiences,
      Random rnd,
      ILogger<LoginController> logger
    ) {
      _googleTokenIdValidator = googleTokenIdValidator ?? throw new ArgumentNullException("googleTokenIdValidator");
      _userManager = userManager ?? throw new ArgumentNullException("userManager");
      _expectedGoogleAudiences = expectedGoogleAudiences?.Auds;
      _rnd = rnd ?? throw new ArgumentNullException("rnd");
      _logger = logger ?? throw new ArgumentNullException("logger");
    }
    /// <summary>
    /// Authorizes sender using Google OAuth 2.0
    /// </summary>
    /// <param name="googleTokenId">Google JWT</param>
    /// <returns>A token which should be used to authorize requests</returns>
    /// <response code="200">Returns token string which should be used to authorize requests</response>
    [HttpPost]
    [Route("google")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<string>> Google(string googleTokenId) {
      try {
        var googlePayload = await _googleTokenIdValidator(
          googleTokenId,
          new GoogleJsonWebSignature.ValidationSettings() {
            Audience = _expectedGoogleAudiences,
          }
        );
        using var hasher = SHA256.Create();
        var hashBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(googlePayload.Subject));
        googlePayload.Subject = string.Concat(Array.ConvertAll(hashBytes, b => b.ToString("X2")));
        var tokenBytes = new byte[TokenLength / 2];
        _rnd.NextBytes(tokenBytes);
        var tokenHexString = string.Concat(Array.ConvertAll(tokenBytes, b => b.ToString("X2")));
        try {
          await _userManager.OnTokenGranted(tokenHexString, googlePayload);
        } catch(Exception ex)
          when(ex is DbUpdateConcurrencyException ||
               ex is DbUpdateException
        ) {
          _logger.LogError(ex.ToString());
          return Conflict();
        }
        if(_userManager.IsLoggedIn(tokenHexString) == googlePayload.Subject)
          return Ok(tokenHexString);
        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
      } catch(Exception ex) {
        _logger.LogError(ex.ToString());
        return Unauthorized();
      }
    }
    public class FakeUserInfo {
      public string id { get; set; }
      public string picture_url { get; set; }
      public string email { get; set; }
      public string name { get; set; }
      public string token { get; set; }
    }
    [HttpPost]
    [Route("fake")]
    public async Task<ActionResult<FakeUserInfo>> Fake([FromBody] FakeUserInfo body) {
      using var hasher = SHA256.Create();
      var hashIdBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes("fake-id" + body.id));
      body.id = string.Concat(Array.ConvertAll(hashIdBytes, b => b.ToString("X2")));
      var hashTokenBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(body.token));
      body.token = string.Concat(Array.ConvertAll(hashTokenBytes, b => b.ToString("X2")));
      body.token += body.token;
      body.token = $"{FakeTokenPrefix}{body.token[FakeTokenPrefix.Length..]}";
      if(_userManager.IsLoggedIn(body.token) != null)
        return Conflict(body);
      var jwt = new GoogleJsonWebSignature.Payload {
        Subject = body.id,
        Picture = $"{Environment.GetEnvironmentVariable("HOST_PATH")}/user-profile.png",
        Email = body.email,
        Name = body.name
      };
      try {
        await _userManager.OnTokenGranted(body.token, jwt);
      } catch(Exception ex)
        when(ex is DbUpdateConcurrencyException ||
             ex is DbUpdateException
      ) {
        _logger.LogError(ex.ToString());
        return Conflict(body);
      }
      return body;
    }
    public const string FakeTokenPrefix = "fake-token-";
    public const int TokenLength = 128;
    private readonly GoogleTokenIdValidator _googleTokenIdValidator;
    private readonly IUserManager _userManager;
    private readonly IEnumerable<string>? _expectedGoogleAudiences;
    private readonly Random _rnd;
    private readonly ILogger<LoginController> _logger;
  }
}
