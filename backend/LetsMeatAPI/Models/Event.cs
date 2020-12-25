using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsMeatAPI.Models {
  public class Event {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public virtual Group Group { get; set; }
    public string CreatorId { get; set; }
    public virtual User Creator { get; set; }
    [MaxLength(64)]
    [Required]
    public string Name { get; set; }
    public string CandidateTimes { get; set; }
    public DateTime Deadline { get; set; }
    public string? Result { get; set; }
    public virtual ICollection<Vote> Votes { get; set; }
    public virtual ICollection<GoogleMapsLocation> CandidateGoogleMapsLocations { get; set; }
    public virtual ICollection<CustomLocation> CandidateCustomLocations { get; set; }
    public virtual ICollection<Image> Images { get; set; }
    public virtual ICollection<PendingDebt> PendingDebts { get; set; }
  }
}
