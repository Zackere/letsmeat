using Google.Apis.Auth;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit.Abstractions;

namespace LetsMeatAPITests {
  public class TestBase {
    protected readonly ITestOutputHelper _output;
    public TestBase(ITestOutputHelper output) {
      _output = output;
    }
    public static string RandomString(Random rnd, int length) {
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
    public (LetsMeatAPI.LMDbContext context, SqliteConnection connection) GetDb() {
      var connectionStringBuilder = new SqliteConnectionStringBuilder() {
        DataSource = Path.GetRandomFileName()
      };
      var connection = new SqliteConnection(connectionStringBuilder.ToString());
      var options = new DbContextOptionsBuilder<LetsMeatAPI.LMDbContext>()
                        .UseSqlite(connection)
                        .UseLazyLoadingProxies()
                        .LogTo(s => _output.WriteLine(s), LogLevel.Debug)
                        .EnableSensitiveDataLogging()
                        .Options;
      var context = new LetsMeatAPI.LMDbContext(options);
      context.Database.EnsureCreated();
      context.Database.EnsureDeleted();
      context.Database.EnsureCreated();
      return (context, connection);
    }
  }
}
