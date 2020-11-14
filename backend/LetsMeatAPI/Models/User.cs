using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LetsMeatAPI.Models {
  public class User {
    [StringLength(128)]
    public string Id { get; set; }
    public string PictureUrl { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Prefs { get; set; }
    public virtual ICollection<Group> Groups { get; set; }
    public virtual ICollection<Debt> DebtsForOthers { get; set; }
    public virtual ICollection<Debt> DebtsForMe { get; set; }
    public virtual ICollection<Invitation> Invitations { get; set; }
  }
}
