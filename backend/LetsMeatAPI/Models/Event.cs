using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LetsMeatAPI.Models {
  public class Event {
    [StringLength(128)]
    public string Id { get; set; }
    [MaxLength(64)]
    [Required]
    public string Name { get; set; }
    public string CandidateTimes { get; set; }
    public virtual ICollection<Location> CandidateLocations { get; set; }
    public DateTime Deadline { get; set; }
    public string? Result { get; set; }
    public virtual ICollection<Vote> Votes { get; set; }
  }
}
