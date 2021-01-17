using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
    [Range(1.0, 5.0)]
    public double Rating { get; set; }
    public ulong UserRatingsTotal { get; set; }
    [Range(0, 4)]
    public int PriceLevel { get; set; }
    public virtual ICollection<GoogleMapsLocationReview> Reviews { get; set; }
    public virtual ICollection<Event> EventsWithMe { get; set; }
  }
}
