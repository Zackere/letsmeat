using System.Collections.Generic;

namespace LetsMeatAPI.Models {
  public class Group {
    public string Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<User> Users { get; set; }
  }
}
