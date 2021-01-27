using LetsMeatAPI.ExternalAPI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetsMeatAPI.ReceiptExtractor {
  public interface IUriReceiptExtractor {
    public Task<IEnumerable<PurchaseInformation>> ExtractPurchases(Uri receiptUri);
  }
  public class GoogleVisionReceiptExtractor : IUriReceiptExtractor {
    public GoogleVisionReceiptExtractor(
      ITextReceiptExtractor textExtractor,
      IGoogleVision googleVision
    ) {
      _textExtractor = textExtractor;
      _googleVision = googleVision;
    }
    public async Task<IEnumerable<PurchaseInformation>> ExtractPurchases(Uri receiptUri) {
      return _textExtractor.ExtractPurchases(await _googleVision.DetectText(receiptUri));
    }
    private readonly ITextReceiptExtractor _textExtractor;
    private readonly IGoogleVision _googleVision;
  }
}
