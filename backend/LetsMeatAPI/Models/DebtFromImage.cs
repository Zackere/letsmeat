using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsMeatAPI.Models {
  public class DebtFromImage {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public uint Amount { get; set; }
    [MaxLength(128)]
    public string Description { get; set; }
    public Guid ImageId { get; set; }
    public virtual Image Image { get; set; }
    public bool Satisfied { get; set; }
    public virtual PendingDebtFromImageBound? Bound { get; set; }
  }
}
