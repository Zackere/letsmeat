using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsMeatAPI.Models {
  public class Location {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Info { get; set; }
    public virtual ICollection<Event> EventsWithMe { get; set; }
  }
}
