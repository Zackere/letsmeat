using LetsMeatAPI;
using LetsMeatAPI.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;
using Xunit.Abstractions;

namespace LetsMeatAPITests {
  public class PaidResourceGuardTest : TestBase {
    public PaidResourceGuardTest(ITestOutputHelper output) : base(output) { }
    [Fact]
    public void UsersWithNoTokenCannotAccessPaidResources() {
      PaidResouceGuard.ResetCounterForTesting();
      var user = CreateUsers(5042, 1)[0];
      user.Token = null;
      var guard = new PaidResouceGuard(Mock.Of<ILogger<PaidResouceGuard>>());
      Assert.False(guard.CanAccessPaidResource(user));
    }
    [Fact]
    public void UsersWithFakeTokenCannotAccessPaidResources() {
      PaidResouceGuard.ResetCounterForTesting();
      var user = CreateUsers(50420, 1)[0];
      var fakeToken = LoginController.FakeTokenPrefix + RandomString(new Random(5678), LoginController.TokenLength - LoginController.FakeTokenPrefix.Length);
      user.Token = fakeToken;
      var guard = new PaidResouceGuard(Mock.Of<ILogger<PaidResouceGuard>>());
      Assert.False(guard.CanAccessPaidResource(user));
    }
    [Fact]
    public void AccessToPaidResurcesIsLimited() {
      PaidResouceGuard.ResetCounterForTesting();
      var user = CreateUsers(50421, 1)[0];
      var guard = new PaidResouceGuard(Mock.Of<ILogger<PaidResouceGuard>>());
      for(var i = 0; i < PaidResouceGuard.MaxAccesses; ++i)
        Assert.True(guard.CanAccessPaidResource(user));
      Assert.False(guard.CanAccessPaidResource(user));
      PaidResouceGuard.DecayCounter();
      Assert.True(guard.CanAccessPaidResource(user));
      Assert.False(guard.CanAccessPaidResource(user));
    }
    [Fact]
    public void DecayingCounterHasNoEffectWhenCounterIsEmpty() {
      PaidResouceGuard.ResetCounterForTesting();
      var user = CreateUsers(50421, 1)[0];
      var guard = new PaidResouceGuard(Mock.Of<ILogger<PaidResouceGuard>>());
      for(var i = 0; i < 100; ++i)
        PaidResouceGuard.DecayCounter();
      for(var i = 0; i < PaidResouceGuard.MaxAccesses; ++i)
        Assert.True(guard.CanAccessPaidResource(user));
      Assert.False(guard.CanAccessPaidResource(user));
      PaidResouceGuard.DecayCounter();
      Assert.True(guard.CanAccessPaidResource(user));
      Assert.False(guard.CanAccessPaidResource(user));
    }
  }
}
