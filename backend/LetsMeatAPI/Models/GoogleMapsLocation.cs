using System;
using System.Collections.Generic;

namespace LetsMeatAPI.Models {
  public class GoogleMapsLocation : LocationBase {
    public string Id { get; set; }
    public string? BusinessStatus { get; set; }
    public string FormattedAddress { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Url { get; set; }
    public string Vicinity { get; set; }
    public DateTime ExpirationDate { get; set; }
    public virtual ICollection<Event> EventsWithMe { get; set; }
  }
}
