namespace LetsMeatAPI.Models {
  public class Debt {
    public string FromId { get; set; }
    public virtual User From { get; set; }
    public string ToId { get; set; }
    public virtual User To { get; set; }
    public string GroupId { get; set; }
    public virtual Group Group { get; set; }
    public int Amount { get; set; }
  }
}
