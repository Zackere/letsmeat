using LetsMeatAPI.ExternalAPI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetsMeatAPI.RecieptExtractor {
  public interface IUriRecieptExtractor {
    public Task<IEnumerable<PurchaseInformation>> ExtractPurchases(Uri recieptUri);
  }
  public class GoogleVisionRecieptExtractor : IUriRecieptExtractor {
    public GoogleVisionRecieptExtractor(
      ITextRecieptExtractor textExtractor,
      IGoogleVision googleVision
    ) {
      _textExtractor = textExtractor;
      _googleVision = googleVision;
    }
    public async Task<IEnumerable<PurchaseInformation>> ExtractPurchases(Uri recieptUri) {
      return _textExtractor.ExtractPurchases(await _googleVision.DetectText(recieptUri));
    }
    private readonly ITextRecieptExtractor _textExtractor;
    private readonly IGoogleVision _googleVision;
  }
}
