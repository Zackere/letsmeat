using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsMeatAPI.Models {
  public class Image {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [MaxLength(512)]
    public string Url { get; set; }
    public Guid? EventId { get; set; }
    public virtual Event? Event { get; set; }
    public Guid GroupId { get; set; }
    public virtual Group Group { get; set; }
    public string UploadedById { get; set; }
    public virtual User UploadedBy { get; set; }
    public DateTime UploadTime { get; set; }
    public virtual ICollection<PendingDebt> PendingDebtsWithMe { get; set; }
  }
}
