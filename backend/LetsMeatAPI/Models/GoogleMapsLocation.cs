using System;
using System.Collections.Generic;

namespace LetsMeatAPI.Models {
  public class GoogleMapsLocation {
    public string Id { get; set; }
    public string? BusinessStatus { get; set; }
    public string FormattedAddress { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Url { get; set; }
    public string Vicinity { get; set; }
    public DateTime ExpirationDate { get; set; }
    public ulong Taste { get; set; }
    public ulong TasteVotes { get; set; }
    public ulong Price { get; set; }
    public ulong PriceVotes { get; set; }
    public ulong AmountOfFood { get; set; }
    public ulong AmountOfFoodVotes { get; set; }
    public ulong WaitingTime { get; set; }
    public ulong WaitingTimeVotes { get; set; }
    public virtual ICollection<Event> EventsWithMe { get; set; }
  }
}
