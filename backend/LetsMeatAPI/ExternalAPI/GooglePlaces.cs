using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LetsMeatAPI.ExternalAPI {
  public interface IGooglePlaces {
    public class PlaceAutocompleteResponse {
      public class Prediction {
        public class StructuredFormatting {
          public string? main_text { get; set; }
          public string? secondary_text { get; set; }
        }
        public string description { get; set; }
        public string place_id { get; set; }
        public StructuredFormatting? structured_formatting { get; set; }
      }
      public string status { get; set; }
      public IEnumerable<Prediction>? predictions { get; set; }
    }
    public Task<PlaceAutocompleteResponse?> PlaceAutocomplete(string input, string sessionToken);
    public class AdvancedPlaceDetailsResponse {
      public class Result {
        public string? business_status { get; set; }
        public string formatted_address { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public string place_id { get; set; }
        [Range(0, 4)]
        public int price_level { get; set; }
        [Range(1.0, 5.0)]
        public double rating { get; set; }
        public string url { get; set; }
        public ulong user_ratings_total { get; set; }
        public string vicinity { get; set; }
      }
      public IEnumerable<string> html_attributions { get; set; }
      public Result result { get; set; }
      public string status { get; set; }
    }
    public Task<AdvancedPlaceDetailsResponse?> AdvancedPlaceDetails(string placeId, string? sessionToken);
    public class BasicPlaceDetailsResponse {
      public class Result {
        public string? business_status { get; set; }
        public string formatted_address { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public string place_id { get; set; }
        public string url { get; set; }
        public string vicinity { get; set; }
      }
      public IEnumerable<string> html_attributions { get; set; }
      public Result result { get; set; }
      public string status { get; set; }
    }
    public Task<BasicPlaceDetailsResponse?> BasicPlaceDetails(string placeId, string? sessionToken);
  }
  public class GooglePlaces : IGooglePlaces {
    public GooglePlaces(
      string apiKey,
      IHttpClientFactory httpClientFactory,
      ILogger<GooglePlaces> logger
    ) {
      _apiKey = apiKey;
      _httpClientFactory = httpClientFactory;
      _logger = logger;
    }
    public async Task<IGooglePlaces.PlaceAutocompleteResponse?>
      PlaceAutocomplete(string input, string sessionToken) {
      const string baseAddr = "https://maps.googleapis.com/maps/api/place/autocomplete/json";
      _logger.LogInformation($"Sending autocomplete request to {baseAddr} for {input}");
      var request = new HttpRequestMessage(
        HttpMethod.Get,
        $"{baseAddr}?input={input}&types=establishment&sessiontoken={sessionToken}&key={_apiKey}"
      );
      var client = _httpClientFactory.CreateClient();
      var response = await client.SendAsync(request);
      if(!response.IsSuccessStatusCode) {
        _logger.LogError(response.ToString());
        return null;
      }
      using var responseStream = await response.Content.ReadAsStreamAsync();
      return await JsonSerializer.DeserializeAsync<IGooglePlaces.PlaceAutocompleteResponse>(responseStream);
    }
    public async Task<IGooglePlaces.AdvancedPlaceDetailsResponse?> AdvancedPlaceDetails(string placeId, string? sessionToken) {
      return await FetchPlaceDetails<IGooglePlaces.AdvancedPlaceDetailsResponse>(_advancedFields, placeId, sessionToken);
    }

    public async Task<IGooglePlaces.BasicPlaceDetailsResponse?> BasicPlaceDetails(string placeId, string? sessionToken) {
      return await FetchPlaceDetails<IGooglePlaces.BasicPlaceDetailsResponse>(_basicFields, placeId, sessionToken);
    }
    private async Task<T?> FetchPlaceDetails<T>(
      string fields,
      string placeId,
      string? sessionToken
    ) {
      const string baseAddr = "https://maps.googleapis.com/maps/api/place/details/json";
      _logger.LogInformation($"Sending details request to {baseAddr} with fields: {fields} for {placeId}");
      var request = new HttpRequestMessage(
        HttpMethod.Get,
        $"{baseAddr}?fields={fields}&place_id={placeId}&key={_apiKey}" + (sessionToken != null ? $"&sessiontoken={sessionToken}" : "")
      );
      var client = _httpClientFactory.CreateClient();
      var response = await client.SendAsync(request);
      if(!response.IsSuccessStatusCode) {
        _logger.LogError(response.ToString());
        return default;
      }
      using var responseStream = await response.Content.ReadAsStreamAsync();
      return await JsonSerializer.DeserializeAsync<T>(responseStream);
    }
    private readonly string _apiKey;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GooglePlaces> _logger;
    private static readonly string _advancedFields = string.Join(',', new[] {
        "business_status",
        "formatted_address",
        "icon",
        "name",
        "place_id",
        "price_level",
        "rating",
        "url",
        "user_ratings_total",
        "vicinity",
      }.OrderBy(s => s));
    private static readonly string _basicFields = string.Join(',', new[] {
        "business_status",
        "formatted_address",
        "icon",
        "name",
        "place_id",
        "url",
        "vicinity",
      }.OrderBy(s => s));
  }
}
