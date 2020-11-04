using Google.Apis.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
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
      if(googleTokenIdValidator == null)
        throw new ArgumentNullException("googleTokenIdValidator");
      if(webHostEnvironment == null)
        throw new ArgumentNullException("webHostEnvironment");
      if(rnd == null)
        throw new ArgumentNullException("rnd");
      _googleTokenIdValidator = googleTokenIdValidator;
      _webHostEnvironment = webHostEnvironment;
      _rnd = rnd;
    }
    [HttpPost]
    [Route("google")]
    public async Task<string> Google(string googleTokenId) {
      try {
        var googlePayload = await _googleTokenIdValidator(
          googleTokenId,
          new GoogleJsonWebSignature.ValidationSettings() {
            Audience = _expectedGoogleAudiences,
          }
        );
        var tokenBytes = new byte[TokenLength];
        _rnd.NextBytes(tokenBytes);
        var tokenHexString = String.Concat(Array.ConvertAll(tokenBytes, b => b.ToString("X2")));
        OnTokenGranted?.Invoke(tokenHexString, googlePayload);
        return tokenHexString;
      } catch(InvalidJwtException e) {
        if(_webHostEnvironment.IsDevelopment())
          throw e;
        Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        return "";
      }
    }
    public uint TokenLength { get; set; } = 128;
    public event TokenGrantedEventHandler OnTokenGranted;
    private readonly GoogleTokenIdValidator _googleTokenIdValidator;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IEnumerable<string> _expectedGoogleAudiences;
    private readonly Random _rnd;
  }
}
