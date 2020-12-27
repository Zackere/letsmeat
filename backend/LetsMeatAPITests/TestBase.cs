using Google.Apis.Auth;
using LetsMeatAPI;
using LetsMeatAPI.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace LetsMeatAPITests {
  public class TestBase {
    protected readonly ITestOutputHelper _output;
    public TestBase(ITestOutputHelper output) {
      _output = output;
    }
    public static Mock<IUserManager> UserManagerMock(User[] users) {
      var userManager = new Mock<IUserManager>(MockBehavior.Strict);
      foreach(var user in users)
        userManager.Setup(m => m.IsLoggedIn(user.Token)).Returns(user.Id);
      return userManager;
    }
    public static string RandomString(Random rnd, int length) {
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      return new string(Enumerable.Repeat(chars, length)
        .Select(s => s[rnd.Next(s.Length)]).ToArray());
    }
    public async Task<(
      User[],
      Group,
      Event[],
      CustomLocation[],
      GoogleMapsLocation[],
      Invitation[],
      Image[],
      Debt[],
      PendingDebt[]
      )> SeedDbWithOneGroup(string connection) {
      var users = CreateUsers(777, 4);
      var group = CreateGroups(890, 1)[0];
      group.Owner = users[0];
      foreach(var user in users.Take(3))
        group.Users.Add(user);
      var events = CreateEvents(682, 2);
      foreach(var ev in events)
        ev.Group = group;
      events[0].Creator = users[2];
      events[1].Creator = users[0];
      var customLocations = CreateCustomLocations(4056, 4);
      events[0].CandidateCustomLocations.Add(customLocations[0]);
      events[1].CandidateCustomLocations.Add(customLocations[1]);
      events[1].CandidateCustomLocations.Add(customLocations[2]);
      events[1].CandidateCustomLocations.Add(customLocations[3]);
      foreach(var location in customLocations)
        location.CreatedFor = group;
      var googleMapsLocations = CreateGoogleMapsLocations(8109, 2);
      events[0].CandidateGoogleMapsLocations.Add(googleMapsLocations[1]);
      events[1].CandidateGoogleMapsLocations.Add(googleMapsLocations[0]);
      var invitations = CreateInvitations(58, 1);
      invitations[0].From = users[0];
      invitations[0].Group = group;
      invitations[0].To = users[3];
      var images = CreateImages(5042, 3);
      foreach(var image in images)
        image.Group = group;
      images[0].Event = events[0];
      images[0].UploadedBy = users[1];
      images[1].Event = events[1];
      images[1].UploadedBy = users[0];
      images[2].Event = events[1];
      images[2].UploadedBy = users[0];
      var debts = CreateDebts(6170, 2);
      debts[0].From = users[0];
      debts[0].Group = group;
      debts[0].To = users[1];
      debts[1].From = users[1];
      debts[1].Group = group;
      debts[1].To = users[2];
      var pendingDebts = CreatePendingDebts(7042, 2);
      pendingDebts[0].Event = events[1];
      pendingDebts[0].From = users[0];
      pendingDebts[0].Group = group;
      pendingDebts[0].Image = images[2];
      pendingDebts[0].To = users[2];
      pendingDebts[1].Event = events[0];
      pendingDebts[1].From = users[1];
      pendingDebts[1].Group = group;
      pendingDebts[1].Image = images[0];
      pendingDebts[1].To = users[2];
      var contextSetup = CreateContextForConnection(connection);
      await Task.WhenAll(
        contextSetup.Users.AddRangeAsync(users),
        contextSetup.CustomLocations.AddRangeAsync(customLocations),
        contextSetup.GoogleMapsLocations.AddRangeAsync(googleMapsLocations),
        contextSetup.Invitations.AddRangeAsync(invitations),
        contextSetup.Images.AddRangeAsync(images),
        contextSetup.Debts.AddRangeAsync(debts),
        contextSetup.Groups.AddRangeAsync(group)
      );
      await contextSetup.SaveChangesAsync();
      return (users, group, events, customLocations, googleMapsLocations, invitations, images, debts, pendingDebts);
    }
    public static PendingDebt[] CreatePendingDebts(int seed, int n) {
      var ret = new PendingDebt[n];
      var rnd = new Random(seed);
      while(n-- > 0) {
        ret[n] = new PendingDebt {
          Amount = (uint)rnd.Next(0, 100),
          Description = RandomString(rnd, 56),
          Timestamp = DateTime.UtcNow.AddDays(rnd.NextDouble() * -2),
        };
      }
      return ret;
    }
    public static Debt[] CreateDebts(int seed, int n) {
      var ret = new Debt[n];
      var rnd = new Random(seed);
      while(n-- > 0) {
        ret[n] = new Debt {
          Amount = rnd.Next(0, 100),
        };
      }
      return ret;
    }
    public static Image[] CreateImages(int seed, int n) {
      var ret = new Image[n];
      var rnd = new Random(seed);
      while(n-- > 0) {
        ret[n] = new Image {
          PendingDebtsWithMe = new List<PendingDebt>(),
          Url = $"https://www.{RandomString(rnd, 32)}.com",
        };
      }
      return ret;
    }
    public static Invitation[] CreateInvitations(int seed, int n) {
      var ret = new Invitation[n];
      var rnd = new Random(seed);
      while(n-- > 0) {
        ret[n] = new Invitation {
          Sent = DateTime.UtcNow.AddDays(rnd.NextDouble() * -5),
        };
      }
      return ret;
    }
    public static Event[] CreateEvents(int seed, int n) {
      var ret = new Event[n];
      var rnd = new Random(seed);
      while(n-- > 0) {
        ret[n] = new Event {
          CandidateCustomLocations = new List<CustomLocation>(),
          CandidateGoogleMapsLocations = new List<GoogleMapsLocation>(),
          CandidateTimes = "[]",
          Deadline = DateTime.UtcNow.AddDays(1),
          Images = new List<Image>(),
          Name = RandomString(rnd, 32),
          PendingDebts = new List<PendingDebt>(),
          Result = null,
          Votes = new List<Vote>(),
        };
      }
      return ret;
    }
    public static GoogleMapsLocation[] CreateGoogleMapsLocations(int seed, int n) {
      var ret = new GoogleMapsLocation[n];
      var rnd = new Random(seed);
      while(n-- > 0) {
        ret[n] = new GoogleMapsLocation {
          AmountOfFood = (ulong)rnd.Next(0, 100),
          AmountOfFoodVotes = (ulong)rnd.Next(0, 100),
          EventsWithMe = new List<Event>(),
          FormattedAddress = RandomString(rnd, 12),
          ExpirationDate = DateTime.UtcNow.AddDays(100),
          Icon = RandomString(rnd, 64),
          Id = RandomString(rnd, 64),
          Name = RandomString(rnd, 15),
          Price = (ulong)rnd.Next(0, 100),
          PriceVotes = (ulong)rnd.Next(0, 100),
          Taste = (ulong)rnd.Next(0, 100),
          TasteVotes = (ulong)rnd.Next(0, 100),
          Url = RandomString(rnd, 64),
          Vicinity = RandomString(rnd, 56),
          WaitingTime = (ulong)rnd.Next(0, 100),
          WaitingTimeVotes = (ulong)rnd.Next(0, 100),
        };
      }
      return ret;
    }
    public static CustomLocation[] CreateCustomLocations(int seed, int n) {
      var ret = new CustomLocation[n];
      var rnd = new Random(seed);
      while(n-- > 0) {
        ret[n] = new CustomLocation {
          Address = RandomString(rnd, 12),
          AmountOfFood = (ulong)rnd.Next(0, 100),
          AmountOfFoodVotes = (ulong)rnd.Next(0, 100),
          EventsWithMe = new List<Event>(),
          Name = RandomString(rnd, 30),
          Price = (ulong)rnd.Next(0, 100),
          PriceVotes = (ulong)rnd.Next(0, 100),
          Taste = (ulong)rnd.Next(0, 100),
          TasteVotes = (ulong)rnd.Next(0, 100),
          WaitingTime = (ulong)rnd.Next(0, 100),
          WaitingTimeVotes = (ulong)rnd.Next(0, 100),
        };
      }
      return ret;
    }
    public static Group[] CreateGroups(int seed, int n) {
      var ret = new Group[n];
      var rnd = new Random(seed);
      while(n-- > 0) {
        ret[n] = new Group {
          CustomLocations = new List<CustomLocation>(),
          Debts = new List<Debt>(),
          Events = new List<Event>(),
          Images = new List<Image>(),
          Name = RandomString(rnd, 12),
          PendingDebts = new List<PendingDebt>(),
          Users = new List<User>(),
        };
      }
      return ret;
    }
    public static User[] CreateUsers(int seed, int n) {
      var (tokens, jwts) = UsersWithTokensData(seed, n);
      var rnd = new Random(seed);
      return (from t in tokens.Zip(jwts)
              select new User {
                AmountOfFoodPref = rnd.Next(0, 100),
                CreatedEvents = new List<Event>(),
                DebtsForMe = new List<Debt>(),
                DebtsForOthers = new List<Debt>(),
                Email = t.Second.Email,
                Groups = new List<Group>(),
                Id = t.Second.Subject,
                Invitations = new List<Invitation>(),
                Name = t.Second.Name,
                OwnedGroups = new List<Group>(),
                PendingDebtsForMe = new List<PendingDebt>(),
                PendingDebtsForOthers = new List<PendingDebt>(),
                PictureUrl = t.Second.Picture,
                PricePref = rnd.Next(0, 100),
                TastePref = rnd.Next(0, 100),
                Token = t.First,
                UploadedImages = new List<Image>(),
                WaitingTimePref = rnd.Next(0, 100),
              }).ToArray();
    }
    public static (string[] tokens, GoogleJsonWebSignature.Payload[] jwts)
      UsersWithTokensData(int seed, int n) {
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
      return (
          tokens,
          jwts
      );
    }
    public static IEnumerable<object[]> UsersWithTokens(int seed, int n) {
      var (tokens, jwts) = UsersWithTokensData(seed, n);
      yield return new object[] {
          tokens,
          jwts
      };
    }
    public LMDbContext CreateContextForConnection(string connstring) {
      var options = new DbContextOptionsBuilder<LMDbContext>()
                  .UseSqlite(connstring)
                  .UseLazyLoadingProxies()
                  .LogTo(s => _output.WriteLine(s), LogLevel.Debug)
                  .EnableSensitiveDataLogging()
                  .Options;
      return new LMDbContext(options);
    }
    public string GetDb() {
      var connectionStringBuilder = new SqliteConnectionStringBuilder() {
        DataSource = Path.GetRandomFileName()
      };
      using var context = CreateContextForConnection(
        connectionStringBuilder.ToString()
      );
      context.Database.EnsureCreated();
      context.Database.EnsureDeleted();
      context.Database.EnsureCreated();
      return connectionStringBuilder.ToString();
    }
  }
}
