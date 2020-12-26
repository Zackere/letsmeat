using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LetsMeatAPI {
  public interface IUserManager {
    public Task OnTokenGranted(string token, GoogleJsonWebSignature.Payload jwt);
    public string? IsLoggedIn(string token);
    public string? LogOut(string token);
  }
  public class UserManager : IUserManager {
    public UserManager(LMDbContext context, ILogger<UserManager> logger) {
      _context = context;
      _logger = logger;
    }
    public async Task OnTokenGranted(string token, GoogleJsonWebSignature.Payload jwt) {
      var user = await _context.Users.FindAsync(jwt.Subject);
      if(user == null) {
        await _context.Users.AddAsync(new() {
          Id = jwt.Subject,
          PictureUrl = jwt.Picture,
          Email = jwt.Email,
          Name = jwt.Name,
          AmountOfFoodPref = 50,
          PricePref = 50,
          TastePref = 50,
          WaitingTimePref = 50,
          Token = token,
        });
      } else {
        user.PictureUrl = jwt.Picture;
        user.Email = jwt.Email;
        user.Name = jwt.Name;
        user.Token = token;
        _context.Entry(user).State = EntityState.Modified;
      }
      await _context.SaveChangesAsync();
      logUser(token, jwt.Subject);
    }
    public string? IsLoggedIn(string token) {
      try {
        _mtx.AcquireReaderLock(_mtxTimeout);
        _loggedUsersTokenToId.TryGetValue(token, out var id);
        _mtx.ReleaseReaderLock();
        return id;
      } catch(ApplicationException ex) {
        _logger.LogError(ex.ToString());
        return null;
      }
    }
    public string? LogOut(string token) {
      try {
        _mtx.AcquireWriterLock(_mtxTimeout);
        _loggedUsersTokenToId.TryGetValue(token, out var id);
        if(id != null) {
          _loggedUsersTokenToId.Remove(token);
          _loggedUsersIdToToken.Remove(id);
        }
        _mtx.ReleaseWriterLock();
        return id;
      } catch(ApplicationException ex) {
        _logger.LogError(ex.ToString());
        return null;
      }
    }
    public static void Init(IEnumerable<(string token, string id)> grantedTokens) {
      try {
        _mtx.AcquireWriterLock(_mtxTimeout);
        foreach(var (token, id) in grantedTokens) {
          _loggedUsersIdToToken[id] = token;
          _loggedUsersTokenToId[token] = id;
        }
        _mtx.ReleaseWriterLock();
      } catch(ApplicationException) { }
    }
    private void logUser(string token, string id) {
      try {
        _mtx.AcquireWriterLock(_mtxTimeout);
        _loggedUsersIdToToken.TryGetValue(id, out var oldToken);
        if(oldToken != null)
          _loggedUsersTokenToId.Remove(oldToken);
        _loggedUsersIdToToken[id] = token;
        _loggedUsersTokenToId[token] = id;
        _mtx.ReleaseWriterLock();
      } catch(ApplicationException ex) {
        _logger.LogError(ex.ToString());
      }
    }
    private readonly LMDbContext _context;
    private readonly ILogger<UserManager> _logger;
    private static readonly Dictionary<string, string> _loggedUsersIdToToken = new();
    private static readonly Dictionary<string, string> _loggedUsersTokenToId = new();
    private static readonly ReaderWriterLock _mtx = new();
    private const int _mtxTimeout = 500;
  }
}
