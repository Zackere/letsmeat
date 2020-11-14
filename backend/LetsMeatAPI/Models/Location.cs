using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LetsMeatAPI.Models {
  public class Location {
    [StringLength(128)]
    public string Id { get; set; }
    public string Info { get; set; }
    public virtual ICollection<Event> EventsWithMe { get; set; }
  }
}
