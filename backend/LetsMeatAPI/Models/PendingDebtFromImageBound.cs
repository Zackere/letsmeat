using System;

namespace LetsMeatAPI.Models {
  public class PendingDebtFromImageBound {
    public Guid DebtFromImageId { get; set; }
    public virtual DebtFromImage DebtFromImage { get; set; }
    public Guid PendingDebtId { get; set; }
    public virtual PendingDebt PendingDebt { get; set; }
  }
}
