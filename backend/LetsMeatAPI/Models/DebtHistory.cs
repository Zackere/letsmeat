using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsMeatAPI.Models {
  public class DebtHistory {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public Guid? PendingDebtId { get; set; }
    public string FromId { get; set; }
    public string ToId { get; set; }
    public Guid GroupId { get; set; }
    public Guid? EventId { get; set; }
    public uint Amount { get; set; }
    public DateTime Timestamp { get; set; }
    [MaxLength(250)]
    public string Description { get; set; }
    public Guid? ImageId { get; set; }
    public Guid? ImageDebtId { get; set; }
    public int Type { get; set; }
    public DateTime HistoryEntryCreatedOn { get; set; }
  }
}
