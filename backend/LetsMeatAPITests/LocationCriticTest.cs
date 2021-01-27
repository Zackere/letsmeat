using LetsMeatAPI;
using LetsMeatAPI.Models;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace LetsMeatAPITests {
  public class LocationCriticTest : TestBase {
    public LocationCriticTest(ITestOutputHelper output) : base(output) { }
    [Fact]
    public void CanRateCustomLocationsWithNoReviews() {
      var location = CreateCustomLocations(23194, 1, 0)[0];
      var critic = new LocationCritic();
      var absoluteScore = critic.AbsoluteScore(location);
      Assert.True(0 <= absoluteScore);
      Assert.True(absoluteScore <= 100);
    }
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(12)]
    [InlineData(13)]
    [InlineData(14)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(17)]
    [InlineData(18)]
    [InlineData(19)]
    [InlineData(20)]
    public void CanRateCustomLocationsWithSomeReviews(int count) {
      var rnd = new Random(count * 100);
      var location = CreateCustomLocations(count * 100, 1, 0)[0];
      location.Reviews = Enumerable.Repeat(0, count).Select(
                _ => new CustomLocationReview {
                  AmountOfFood = rnd.Next(0, 100),
                  Price = rnd.Next(0, 100),
                  Taste = rnd.Next(0, 100),
                  WaitingTime = rnd.Next(0, 100),
                }).ToList();
      var critic = new LocationCritic();
      var absoluteScore = critic.AbsoluteScore(location);
      Assert.True(0 <= absoluteScore);
      Assert.True(absoluteScore <= 100);
    }
    [Fact]
    public void CanRateCustomLocationsWithOnlyBadReviews() {
      var location = CreateCustomLocations(67238, 1, 0)[0];
      location.Reviews = Enumerable.Repeat(0, 10).Select(
                _ => new CustomLocationReview {
                  AmountOfFood = 0,
                  Price = 0,
                  Taste = 0,
                  WaitingTime = 0,
                }).ToList();
      var critic = new LocationCritic();
      var absoluteScore = critic.AbsoluteScore(location);
      Assert.Equal(0, absoluteScore);
    }
    [Fact]
    public void CanRateCustomLocationsWithOnlyPerfectReviews() {
      var location = CreateCustomLocations(62238, 1, 0)[0];
      location.Reviews = Enumerable.Repeat(0, 10).Select(
                _ => new CustomLocationReview {
                  AmountOfFood = 100,
                  Price = 100,
                  Taste = 100,
                  WaitingTime = 100,
                }).ToList();
      var critic = new LocationCritic();
      var absoluteScore = critic.AbsoluteScore(location);
      Assert.Equal(100, absoluteScore);
    }
    [Fact]
    public void CanRateCustomLocationRespectingMaxUserPreferences() {
      var location = CreateCustomLocations(62238, 1, 0)[0];
      location.Reviews = Enumerable.Repeat(0, 10).Select(
                _ => new CustomLocationReview {
                  AmountOfFood = 100,
                  Price = 100,
                  Taste = 100,
                  WaitingTime = 100,
                }).ToList();
      var user = CreateUsers(45678, 1)[0];
      user.AmountOfFoodPref = user.PricePref = user.TastePref = user.WaitingTimePref = 100;
      var critic = new LocationCritic();
      var personalScore = critic.PersonalScore(location, user);
      Assert.Equal(100, personalScore);
    }
    [Fact]
    public void CanRateCustomLocationRespectingUserPreferences() {
      var location = CreateCustomLocations(62238, 1, 0)[0];
      location.Reviews = Enumerable.Repeat(0, 10).Select(
                _ => new CustomLocationReview {
                  AmountOfFood = 100,
                  Price = 100,
                  Taste = 100,
                  WaitingTime = 100,
                }).ToList();
      var user = CreateUsers(45678, 1)[0];
      user.AmountOfFoodPref = 100; // At least one preference is always at 100
      var critic = new LocationCritic();
      var personalScore = critic.PersonalScore(location, user);
      Assert.True(0 <= personalScore);
      Assert.True(personalScore <= 100);
    }
  }
}
