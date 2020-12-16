using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LetsMeatAPI {
  public class UserManager {
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
          Prefs = "{}",
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
        return id;
      } catch(ApplicationException ex) {
        _logger.LogError(ex.ToString());
        return null;
      } finally {
        _mtx.ReleaseReaderLock();
      }
    }
    public string? LogOut(string token) {
      try {
        _mtx.AcquireWriterLock(_mtxTimeout);
        _loggedUsersTokenToId.TryGetValue(token, out var id);
        if(id == null)
          return null;
        _loggedUsersTokenToId.Remove(token);
        _loggedUsersIdToToken.Remove(id);
        return id;
      } catch(ApplicationException ex) {
        _logger.LogError(ex.ToString());
        return null;
      } finally {
        _mtx.ReleaseWriterLock();
      }
    }
    public static void Init(IEnumerable<(string token, string id)> grantedTokens) {
      try {
        _mtx.AcquireWriterLock(_mtxTimeout);
        foreach(var (token, id) in grantedTokens) {
          _loggedUsersIdToToken[id] = token;
          _loggedUsersTokenToId[token] = id;
        }
      } finally {
        _mtx.ReleaseWriterLock();
      }
    }
    private void logUser(string token, string id) {
      try {
        _mtx.AcquireWriterLock(_mtxTimeout);
        _loggedUsersIdToToken.TryGetValue(id, out var oldToken);
        if(oldToken != null)
          _loggedUsersTokenToId.Remove(oldToken);
        _loggedUsersIdToToken[id] = token;
        _loggedUsersTokenToId[token] = id;
      } catch(ApplicationException ex) {
        _logger.LogError(ex.ToString());
      } finally {
        _mtx.ReleaseWriterLock();
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
