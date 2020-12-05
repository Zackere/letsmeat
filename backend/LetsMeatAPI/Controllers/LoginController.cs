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
      UserManager userManager,
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
        var tokenBytes = new byte[TokenLength];
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
        return StatusCode(StatusCodes.Status500InternalServerError);
      } catch(Exception ex) {
        _logger.LogError(ex.ToString());
        return Unauthorized();
      }
    }
    public uint TokenLength { get; set; } = 128;
    private readonly GoogleTokenIdValidator _googleTokenIdValidator;
    private readonly UserManager _userManager;
    private readonly IEnumerable<string>? _expectedGoogleAudiences;
    private readonly Random _rnd;
    private readonly ILogger<LoginController> _logger;
  }
}
