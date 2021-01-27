namespace LetsMeatAPI.Models {
  public class GoogleMapsLocationReview : Review {
    public string GoogleMapsLocationId { get; set; }
    public virtual GoogleMapsLocation GoogleMapsLocation { get; set; }
  }
}
