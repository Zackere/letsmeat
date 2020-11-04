using Google.Apis.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
    public delegate void TokenGrantedEventHandler(string token, object userInformation);
    public LoginController(
      GoogleTokenIdValidator googleTokenIdValidator,
      IEnumerable<string> expectedGoogleAudiences,
      IWebHostEnvironment webHostEnvironment,
      Random rnd
    ) {
      _googleTokenIdValidator = googleTokenIdValidator ?? throw new ArgumentNullException("googleTokenIdValidator");
      _expectedGoogleAudiences = expectedGoogleAudiences;
      _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException("webHostEnvironment");
      _rnd = rnd ?? throw new ArgumentNullException("rnd");
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
    public async Task<IActionResult> Google(string googleTokenId) {
      try {
        var googlePayload = await _googleTokenIdValidator(
          googleTokenId,
          new GoogleJsonWebSignature.ValidationSettings() {
            Audience = _expectedGoogleAudiences,
          }
        );
        var tokenBytes = new byte[TokenLength];
        _rnd.NextBytes(tokenBytes);
        var tokenHexString = string.Concat(Array.ConvertAll(tokenBytes, b => b.ToString("X2")));
        OnTokenGranted?.Invoke(tokenHexString, googlePayload);
        return Ok(tokenHexString);
      } catch {
        return Unauthorized();
      }
    }
    public uint TokenLength { get; set; } = 128;
    public event TokenGrantedEventHandler OnTokenGranted;
    private readonly GoogleTokenIdValidator _googleTokenIdValidator;
    private readonly IEnumerable<string> _expectedGoogleAudiences;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly Random _rnd;
  }
}
