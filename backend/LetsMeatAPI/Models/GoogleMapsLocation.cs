using System.Collections.Generic;

namespace LetsMeatAPI.Models {
  public class GoogleMapsLocation {
    public string Id { get; set; }
    public string Address { get; set; }
    public string Name { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public string Rating { get; set; }
    public virtual ICollection<Event> EventsWithMe { get; set; }
  }
}
