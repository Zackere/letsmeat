namespace LetsMeatAPI.Models {
  public class LocationBase {
    public ulong Taste { get; set; }
    public ulong TasteVotes { get; set; }
    public ulong Price { get; set; }
    public ulong PriceVotes { get; set; }
    public ulong AmountOfFood { get; set; }
    public ulong AmountOfFoodVotes { get; set; }
    public ulong WaitingTime { get; set; }
    public ulong WaitingTimeVotes { get; set; }
  }
}
