using System;

namespace LetsMeatAPI.Models {
  public class Vote {
    public Guid EventId { get; set; }
    public string UserId { get; set; }
    public string Order { get; set; }
  }
}
