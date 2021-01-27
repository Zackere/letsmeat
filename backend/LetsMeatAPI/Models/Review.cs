using System;
using System.ComponentModel.DataAnnotations;

namespace LetsMeatAPI.Models {
  public class Review {
    [Range(0, 100)]
    public int Taste { get; set; }
    [Range(0, 100)]
    public int Price { get; set; }
    [Range(0, 100)]
    public int AmountOfFood { get; set; }
    [Range(0, 100)]
    public int WaitingTime { get; set; }
    public string UserId { get; set; }
    public virtual User User { get; set; }
  }
}
