using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace LetsMeatAPI.RecieptExtractor {
  public interface ITextRecieptExtractor {
    public IEnumerable<PurchaseInformation> ExtractPurchases(string recieptText);
  }
  public class EmptyRecieptExtractor : ITextRecieptExtractor {
    public IEnumerable<PurchaseInformation> ExtractPurchases(string recieptText) {
      return Enumerable.Empty<PurchaseInformation>();
    }
  }
  public class PLMcDonaldsRecieptExtractor : ITextRecieptExtractor {
    public PLMcDonaldsRecieptExtractor(ITextRecieptExtractor next) {
      _next = next;
    }
    public IEnumerable<PurchaseInformation> ExtractPurchases(string recieptText) {
      var matches = _productListRx.Matches(recieptText);
      if(!matches.Any()) {
        foreach(var p in _next.ExtractPurchases(recieptText))
          yield return p;
        yield break;
      }
      var productList = _whitespaceRx.Replace(
        matches.First().Groups[1]?.Value.Trim(),
        _ => " "
      );
      foreach(var product in _productRx.Matches(productList)
                                       .Where(p => p is not null)
      ) {
        var groups = product.Groups;
        var n = int.Parse(groups[2].Value);
        while(n-- > 0) {
          double.TryParse(
            groups[3].Value.Replace(',', '.'),
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out var doubleAmount
          );
          yield return new PurchaseInformation {
            Amount = (uint)Math.Round(doubleAmount * 100),
            Description = groups[1].Value.Trim(),
          };
        }
      }
    }
    private readonly ITextRecieptExtractor _next;
    private const RegexOptions _regexpOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline;
    private static readonly Regex _productListRx = new Regex(@"fiskalny(.*)sprzedaz", _regexpOptions);
    private static readonly Regex _productRx = new Regex(
        @"(.+?) (\d+) ?. ?(\d+[,.]\d{2}) \d+[,.]\d{2}\w",
        _regexpOptions
    );
    private static readonly Regex _whitespaceRx = new Regex(@"\s+", _regexpOptions);
  }
}
