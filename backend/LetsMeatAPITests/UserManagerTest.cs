using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                        .LogTo(s => _output.WriteLine(s), LogLevel.Information)
                        .UseLazyLoadingProxies()
                        .Options;
      var context = new LetsMeatAPI.LMDbContext(options);
      context.Database.EnsureCreated();
      context.Database.EnsureDeleted();
      context.Database.EnsureCreated();
      return (context, connection);
    }
    [Theory]
    [MemberData(nameof(UsersWithTokens), 35712, 1)]
    public void RegistersUsersOnTokenGrantedForTheFirstLoginEver(
      string[] tokens,
      Google.Apis.Auth.GoogleJsonWebSignature.Payload[] jwts
    ) {
      var (context, connection) = GetDb();
      var userManager = new LetsMeatAPI.UserManager(context, Mock.Of<ILogger<LetsMeatAPI.UserManager>>());
      foreach(var (token, jwt) in tokens.Zip(jwts))
        userManager.OnTokenGranted(token, jwt);

      foreach(var user in context.Users) {
        var (token, jwt) = (from p in tokens.Zip(jwts)
                            where p.Second.Subject.Equals(user.Id)
                            select p).FirstOrDefault();
        Assert.NotNull(token);
        Assert.NotNull(jwt);
        Assert.Equal(jwt.Subject, user.Id);
        Assert.Equal(jwt.Picture, user.PictureUrl);
        Assert.Equal(jwt.Email, user.Email);
        Assert.Equal(jwt.Name, user.Name);
        Assert.Equal("{}", user.Prefs);
        Assert.Equal(jwt.Subject, userManager.IsLoggedIn(token));
      }

      connection.Close();
      connection.Dispose();
    }
    [Fact]
    public void ProvidesInformationAboutLoggedInUsers() {
      var data = UsersWithTokens(123321, 1);
      var token = (data.First()[0] as object[])[0] as string;
      var jwt = (data.First()[1] as object[])[0] as Google.Apis.Auth.GoogleJsonWebSignature.Payload;
      var (context, connection) = GetDb();
      var userManager = new LetsMeatAPI.UserManager(context, Mock.Of<ILogger<LetsMeatAPI.UserManager>>());

      userManager.OnTokenGranted(token, jwt);
      Assert.Equal(jwt.Subject, userManager.IsLoggedIn(token));
      Assert.True(userManager.LogOut(token));
      Assert.False(userManager.LogOut(token));
      token += "dsjiadjsaoi";
      userManager.OnTokenGranted(token, jwt);
      Assert.Equal(jwt.Subject, userManager.IsLoggedIn(token));
      Assert.True(userManager.LogOut(token));
      Assert.False(userManager.LogOut(token));

      connection.Close();
      connection.Dispose();
    }
    [Fact]
    public void UpdatesUserInformationUponLogin() {
      var data = UsersWithTokens(123321, 1);
      var token = (data.First()[0] as object[])[0] as string;
      var jwt = (data.First()[1] as object[])[0] as Google.Apis.Auth.GoogleJsonWebSignature.Payload;
      data = UsersWithTokens(1233211, 1);
      var token2 = (data.First()[0] as object[])[0] as string;
      var jwt2 = (data.First()[1] as object[])[0] as Google.Apis.Auth.GoogleJsonWebSignature.Payload;
      jwt2.Subject = jwt.Subject; // They are the same user
      var (context, connection) = GetDb();

      var userManager = new LetsMeatAPI.UserManager(context, Mock.Of<ILogger<LetsMeatAPI.UserManager>>());
      userManager.OnTokenGranted(token, jwt);

      Assert.Equal(1, context.Users.Count());
      var user = context.Users.First();
      var newPrefs = "{likes:\"food\"}";
      user.Prefs = newPrefs;
      Assert.NotNull(user);

      userManager.OnTokenGranted(token2, jwt2);

      Assert.Equal(1, context.Users.Count());
      user = context.Users.First();
      Assert.NotNull(user);
      Assert.Equal(jwt2.Subject, user.Id);
      Assert.Equal(jwt2.Picture, user.PictureUrl);
      Assert.Equal(jwt2.Email, user.Email);
      Assert.Equal(jwt2.Name, user.Name);
      Assert.Equal(newPrefs, user.Prefs);
      Assert.Equal(jwt.Subject, userManager.IsLoggedIn(token2));
      Assert.Null(userManager.IsLoggedIn(token));

      connection.Close();
      connection.Dispose();
    }
    private static string RandomString(Random rnd, int length) {
      var bytes = new byte[length];
      rnd.NextBytes(bytes);
      return Encoding.UTF8.GetString(bytes);
    }
    public static IEnumerable<object[]> UsersWithTokens(int seed, int n) {
      var rnd = new Random(seed);
      while(n-- > 0) {
        yield return new object[] {
          new[] { RandomString(rnd, 128) },
          new[] {
            new Google.Apis.Auth.GoogleJsonWebSignature.Payload {
              Subject = RandomString(rnd, 12),
              Picture = RandomString(rnd, 18),
              Email = RandomString(rnd, 10),
              Name = RandomString(rnd, 25)
            }
          }
        };
      }
    }
  }
}
