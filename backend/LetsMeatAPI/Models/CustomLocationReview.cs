using System;

namespace LetsMeatAPI.Models {
  public class CustomLocationReview : Review {
    public Guid CustomLocationId { get; set; }
    public virtual CustomLocation CustomLocation { get; set; }
  }
}
