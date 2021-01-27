using Google.Cloud.Vision.V1;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LetsMeatAPI.ExternalAPI {
  public interface IGoogleVision {
    public Task<string> DetectText(Uri imageUrl);
  }
  public class GoogleVision : IGoogleVision {
    public GoogleVision(
      string credentials,
      ILogger<GoogleVision> logger
    ) {
      _credentials = credentials;
      _logger = logger;
    }
    public async Task<string> DetectText(Uri imageUri) {
      var client = await new ImageAnnotatorClientBuilder {
        JsonCredentials = _credentials,
      }.BuildAsync();
      var annotations = await client.DetectDocumentTextAsync(Image.FromUri(imageUri));
      _logger.LogInformation(annotations.Text);
      return annotations.Text;
    }
    private readonly string _credentials;
    private readonly ILogger<GoogleVision> _logger;
  }
}
