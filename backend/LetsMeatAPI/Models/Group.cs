using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsMeatAPI.Models {
  public class Group {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [MaxLength(64)]
    [Required]
    public string Name { get; set; }
    public string OwnerId { get; set; }
    public virtual User Owner { get; set; }
    public virtual ICollection<User> Users { get; set; }
    public virtual ICollection<Event> Events { get; set; }
    public virtual ICollection<CustomLocation> CustomLocations { get; set; }
    public virtual ICollection<Image> Images { get; set; }
  }
}
