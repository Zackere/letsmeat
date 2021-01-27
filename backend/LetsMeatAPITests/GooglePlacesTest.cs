using LetsMeatAPI.ExternalAPI;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LetsMeatAPITests {
  public class GooglePlacesTest : TestBase {
    public class HttpClientFactoryMock : IHttpClientFactory {
      public HttpClient client;
      public HttpClient CreateClient(string name) {
        return client;
      }
    }
    private Mock<HttpMessageHandler> CreateHttpMessageHandlerMock(Uri url, string response) {
      var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
      handlerMock
        .Protected()
        .Setup<Task<HttpResponseMessage>>(
          "SendAsync",
          ItExpr.Is<HttpRequestMessage>(
            m => url.Scheme == m.RequestUri.Scheme &&
                 url.Host == m.RequestUri.Host &&
                 !url.Segments.Except(m.RequestUri.Segments).Any() &&
                 !url.Query.Substring(1)
                             .Split('&', StringSplitOptions.RemoveEmptyEntries)
                             .Except(m.RequestUri.Query.Substring(1)
                             .Split('&', StringSplitOptions.RemoveEmptyEntries))
                             .Any() &&
                 m.Method == HttpMethod.Get
          ),
          ItExpr.IsAny<CancellationToken>()
        ).Returns(Task.FromResult(new HttpResponseMessage {
          Content = new StringContent(response),
          StatusCode = HttpStatusCode.OK,
        }));
      return handlerMock;
    }
    public GooglePlacesTest(ITestOutputHelper output) : base(output) { }
    [Fact]
    public async Task DeserializesPlaceSearchResponseOnSuccess() {
      var apiKey = RandomString(new(11111), 128);
      var input = RandomString(new(123843), 12);
      var sessionToken = RandomString(new(111111), 64);
      const string placesResponse = @"{""status"":""OK"",""predictions"":[{""description"":""Paris, France"",""distance_meters"":8030004,""id"":""691b237b0322f28988f3ce03e321ff72a12167fd"",""matched_substrings"":[{""length"":5,""offset"":0}],""place_id"":""ChIJD7fiBh9u5kcRYJSMaMOCCwQ"",""reference"":""CjQlAAAA_KB6EEceSTfkteSSF6U0pvumHCoLUboRcDlAH05N1pZJLmOQbYmboEi0SwXBSoI2EhAhj249tFDCVh4R - PXZkPK8GhTBmp_6_lWljaf1joVs1SH2ttB_tw"",""terms"":[{""offset"":0,""value"":""Paris""},{""offset"":7,""value"":""France""}],""types"":[""locality"",""political"",""geocode""]},{""description"":""Paris - Madrid Grocery(Spanish Table Seattle), Western Avenue, Seattle, WA, USA"",""distance_meters"":12597,""id"":""f4231a82cfe0633a6a32e63538e61c18277d01c0"",""matched_substrings"":[{""length"":5,""offset"":0}],""place_id"":""ChIJHcYlZ7JqkFQRlpy - 6pytmPI"",""reference"":""ChIJHcYlZ7JqkFQRlpy - 6pytmPI"",""structured_formatting"":{""main_text"":""Paris - Madrid Grocery(Spanish Table Seattle)"",""main_text_matched_substrings"":[{""length"":5,""offset"":0}],""secondary_text"":""Western Avenue, Seattle, WA, USA""},""terms"":[{""offset"":0,""value"":""Paris - Madrid Grocery(Spanish Table Seattle)""},{""offset"":46,""value"":""Western Avenue""},{""offset"":62,""value"":""Seattle""},{""offset"":71,""value"":""WA""},{""offset"":75,""value"":""USA""}],""types"":[""grocery_or_supermarket"",""food"",""store"",""point_of_interest"",""establishment""]},{""description"":""Paris, TX, USA"",""distance_meters"":2712292,""id"":""518e47f3d7f39277eb3bc895cb84419c2b43b5ac"",""matched_substrings"":[{""length"":5,""offset"":0}],""place_id"":""ChIJmysnFgZYSoYRSfPTL2YJuck"",""reference"":""ChIJmysnFgZYSoYRSfPTL2YJuck"",""structured_formatting"":{""main_text"":""Paris"",""main_text_matched_substrings"":[{""length"":5,""offset"":0}],""secondary_text"":""TX, USA""},""terms"":[{""offset"":0,""value"":""Paris""},{""offset"":7,""value"":""TX""},{""offset"":11,""value"":""USA""}],""types"":[""locality"",""political"",""geocode""]}]}";
      var expectedUrl = new Uri($"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={input}&types=establishment&sessiontoken={sessionToken}&key={apiKey}");
      var handlerMock = CreateHttpMessageHandlerMock(expectedUrl, placesResponse);
      var places = new GooglePlaces(
        apiKey,
        new HttpClientFactoryMock { client = new(handlerMock.Object) },
        Mock.Of<ILogger<GooglePlaces>>()
      );
      var actual = await places.PlaceAutocomplete(input, sessionToken);
      var expected = JsonSerializer.Deserialize<IGooglePlaces.PlaceAutocompleteResponse>(placesResponse);
      Assert.Equal(expected.status, actual.status);
      Assert.NotNull(expected.predictions);
      Assert.NotNull(actual.predictions);
      Assert.Equal(expected.predictions.Count(), actual.predictions.Count());
      expected.predictions = expected.predictions.OrderBy(p => p.place_id);
      actual.predictions = actual.predictions.OrderBy(p => p.place_id);
      foreach(var (e, a) in expected.predictions.Zip(actual.predictions)) {
        Assert.Equal(e.description, a.description);
        Assert.Equal(e.place_id, a.place_id);
        if(e.structured_formatting == null) {
          Assert.Null(a.structured_formatting);
        } else {
          Assert.NotNull(a.structured_formatting);
          Assert.Equal(e.structured_formatting.main_text, a.structured_formatting.main_text);
          Assert.Equal(e.structured_formatting.secondary_text, a.structured_formatting.secondary_text);
        }
      }
    }
    [Fact]
    public async Task DeserializesPlaceDetailsOnSuccess() {
      var apiKey = RandomString(new(111111), 128);
      var placeId = RandomString(new(1238432), 12);
      var sessionToken = RandomString(new(1111113), 64);
      const string placesResponse = @"{""html_attributions"":[""jdsadjsa""],""result"":{""address_components"":[{""long_name"":""5"",""short_name"":""5"",""types"":[""floor""]},{""long_name"":""48"",""short_name"":""48"",""types"":[""street_number""]},{""long_name"":""Pirrama Road"",""short_name"":""Pirrama Rd"",""types"":[""route""]},{""long_name"":""Pyrmont"",""short_name"":""Pyrmont"",""types"":[""locality"",""political""]},{""long_name"":""Council of the City of Sydney"",""short_name"":""Sydney"",""types"":[""administrative_area_level_2"",""political""]},{""long_name"":""New South Wales"",""short_name"":""NSW"",""types"":[""administrative_area_level_1"",""political""]},{""long_name"":""Australia"",""short_name"":""AU"",""types"":[""country"",""political""]},{""long_name"":""2009"",""short_name"":""2009"",""types"":[""postal_code""]}],""adr_address"":""5, \u003cspan class=\""street-address\""\u003e48 Pirrama Rd\u003c/span\u003e, \u003cspan class=\""locality\""\u003ePyrmont\u003c/span\u003e \u003cspan class=\""region\""\u003eNSW\u003c/span\u003e \u003cspan class=\""postal-code\""\u003e2009\u003c/span\u003e, \u003cspan class=\""country-name\""\u003eAustralia\u003c/span\u003e"",""formatted_address"":""5, 48 Pirrama Rd, Pyrmont NSW 2009, Australia"",""formatted_phone_number"":""(02) 9374 4000"",""geometry"":{""location"":{""lat"":-33.866651,""lng"":151.195827},""viewport"":{""northeast"":{""lat"":-33.8653881697085,""lng"":151.1969739802915},""southwest"":{""lat"":-33.86808613029149,""lng"":151.1942760197085}}},""icon"":""https://maps.gstatic.com/mapfiles/place_api/icons/generic_business-71.png"",""id"":""4f89212bf76dde31f092cfc14d7506555d85b5c7"",""international_phone_number"":""+61 2 9374 4000"",""name"":""Google"",""place_id"":""ChIJN1t_tDeuEmsRUsoyG83frY4"",""rating"":4.5,""reference"":""CmRSAAAAjiEr2_A4yI-DyqGcfsceTv-IBJXHB5-W3ckmGk9QAYk4USgeV8ihBcGBEK5Z1w4ajRZNVAfSbROiKbbuniq0c9rIq_xqkrf_3HpZzX-pFJuJY3cBtG68LSAHzWXB8UzwEhAx04rgN0_WieYLfVp4K0duGhTU58LFaqwcaex73Kcyy0ghYOQTkg"",""reviews"":[{""author_name"":""Robert Ardill"",""author_url"":""https://www.google.com/maps/contrib/106422854611155436041/reviews"",""language"":""en"",""profile_photo_url"":""https://lh3.googleusercontent.com/-T47KxWuAoJU/AAAAAAAAAAI/AAAAAAAAAZo/BDmyI12BZAs/s128-c0x00000000-cc-rp-mo-ba1/photo.jpg"",""rating"":5,""relative_time_description"":""a month ago"",""text"":""Awesome offices. Great facilities, location and views. Staff are great hosts"",""time"":1491144016}],""types"":[""point_of_interest"",""establishment""],""url"":""https://maps.google.com/?cid=10281119596374313554"",""utc_offset"":600,""vicinity"":""5, 48 Pirrama Road, Pyrmont"",""website"":""https://www.google.com.au/about/careers/locations/sydney/""},""status"":""OK""}";
      var expectedUrl = new Uri($"https://maps.googleapis.com/maps/api/place/details/json?fields=business_status,formatted_address,icon,name,place_id,price_level,rating,url,user_ratings_total,vicinity&sessiontoken={sessionToken}&key={apiKey}");
      var handlerMock = CreateHttpMessageHandlerMock(expectedUrl, placesResponse);
      var places = new GooglePlaces(
        apiKey,
        new HttpClientFactoryMock { client = new(handlerMock.Object) },
        Mock.Of<ILogger<GooglePlaces>>()
      );
      var actual = await places.AdvancedPlaceDetails(placeId, sessionToken);
      var expected = JsonSerializer.Deserialize<IGooglePlaces.AdvancedPlaceDetailsResponse>(placesResponse);
      Assert.True(actual.html_attributions.OrderBy(s => s).SequenceEqual(expected.html_attributions.OrderBy(s => s)));
      Assert.Equal(actual.status, expected.status);
      foreach(var property in typeof(IGooglePlaces.AdvancedPlaceDetailsResponse.Result).GetProperties())
        Assert.Equal(property.GetValue(actual.result), property.GetValue(expected.result));
    }
  }
}
