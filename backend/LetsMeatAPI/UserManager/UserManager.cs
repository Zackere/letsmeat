using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace LetsMeatAPI {
  public class UserManager {
    public UserManager(LMDbContext context, ILogger<UserManager> logger) {
      _context = context;
      _logger = logger;
    }
    public void OnTokenGranted(string token, GoogleJsonWebSignature.Payload jwt) {
      var userEntry = _context.Users.Find(jwt.Subject);
      if(userEntry == null) {
        _context.Users.Add(new() {
          Id = jwt.Subject,
          PictureUrl = jwt.Picture,
          Email = jwt.Email,
          Name = jwt.Name,
          Prefs = "{}"
        });
      } else {
        userEntry.Email = jwt.Email;
        userEntry.PictureUrl = jwt.Picture;
        userEntry.Name = jwt.Name;
        _context.Users.Update(userEntry);
      }
      _context.SaveChanges();
      logUser(token, jwt.Subject);
    }
    public string? IsLoggedIn(string token) {
      _loggedUsersTokenToId.TryGetValue(token, out var id);
      return id;
    }
    public bool LogOut(string token) {
      var id = IsLoggedIn(token);
      if(id == null)
        return false;
      _loggedUsersTokenToId.Remove(token);
      _loggedUsersIdToToken.Remove(token);
      return true;
    }
    private void logUser(string token, string id) {
      _loggedUsersIdToToken.TryGetValue(id, out var oldToken);
      if(oldToken != null)
        _loggedUsersTokenToId.Remove(oldToken);
      _loggedUsersIdToToken[id] = token;
      _loggedUsersTokenToId[token] = id;
    }
    private readonly LMDbContext _context;
    private readonly ILogger<UserManager> _logger;
    private readonly static Dictionary<string, string> _loggedUsersIdToToken = new();
    private readonly static Dictionary<string, string> _loggedUsersTokenToId = new();
  }
}
