using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LetsMeatAPI.Models {
  public class User {
    [StringLength(128)]
    public string Id { get; set; }
    public string PictureUrl { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public int TastePref { get; set; }
    public int PricePref { get; set; }
    public int AmountOfFoodPref { get; set; }
    public int WaitingTimePref { get; set; }
    [StringLength(Controllers.LoginController.TokenLength)]
    public string? Token { get; set; }
    public virtual ICollection<Group> Groups { get; set; }
    public virtual ICollection<Debt> DebtsForOthers { get; set; }
    public virtual ICollection<Debt> DebtsForMe { get; set; }
    public virtual ICollection<PendingDebt> PendingDebtsForOthers { get; set; }
    public virtual ICollection<PendingDebt> PendingDebtsForMe { get; set; }
    public virtual ICollection<Invitation> Invitations { get; set; }
    public virtual ICollection<Group> OwnedGroups { get; set; }
    public virtual ICollection<Event> CreatedEvents { get; set; }
    public virtual ICollection<Image> UploadedImages { get; set; }
  }
}
