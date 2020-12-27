using LetsMeatAPI;
using LetsMeatAPI.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LetsMeatAPITests {
  public class PaidResourceGuardTest : TestBase {
    public PaidResourceGuardTest(ITestOutputHelper output) : base(output) { }
    [Fact]
    public async Task UsersWithNoTokenCannotAccessPaidResources() {
      PaidResouceGuard.ResetCounterForTesting();
      var user = CreateUsers(5042, 1)[0];
      user.Token = null;
      var guard = new PaidResouceGuard(10, Mock.Of<ILogger<PaidResouceGuard>>());
      Assert.False(await guard.CanAccessPaidResource(user));
    }
    [Fact]
    public async Task UsersWithFakeTokenCannotAccessPaidResources() {
      PaidResouceGuard.ResetCounterForTesting();
      var user = CreateUsers(50420, 1)[0];
      var fakeToken = LoginController.FakeTokenPrefix + RandomString(new Random(5678), LoginController.TokenLength - LoginController.FakeTokenPrefix.Length);
      user.Token = fakeToken;
      var guard = new PaidResouceGuard(10, Mock.Of<ILogger<PaidResouceGuard>>());
      Assert.False(await guard.CanAccessPaidResource(user));
    }
    [Fact]
    public async Task AccessToPaidResurcesIsLimited() {
      PaidResouceGuard.ResetCounterForTesting();
      var user = CreateUsers(50421, 1)[0];
      var guard = new PaidResouceGuard(10, Mock.Of<ILogger<PaidResouceGuard>>());
      for(var i = 0; i < PaidResouceGuard.MaxAccesses; ++i)
        Assert.True(await guard.CanAccessPaidResource(user));
      Assert.False(await guard.CanAccessPaidResource(user));
      PaidResouceGuard.DecayCounter(10);
      Assert.True(await guard.CanAccessPaidResource(user));
      Assert.False(await guard.CanAccessPaidResource(user));
    }
    [Fact]
    public async Task DecayingCounterHasNoEffectWhenCounterIsEmpty() {
      PaidResouceGuard.ResetCounterForTesting();
      var user = CreateUsers(50421, 1)[0];
      var guard = new PaidResouceGuard(10, Mock.Of<ILogger<PaidResouceGuard>>());
      for(var i = 0; i < 100; ++i)
        PaidResouceGuard.DecayCounter(10);
      for(var i = 0; i < PaidResouceGuard.MaxAccesses; ++i)
        Assert.True(await guard.CanAccessPaidResource(user));
      Assert.False(await guard.CanAccessPaidResource(user));
      PaidResouceGuard.DecayCounter(10);
      Assert.True(await guard.CanAccessPaidResource(user));
      Assert.False(await guard.CanAccessPaidResource(user));
    }
    [Fact]
    public async Task QueryingForResourcesIsSubjectToADelay() {
      PaidResouceGuard.ResetCounterForTesting();
      var user = CreateUsers(50421, 1)[0];
      var guard = new PaidResouceGuard(1500, Mock.Of<ILogger<PaidResouceGuard>>());
      var sw = new Stopwatch();
      sw.Start();
      var res = await guard.CanAccessPaidResource(user);
      sw.Stop();
      Assert.True(res);
      Assert.True(sw.ElapsedMilliseconds >= 1500);
    }
    [Fact]
    public async Task QueryingForResourcesIsQueued() {
      PaidResouceGuard.ResetCounterForTesting();
      var user = CreateUsers(50421, 1)[0];
      var guard = new PaidResouceGuard(1500, Mock.Of<ILogger<PaidResouceGuard>>());
      var sw = new Stopwatch();
      sw.Start();
      var res = await Task.WhenAll(guard.CanAccessPaidResource(user), guard.CanAccessPaidResource(user));
      sw.Stop();
      Assert.True(res[0]);
      Assert.True(res[1]);
      Assert.True(sw.ElapsedMilliseconds >= 2 * 1500);
    }
  }
}
