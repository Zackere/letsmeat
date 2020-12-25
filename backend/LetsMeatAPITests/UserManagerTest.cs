using Google.Apis.Auth;
using LetsMeatAPI;
using LetsMeatAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LetsMeatAPITests {
  public class UserManagerTest : TestBase {
    public UserManagerTest(ITestOutputHelper output) : base(output) { }
    [Theory]
    [MemberData(nameof(UsersWithTokens), 35712, 1)]
    [MemberData(nameof(UsersWithTokens), 32167, 5)]
    public async Task RegistersUsersOnTokenGrantedForTheFirstLoginEver(
      string[] tokens,
      GoogleJsonWebSignature.Payload[] jwts
    ) {
      var connection = GetDb();
      using var context = CreateContextForConnection(connection);
      var userManager = new UserManager(context, Mock.Of<ILogger<UserManager>>());
      await Task.WhenAll(tokens.Zip(jwts).Select(p => userManager.OnTokenGranted(p.First, p.Second)).ToArray());

      Assert.Equal(tokens.Count(), context.Users.Count());

      foreach(var user in context.Users) {
        var (token, jwt) = (from p in tokens.Zip(jwts)
                            where p.Second.Subject == user.Id
                            select p).FirstOrDefault();
        Assert.NotNull(token);
        Assert.NotNull(jwt);
        VerifyUserInformation(user, jwt);
        Assert.Equal(jwt.Subject, userManager.IsLoggedIn(token));
      }
    }
    [Fact]
    public async Task ProvidesInformationAboutLoggedInUsers() {
      var data = UsersWithTokens(123321, 1).First();
      var token = (data[0] as object[])[0] as string;
      var jwt = (data[1] as object[])[0] as GoogleJsonWebSignature.Payload;
      var connection = GetDb();
      using var context = CreateContextForConnection(connection);
      var userManager = new UserManager(context, Mock.Of<ILogger<UserManager>>());

      await userManager.OnTokenGranted(token, jwt);
      Assert.Equal(jwt.Subject, userManager.IsLoggedIn(token));
      Assert.Equal(jwt.Subject, userManager.LogOut(token));
      Assert.Null(userManager.LogOut(token));
      token += "dsjiadjsaoi";
      await userManager.OnTokenGranted(token, jwt);
      Assert.Equal(jwt.Subject, userManager.IsLoggedIn(token));
      Assert.Equal(jwt.Subject, userManager.LogOut(token));
      Assert.Null(userManager.LogOut(token));
    }
    [Fact]
    public async Task UpdatesUserInformationUponLogin() {
      var data = UsersWithTokens(123321, 2).First();
      var token = (data[0] as object[])[0] as string;
      var jwt = (data[1] as object[])[0] as GoogleJsonWebSignature.Payload;
      var token2 = (data[0] as object[])[1] as string;
      var jwt2 = (data[1] as object[])[1] as GoogleJsonWebSignature.Payload;
      jwt2.Subject = jwt.Subject; // They are the same user
      var connection = GetDb();
      using var context = CreateContextForConnection(connection);

      var userManager = new UserManager(
        context,
        Mock.Of<ILogger<UserManager>>()
      );
      await userManager.OnTokenGranted(token, jwt);

      Assert.Equal(1, context.Users.Count());
      var user = context.Users.First();
      VerifyUserInformation(user, jwt);
      ++user.PricePref;
      context.Users.Update(user);
      await context.SaveChangesAsync();

      await userManager.OnTokenGranted(token2, jwt2);

      Assert.Equal(1, context.Users.Count());
      user = context.Users.First();
      VerifyUserInformation(user, jwt2);
      Assert.Equal(jwt.Subject, userManager.IsLoggedIn(token2));
      Assert.Null(userManager.IsLoggedIn(token));
    }
    [Fact]
    public async Task ThrowsDbUpdateExceptionOnRealDb() {
      var data = UsersWithTokens(123321, 2).First();
      var token1 = (data[0] as object[])[0] as string;
      var jwt1 = (data[1] as object[])[0] as GoogleJsonWebSignature.Payload;
      var token2 = (data[0] as object[])[1] as string;
      var jwt2 = (data[1] as object[])[1] as GoogleJsonWebSignature.Payload;
      jwt2.Subject = jwt1.Subject; // Force conflict
      var connection = GetDb();
      using var context1 = CreateContextForConnection(connection);
      using var context2 = CreateContextForConnection(connection);
      var genuineUsers = context2.Users;
      var mockUsers = new Mock<DbSet<User>>(MockBehavior.Strict);
      mockUsers.Setup(u => u.FindAsync(It.IsAny<object[]>()))
        .Returns<object[]>(async key => {
          Assert.Equal(new[] { jwt1.Subject }, key);
          var ret = await genuineUsers.FindAsync(key);
          Assert.Null(ret);
          await Task.Delay(1500);
          return ret;
        });
      mockUsers.Setup(u => u.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
        .Returns<User, CancellationToken>((u, t) => genuineUsers.AddAsync(u, t));

      var userManager1 = new UserManager(context1, Mock.Of<ILogger<UserManager>>());
      var userManager2 = new UserManager(context2, Mock.Of<ILogger<UserManager>>());
      context2.Users = mockUsers.Object;
      var create2 = Assert.ThrowsAsync<DbUpdateException>(() => userManager2.OnTokenGranted(token2, jwt2));
      await Task.Delay(500);
      await userManager1.OnTokenGranted(token1, jwt1);
      await create2;

      Assert.Equal(1, genuineUsers.Count());
      var user = genuineUsers.First();
      VerifyUserInformation(user, jwt2);
    }
    [Fact]
    public async Task ThrowsDbUpdateConcurrencyExceptionOnRealDb() {
      var data = UsersWithTokens(123321, 1).First();
      var token1 = (data[0] as object[])[0] as string;
      var jwt1 = (data[1] as object[])[0] as GoogleJsonWebSignature.Payload;
      var connection = GetDb();
      using var context1 = CreateContextForConnection(connection);
      using var context2 = CreateContextForConnection(connection);
      context2.Users.Add(new User {
        Id = jwt1.Subject,
        PictureUrl = jwt1.Picture,
        Email = jwt1.Email,
        Name = jwt1.Name,
        PricePref = 12,
        AmountOfFoodPref = 31,
        TastePref = 56,
        WaitingTimePref = 89,
      });
      await context2.SaveChangesAsync();
      var genuineUsers = context1.Users;
      var mockUsers = new Mock<DbSet<User>>(MockBehavior.Strict);
      mockUsers.Setup(u => u.FindAsync(It.IsAny<object[]>()))
        .Returns<object[]>(async key => {
          Assert.Equal(new[] { jwt1.Subject }, key);
          var ret = await genuineUsers.FindAsync(key);
          Assert.NotNull(ret);
          await Task.Delay(1500);
          return ret;
        });
      context1.Users = mockUsers.Object;
      var userManager1 = new UserManager(context1, Mock.Of<ILogger<UserManager>>());

      var update1 = Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => userManager1.OnTokenGranted(token1, jwt1));
      await Task.Delay(500);
      context2.Users.Remove(context2.Users.Find(jwt1.Subject));
      await context2.SaveChangesAsync();
      await update1;

      Assert.Equal(0, genuineUsers.Count());
    }
    private void VerifyUserInformation(
      User user,
      GoogleJsonWebSignature.Payload jwt
    ) {
      Assert.Equal(jwt.Subject, user.Id);
      Assert.Equal(jwt.Picture, user.PictureUrl);
      Assert.Equal(jwt.Email, user.Email);
      Assert.Equal(jwt.Name, user.Name);
    }
  }
}
