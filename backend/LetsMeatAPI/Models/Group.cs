using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LetsMeatAPI.Models {
  public class Group {
    [StringLength(128)]
    public string Id { get; set; }
    [MaxLength(64)]
    [Required]
    public string Name { get; set; }
    public virtual ICollection<User> Users { get; set; }
    public virtual ICollection<Location> Locations { get; set; }
    public virtual ICollection<Event> Events { get; set; }
  }
}
