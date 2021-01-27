using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsMeatAPI.Models {
  public class PendingDebt {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string FromId { get; set; }
    public virtual User From { get; set; }
    public string ToId { get; set; }
    public virtual User To { get; set; }
    public Guid GroupId { get; set; }
    public virtual Group Group { get; set; }
    public Guid? EventId { get; set; }
    public virtual Event? Event { get; set; }
    public uint Amount { get; set; }
    public DateTime Timestamp { get; set; }
    [MaxLength(250)]
    public string Description { get; set; }
    public Guid? ImageId { get; set; }
    public virtual Image? Image { get; set; }
    public virtual PendingDebtFromImageBound? Bound { get; set; }
    public int Type { get; set; }
  }
}
