using LetsMeatAPI.Models;
using System.Linq;

namespace LetsMeatAPI {
  public interface ILocationCritic {
    public double AbsoluteScore(GoogleMapsLocation location);
    public double PersonalScore(GoogleMapsLocation location, User user);
    public double AbsoluteScore(CustomLocation location);
    public double PersonalScore(CustomLocation location, User user);
  }
  public class LocationCritic : ILocationCritic {
    public double AbsoluteScore(GoogleMapsLocation location) {
      if(!location.Reviews.Any() && location.UserRatingsTotal == 0)
        return 50;
      var food = location.Reviews.Select(r => (decimal)r.AmountOfFood).Sum();
      var price = location.Reviews.Select(r => (decimal)r.Price).Sum();
      var taste = location.Reviews.Select(r => (decimal)r.Taste).Sum();
      var time = location.Reviews.Select(r => (decimal)r.WaitingTime).Sum();
      price += (4m - location.PriceLevel) / 4 * 100 * location.UserRatingsTotal;
      var locationScore = (decimal)(location.Rating - 1) / 4 * 100 * location.UserRatingsTotal;
      food += locationScore;
      taste += locationScore;
      time += locationScore;
      return (double)((food + price + taste + time)
        / (4 * ((ulong)location.Reviews.Count() + location.UserRatingsTotal)));
    }
    public double PersonalScore(GoogleMapsLocation location, User user) {
      if(!location.Reviews.Any())
        return 50;
      var review = user.GoogleMapsLocationReviews
        .Where(r => r.GoogleMapsLocationId == location.Id)
        .SingleOrDefault();
      if(review != null)
        return (review.AmountOfFood + review.Price + review.Taste + review.WaitingTime) / 4;
      var food = location.Reviews.Select(r => (decimal)r.AmountOfFood).Sum();
      var price = location.Reviews.Select(r => (decimal)r.Price).Sum();
      var taste = location.Reviews.Select(r => (decimal)r.Taste).Sum();
      var time = location.Reviews.Select(r => (decimal)r.WaitingTime).Sum();
      price += (4m - location.PriceLevel) / 4 * 100 * location.UserRatingsTotal;
      var locationScore = (decimal)(location.Rating - 1) / 4 * 100 * location.UserRatingsTotal;
      food += locationScore;
      taste += locationScore;
      time += locationScore;
      return (double)((
        food * user.AmountOfFoodPref +
        price * user.PricePref +
        taste * user.TastePref +
        time * user.WaitingTimePref
        ) / (4 * ((ulong)location.Reviews.Count() + location.UserRatingsTotal) * 100));
    }
    public double AbsoluteScore(CustomLocation location) {
      if(!location.Reviews.Any())
        return 50;
      var food = location.Reviews.Select(r => (decimal)r.AmountOfFood).Sum();
      var price = location.Reviews.Select(r => (decimal)r.Price).Sum();
      var taste = location.Reviews.Select(r => (decimal)r.Taste).Sum();
      var time = location.Reviews.Select(r => (decimal)r.WaitingTime).Sum();
      return (double)((food + price + taste + time) / (4 * location.Reviews.Count()));
    }
    public double PersonalScore(CustomLocation location, User user) {
      if(!location.Reviews.Any())
        return 50;
      var review = user.CustomLocationReviews
        .Where(r => r.CustomLocationId == location.Id)
        .SingleOrDefault();
      if(review != null)
        return (review.AmountOfFood + review.Price + review.Taste + review.WaitingTime) / 4;
      var food = location.Reviews.Select(r => (decimal)r.AmountOfFood).Sum();
      var price = location.Reviews.Select(r => (decimal)r.Price).Sum();
      var taste = location.Reviews.Select(r => (decimal)r.Taste).Sum();
      var time = location.Reviews.Select(r => (decimal)r.WaitingTime).Sum();
      return (double)((
        food * user.AmountOfFoodPref +
        price * user.PricePref +
        taste * user.TastePref +
        time * user.WaitingTimePref
        ) / (4 * location.Reviews.Count() * 100));
    }
  }
}
