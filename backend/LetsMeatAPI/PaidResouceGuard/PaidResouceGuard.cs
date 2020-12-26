using LetsMeatAPI.Controllers;
using LetsMeatAPI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace LetsMeatAPI {
  public interface IPaidResourceGuard {
    public bool CanAccessPaidResource(User user);
  }
  public class PaidResouceGuard : IPaidResourceGuard {
    public PaidResouceGuard(ILogger<PaidResouceGuard> logger) {
      _logger = logger;
    }
    public bool CanAccessPaidResource(User user) {
      if(
        user.Token == null ||
        user.Token.StartsWith(LoginController.FakeTokenPrefix)
      ) {
        return false;
      }
      try {
        _mtx.AcquireWriterLock(_mtxTimeout);
        var ret = false;
        if(_counter < MaxAccesses) {
          ++_counter;
          ret = true;
        }
        _mtx.ReleaseWriterLock();
        return ret;
      } catch(ApplicationException ex) {
        _logger.LogError(ex.ToString());
        return false;
      }
    }
    public static void DecayCounter() {
      try {
        _mtx.AcquireWriterLock(_mtxTimeout);
        if(_counter > 0)
          --_counter;
        _mtx.ReleaseWriterLock();
      } catch(ApplicationException) {
      }
    }
    public static void ResetCounterForTesting() {
      _counter = 0;
    }
    private readonly ILogger<PaidResouceGuard> _logger;
    private static readonly ReaderWriterLock _mtx = new();
    private const int _mtxTimeout = 500;
    private static uint _counter = 0;
    public const uint MaxAccesses = 50;
  }
}
