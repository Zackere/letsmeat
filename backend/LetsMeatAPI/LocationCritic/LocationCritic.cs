using LetsMeatAPI.Models;
using System;

namespace LetsMeatAPI {
  public interface ILocationCritic {
    public double AbsoluteScore(LocationBase location);
    public double PersonalScore(LocationBase location, User user);
  }
  public class LocationCritic : ILocationCritic {
    public double AbsoluteScore(LocationBase location) {
      var ret = 0.0;
      var nProps = 0;
      if(location.AmountOfFoodVotes > 0) {
        ret = (double)location.AmountOfFood / location.AmountOfFoodVotes;
        ++nProps;
      }
      if(location.PriceVotes > 0) {
        ret += (double)location.Price / location.PriceVotes;
        ++nProps;
      }
      if(location.TasteVotes > 0) {
        ret += (double)location.Taste / location.TasteVotes;
        ++nProps;
      }
      if(location.TasteVotes > 0) {
        ret += (double)location.WaitingTime / location.WaitingTimeVotes;
        ++nProps;
      }
      if(nProps > 0)
        return ret / nProps;
      return ret;
    }
    public double PersonalScore(LocationBase location, User user) {
      var ret = 0.0;
      var nProps = 0;
      if(location.AmountOfFoodVotes > 0) {
        ret = user.AmountOfFoodPref * (double)location.AmountOfFood / location.AmountOfFoodVotes;
        ++nProps;
      }
      if(location.PriceVotes > 0) {
        ret += user.PricePref * (double)location.Price / location.PriceVotes;
        ++nProps;
      }
      if(location.TasteVotes > 0) {
        ret += user.TastePref * (double)location.Taste / location.TasteVotes;
        ++nProps;
      }
      if(location.TasteVotes > 0) {
        ret += user.WaitingTimePref * (double)location.WaitingTime / location.WaitingTimeVotes;
        ++nProps;
      }
      if(nProps > 0)
        return ret / (nProps * 100);
      return ret;
    }
  }
}
