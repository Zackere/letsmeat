using LetsMeatAPI.Controllers;
using LetsMeatAPI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LetsMeatAPI {
  public interface IPaidResourceGuard {
    public Task<bool> CanAccessPaidResource(User user);
  }
  public class PaidResouceGuard : IPaidResourceGuard {
    public PaidResouceGuard(
      int delay,
      ILogger<PaidResouceGuard> logger
    ) {
      _delay = delay;
      _logger = logger;
    }
    public Task<bool> CanAccessPaidResource(User user) {
      return Task.Run(() => {
        if(
          user.Token == null ||
          user.Token.StartsWith(LoginController.FakeTokenPrefix)
        ) {
          _logger.LogInformation($"Request DENIED to access paid resource for {user.Name}");
          return false;
        }
        try {
          _mtx.AcquireWriterLock(4 * _delay);
          var ret = _counter < MaxAccesses;
          if(ret)
            ++_counter;
          Thread.Sleep(_delay);
          _logger.LogInformation($"Request to paid reasource {(ret ? "" : "NOT ")}granted. Counter: {_counter}");
          _mtx.ReleaseWriterLock();
          return ret;
        } catch(ApplicationException ex) {
          _logger.LogError(ex.ToString());
          return false;
        }
      });
    }
    public static void DecayCounter(int delay) {
      try {
        _mtx.AcquireWriterLock(delay);
        if(_counter > 0)
          --_counter;
        _mtx.ReleaseWriterLock();
      } catch(ApplicationException) { }
    }
    public static void ResetCounterForTesting() {
      _counter = 0;
    }
    private readonly int _delay;
    private readonly ILogger<PaidResouceGuard> _logger;
    private static readonly ReaderWriterLock _mtx = new();
    private static uint _counter = 0;
    public const uint MaxAccesses = 50;
  }
}
