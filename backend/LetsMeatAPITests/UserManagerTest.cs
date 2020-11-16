using Google.Apis.Auth;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LetsMeatAPITests {
  public class UserManagerTest {
    private readonly ITestOutputHelper _output;
    public UserManagerTest(ITestOutputHelper output) {
      _output = output;
    }
    private (LetsMeatAPI.LMDbContext context, SqliteConnection connection) GetDb() {
      var connectionStringBuilder = new SqliteConnectionStringBuilder() {
        DataSource = Path.GetRandomFileName()
      };
      var connection = new SqliteConnection(connectionStringBuilder.ToString());
      var options = new DbContextOptionsBuilder<LetsMeatAPI.LMDbContext>()
                        .UseSqlite(connection)
                        .LogTo(s => _output.WriteLine(s), LogLevel.Debug)
                        .EnableSensitiveDataLogging()
                        .Options;
      var context = new LetsMeatAPI.LMDbContext(options);
      context.Database.EnsureCreated();
      context.Database.EnsureDeleted();
      context.Database.EnsureCreated();
      return (context, connection);
    }
    [Theory]
    [MemberData(nameof(UsersWithTokens), 35712, 1)]
    [MemberData(nameof(UsersWithTokens), 32167, 5)]
    public async Task RegistersUsersOnTokenGrantedForTheFirstLoginEver(
      string[] tokens,
      GoogleJsonWebSignature.Payload[] jwts
    ) {
      var (context, connection) = GetDb();
      var userManager = new LetsMeatAPI.UserManager(context, Mock.Of<ILogger<LetsMeatAPI.UserManager>>());
      foreach(var (token, jwt) in tokens.Zip(jwts))
        await userManager.OnTokenGranted(token, jwt);

      Assert.Equal(tokens.Count(), context.Users.Count());

      foreach(var user in context.Users) {
        var (token, jwt) = (from p in tokens.Zip(jwts)
                            where p.Second.Subject.Equals(user.Id)
                            select p).FirstOrDefault();
        Assert.NotNull(token);
        Assert.NotNull(jwt);
        VerifyUserInformation(user, jwt);
        Assert.Equal(jwt.Subject, userManager.IsLoggedIn(token));
      }

      context.Dispose();
      connection.Close();
      connection.Dispose();
    }
    [Fact]
    public async Task ProvidesInformationAboutLoggedInUsers() {
      var data = UsersWithTokens(123321, 1).First();
      var token = (data[0] as object[])[0] as string;
      var jwt = (data[1] as object[])[0] as GoogleJsonWebSignature.Payload;
      var (context, connection) = GetDb();
      var userManager = new LetsMeatAPI.UserManager(context, Mock.Of<ILogger<LetsMeatAPI.UserManager>>());

      await userManager.OnTokenGranted(token, jwt);
      Assert.Equal(jwt.Subject, userManager.IsLoggedIn(token));
      Assert.Equal(jwt.Subject, userManager.LogOut(token));
      Assert.Null(userManager.LogOut(token));
      token += "dsjiadjsaoi";
      await userManager.OnTokenGranted(token, jwt);
      Assert.Equal(jwt.Subject, userManager.IsLoggedIn(token));
      Assert.Equal(jwt.Subject, userManager.LogOut(token));
      Assert.Null(userManager.LogOut(token));

      context.Dispose();
      connection.Close();
      connection.Dispose();
    }
    [Fact]
    public async Task UpdatesUserInformationUponLogin() {
      var data = UsersWithTokens(123321, 2).First();
      var token = (data[0] as object[])[0] as string;
      var jwt = (data[1] as object[])[0] as GoogleJsonWebSignature.Payload;
      var token2 = (data[0] as object[])[1] as string;
      var jwt2 = (data[1] as object[])[1] as GoogleJsonWebSignature.Payload;
      jwt2.Subject = jwt.Subject; // They are the same user
      var (context, connection) = GetDb();

      var userManager = new LetsMeatAPI.UserManager(context, Mock.Of<ILogger<LetsMeatAPI.UserManager>>());
      await userManager.OnTokenGranted(token, jwt);

      Assert.Equal(1, context.Users.Count());
      var user = context.Users.First();
      VerifyUserInformation(user, jwt);
      var newPrefs = "{likes:\"food\"}";
      user.Prefs = newPrefs;
      context.Users.Update(user);
      await context.SaveChangesAsync();

      await userManager.OnTokenGranted(token2, jwt2);

      Assert.Equal(1, context.Users.Count());
      user = context.Users.First();
      VerifyUserInformation(user, jwt2, newPrefs);
      Assert.Equal(jwt.Subject, userManager.IsLoggedIn(token2));
      Assert.Null(userManager.IsLoggedIn(token));

      context.Dispose();
      connection.Close();
      connection.Dispose();
    }
    [Fact]
    public async Task ThrowsDbUpdateExceptionOnRealDb() {
      var data = UsersWithTokens(123321, 2).First();
      var token1 = (data[0] as object[])[0] as string;
      var jwt1 = (data[1] as object[])[0] as GoogleJsonWebSignature.Payload;
      var token2 = (data[0] as object[])[1] as string;
      var jwt2 = (data[1] as object[])[1] as GoogleJsonWebSignature.Payload;
      jwt2.Subject = jwt1.Subject; // Force conflict
      var (context1, connection1) = GetDb();
      var context2 = new LetsMeatAPI.LMDbContext(
                          new DbContextOptionsBuilder<LetsMeatAPI.LMDbContext>()
                           .UseSqlite(connection1)
                           .LogTo(s => _output.WriteLine(s), LogLevel.Debug)
                           .EnableSensitiveDataLogging()
                           .Options
                         );
      var genuineUsers = context2.Users;
      var mockUsers = new Mock<DbSet<LetsMeatAPI.Models.User>>(MockBehavior.Strict);
      mockUsers.Setup(u => u.FindAsync(It.IsAny<object[]>()))
        .Returns<object[]>(async key => {
          Assert.Equal(new[] { jwt1.Subject }, key);
          var ret = await genuineUsers.FindAsync(key);
          Assert.Null(ret);
          await Task.Delay(1500);
          return ret;
        });
      mockUsers.Setup(u => u.AddAsync(It.IsAny<LetsMeatAPI.Models.User>(), It.IsAny<CancellationToken>()))
        .Returns<LetsMeatAPI.Models.User, CancellationToken>((u, t) => genuineUsers.AddAsync(u, t));

      var userManager1 = new LetsMeatAPI.UserManager(context1, Mock.Of<ILogger<LetsMeatAPI.UserManager>>());
      var userManager2 = new LetsMeatAPI.UserManager(context2, Mock.Of<ILogger<LetsMeatAPI.UserManager>>());
      context2.Users = mockUsers.Object;
      var create2 = Assert.ThrowsAsync<DbUpdateException>(() => userManager2.OnTokenGranted(token2, jwt2));
      await Task.Delay(500);
      await userManager1.OnTokenGranted(token1, jwt1);
      await create2;

      Assert.Equal(1, genuineUsers.Count());
      var user = genuineUsers.First();
      VerifyUserInformation(user, jwt2);

      context1.Dispose();
      context2.Dispose();
      connection1.Close();
      connection1.Dispose();
    }
    [Fact]
    public async Task ThrowsDbUpdateConcurrencyExceptionOnRealDb() {
      var data = UsersWithTokens(123321, 1).First();
      var token1 = (data[0] as object[])[0] as string;
      var jwt1 = (data[1] as object[])[0] as GoogleJsonWebSignature.Payload;
      var (context1, connection1) = GetDb();
      var context2 = new LetsMeatAPI.LMDbContext(
                          new DbContextOptionsBuilder<LetsMeatAPI.LMDbContext>()
                           .UseSqlite(connection1)
                           .LogTo(s => _output.WriteLine(s), LogLevel.Debug)
                           .EnableSensitiveDataLogging()
                           .Options
                         );
      context2.Users.Add(new LetsMeatAPI.Models.User {
        Id = jwt1.Subject,
        PictureUrl = jwt1.Picture,
        Email = jwt1.Email,
        Name = jwt1.Name,
        Prefs = "{}"
      });
      await context2.SaveChangesAsync();
      var genuineUsers = context1.Users;
      var mockUsers = new Mock<DbSet<LetsMeatAPI.Models.User>>(MockBehavior.Strict);
      mockUsers.Setup(u => u.FindAsync(It.IsAny<object[]>()))
        .Returns<object[]>(async key => {
          Assert.Equal(new[] { jwt1.Subject }, key);
          var ret = await genuineUsers.FindAsync(key);
          Assert.NotNull(ret);
          await Task.Delay(1500);
          return ret;
        });
      context1.Users = mockUsers.Object;
      var userManager1 = new LetsMeatAPI.UserManager(context1, Mock.Of<ILogger<LetsMeatAPI.UserManager>>());

      var update1 = Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => userManager1.OnTokenGranted(token1, jwt1));
      await Task.Delay(500);
      context2.Users.Remove(context2.Users.Find(jwt1.Subject));
      await context2.SaveChangesAsync();
      await update1;

      Assert.Equal(0, genuineUsers.Count());

      context1.Users = genuineUsers;
      context1.Dispose();
      context2.Dispose();
      connection1.Close();
      connection1.Dispose();
    }
    private void VerifyUserInformation(
      LetsMeatAPI.Models.User user,
      GoogleJsonWebSignature.Payload jwt,
      string prefs = "{}"
    ) {
      Assert.Equal(jwt.Subject, user.Id);
      Assert.Equal(jwt.Picture, user.PictureUrl);
      Assert.Equal(jwt.Email, user.Email);
      Assert.Equal(jwt.Name, user.Name);
      Assert.Equal(prefs, user.Prefs);
    }
    private static string RandomString(Random rnd, int length) {
      var bytes = new byte[length];
      rnd.NextBytes(bytes);
      return Convert.ToBase64String(bytes);
    }
    public static IEnumerable<object[]> UsersWithTokens(int seed, int n) {
      var rnd = new Random(seed);
      var tokens = new string[n];
      var jwts = new GoogleJsonWebSignature.Payload[n];
      while(n-- > 0) {
        tokens[n] = RandomString(rnd, 128);
        jwts[n] = new() {
          Subject = RandomString(rnd, 12),
          Picture = RandomString(rnd, 18),
          Email = RandomString(rnd, 10),
          Name = RandomString(rnd, 25)
        };
      }
      yield return new object[] {
          tokens,
          jwts
      };
    }
  }
}
