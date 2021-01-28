using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
    public void LogIn(string token, string id);
  }
  public class UserManager : IUserManager {
    public UserManager(
      IServiceProvider serviceProvider,
      ILogger<UserManager> logger
    ) {
      _serviceProvider = serviceProvider;
      _logger = logger;
    }
    public async Task OnTokenGranted(string token, GoogleJsonWebSignature.Payload jwt) {
      if(_loggedUsersTokenToId.ContainsKey(token))
        return;
      using var scope = _serviceProvider.CreateScope();
      using var context = scope.ServiceProvider.GetRequiredService<LMDbContext>();
      var user = await context.Users.FindAsync(jwt.Subject);
      if(user == null) {
        await context.Users.AddAsync(new() {
          Id = jwt.Subject,
          PictureUrl = jwt.Picture,
          Email = jwt.Email,
          Name = jwt.Name,
          AmountOfFoodPref = 100,
          PricePref = 100,
          TastePref = 100,
          WaitingTimePref = 100,
          Token = token,
        });
      } else {
        user.PictureUrl = jwt.Picture;
        user.Email = jwt.Email;
        user.Name = jwt.Name;
        user.Token = token;
        context.Entry(user).State = EntityState.Modified;
      }
      await context.SaveChangesAsync();
      LogIn(token, jwt.Subject);
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
    public void LogIn(string token, string id) {
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
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UserManager> _logger;
    private readonly Dictionary<string, string> _loggedUsersIdToToken = new();
    private readonly Dictionary<string, string> _loggedUsersTokenToId = new();
    private readonly ReaderWriterLock _mtx = new();
    private const int _mtxTimeout = 500;
  }
}
