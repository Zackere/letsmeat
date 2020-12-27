using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsMeatAPI.Models {
  public class CustomLocation : LocationBase {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public Guid CreatedForId { get; set; }
    public virtual Group CreatedFor { get; set; }
    [MaxLength(128)]
    [Required]
    public string Address { get; set; }
    [MaxLength(64)]
    [Required]
    public string Name { get; set; }
    public virtual ICollection<Event> EventsWithMe { get; set; }
  }
}
