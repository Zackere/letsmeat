using System.Collections.Generic;

namespace LetsMeatAPI.Models {
  public class User {
    public string Id { get; set; }
    public string PictureUrl { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Prefs { get; set; }
    public virtual ICollection<Group> Groups { get; set; }
  }
}
