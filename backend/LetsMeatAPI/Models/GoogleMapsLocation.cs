using System.Collections.Generic;

namespace LetsMeatAPI.Models {
  public class GoogleMapsLocation {
    public string Id { get; set; }
    public string Address { get; set; }
    public string Name { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
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
