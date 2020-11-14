using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LetsMeatAPI {
  public class UserManager {
    public UserManager(LMDbContext context, ILogger<UserManager> logger) {
      _context = context;
      _logger = logger;
    }
    public void OnTokenGranted(string token, GoogleJsonWebSignature.Payload jwt) {
      var user = _context.Users.Find(jwt.Subject) ?? new();
      user.Id = jwt.Subject;
      user.PictureUrl = jwt.Picture;
      user.Email = jwt.Email;
      user.Name = jwt.Name;
      switch(_context.Entry(user).State) {
      case EntityState.Added:
      case EntityState.Detached:
        try {
          _context.Add(user);
        } catch(Exception ex) {
          _logger.LogError(ex.ToString());
        }
        break;
      case EntityState.Modified:
        _context.Update(user);
        break;
      case EntityState.Unchanged:
        break;
      default:
        throw new ArgumentOutOfRangeException();
      }
      try {
        _context.SaveChanges();
      } catch(DbUpdateConcurrencyException ex) {
        _logger.LogError(ex.ToString());
        ex.Entries.Single().Reload();
      } catch(Exception ex) {
        _logger.LogError(ex.ToString());
        return;
      }
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
    public bool LogOut(string token) {
      var id = IsLoggedIn(token);
      if(id == null)
        return false;
      try {
        _mtx.AcquireWriterLock(_mtxTimeout);
        _loggedUsersTokenToId.Remove(token);
        _loggedUsersIdToToken.Remove(id);
        return true;
      } catch(ApplicationException ex) {
        _logger.LogError(ex.ToString());
        return false;
      } finally {
        _mtx.ReleaseWriterLock();
      }
    }
    private void logUser(string token, string id) {
      try {
        _mtx.AcquireReaderLock(_mtxTimeout);
        _loggedUsersIdToToken.TryGetValue(id, out var oldToken);
        _mtx.UpgradeToWriterLock(_mtxTimeout);
        if(oldToken != null)
          _loggedUsersTokenToId.Remove(oldToken);
        _loggedUsersIdToToken[id] = token;
        _loggedUsersTokenToId[token] = id;
      } catch(ApplicationException ex) {
        _logger.LogError(ex.ToString());
        return;
      } finally {
        _mtx.ReleaseLock();
      }
    }
    private readonly LMDbContext _context;
    private readonly ILogger<UserManager> _logger;
    private readonly static Dictionary<string, string> _loggedUsersIdToToken = new();
    private readonly static Dictionary<string, string> _loggedUsersTokenToId = new();
    private readonly static ReaderWriterLock _mtx = new();
    private readonly static int _mtxTimeout = 1000;
  }
}
