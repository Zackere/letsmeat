namespace LetsMeatAPI.ReceiptExtractor {
  public class PurchaseInformation {
    public uint Amount { get; set; }
    public string Description { get; set; }
    public override bool Equals(object? obj) {
      if(obj is PurchaseInformation p) {
        return p.Amount == Amount && p.Description == Description;
      }
      return false;
    }
  }
}
