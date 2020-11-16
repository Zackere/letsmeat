using System;

namespace LetsMeatAPI.Models {
  public class Invitation {
    public string FromId { get; set; }
    public virtual User From { get; set; }
    public string ToId { get; set; }
    public virtual User To { get; set; }
    public Guid GroupId { get; set; }
    public virtual Group Group { get; set; }
  }
}
