using LetsMeatAPI.RecieptExtractor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace LetsMeatAPITests {
  public class TextRecieptExtractorsTest : TestBase {
    public TextRecieptExtractorsTest(ITestOutputHelper output) : base(output) { }
    [Theory]
    [ClassData(typeof(RecieptData))]
    public void PLMcDonaldsRecieptExtractorExtractsFromPolishReciepts(
      string recieptText,
      PurchaseInformation[] expectedPurchaseInformation
    ) {
      var ret = new PLMcDonaldsRecieptExtractor(
        new EmptyRecieptExtractor()
      ).ExtractPurchases(recieptText);
      foreach(var p in ret)
        _output.WriteLine($"{p.Amount} {p.Description}");
      Assert.Equal(expectedPurchaseInformation, ret.ToArray());
    }
    public class RecieptData : IEnumerable<object[]> {
      public IEnumerator<object[]> GetEnumerator() {
        yield return new object[] { @"KH Group Kristóf Hankó
Restauracja McDonald's
Warszawa 01 Centrum (464)
ul. Swi?tokrzyska 35
00-049 Warszawa
NIP 7010001447
nr: 113735
PARAGON FISKALNY
*Kurczakburger... 1 x4,00 4,00B
*Frytki Srednie
1 x3,90 3,90B
SPRZEDAZ OPODATKOWANA B
7,90
PTU B 8%
0,59
SUMA PTU
0,59
SUMA PLN
ROZLICZENIE P?ATNO?CI
GOTÓWKA
0,00 PLN
KARTA Mastercard
7,90 PLN
00009 #27 Niezalogowany 2020-12-24 11:34
E11C2F1C49B7FDA2DE602299957A7003A8200770
P EAZ 1901834151
nr rej. BDO: 000020518
7,90
Ocoz-r18a-sji
Nr sys. #27, Kodi 7412",
          new[] {
            new PurchaseInformation { Amount = 400, Description = "*Kurczakburger..." },
            new PurchaseInformation { Amount = 390, Description = "*Frytki Srednie" }
          }
        };
        yield return new object[] { @"McDonald's Polska Sp. 20.0.
Kielce 1 (137)
AL Solidarności 16
25-900 Kielce
NIP 521-008-81-10
2015-10-07 18:30
68
PARAGON FISKALNY
Grand BigMac M+ 1 ~12.90 12,90D
*FRYTKI NA
15.40 5.90D
COCA COLA M+
1 x1,50 1,50A
*McRoyal PodPikM+ 1 x12,60 12,60D
*FRYTKI M+
1 x5,00 5,900
COCA COLA M+
1 x1,50 1,50A
ChickenBigMac M+ 1 x10,90 10,90D
*FRYTKI M+
1 x5,00 5,000
COCA COLA M+
1 x1,50 1,50A
*WiesMadPodu
1 x12,60 12,60D
*McNuggets HM
1 x3,50 3,500
*Frytki HM
1 x3,60 3,60D
Zab Happy Meal 7
1 X2,60 2,60A
W.NieGazowana HM
1 x1,20 1,200
SPRZEDAZ OPODATKOWANA A
8,30
PIU A 23,00 %
1,55
SPRZEDAZ OPODATKOWANA D
73,80
PTU D 5,00 %
3,51
SUMO PHU
5,06
SUMA PLN
82,10
00015 #28 Niezalogowany 2015-10-07 18:30
16002889481110501767764F COAG5EB46709A188
CFC 150 1227932
Nr sys. #28
GOTOWKA
0.00 PLN
Karta Visa
82,10 PLN
",
          new[] {
            new PurchaseInformation { Amount = 1290, Description = "Grand BigMac M+" },
            new PurchaseInformation { Amount = 0150, Description = "*FRYTKI NA 15.40 5.90D COCA COLA M+" },
            new PurchaseInformation { Amount = 1260, Description = "*McRoyal PodPikM+"},
            new PurchaseInformation { Amount = 0500, Description = "*FRYTKI M+"},
            new PurchaseInformation { Amount = 0150, Description = "COCA COLA M+"},
            new PurchaseInformation { Amount = 1090, Description = "ChickenBigMac M+"},
            new PurchaseInformation { Amount = 0500, Description = "*FRYTKI M+"},
            new PurchaseInformation { Amount = 0150, Description = "COCA COLA M+"},
            new PurchaseInformation { Amount = 1260, Description = "*WiesMadPodu"},
            new PurchaseInformation { Amount = 0350, Description = "*McNuggets HM"},
            new PurchaseInformation { Amount = 0360, Description = "*Frytki HM"},
            new PurchaseInformation { Amount = 0260, Description = "Zab Happy Meal 7"},
            new PurchaseInformation { Amount = 0120, Description = "W.NieGazowana HM"}
          }
        };
        yield return new object[] { @"#6
IT IS A MLY
-001-
09221759
15:15
47
McDonald's Polska Sp. z 0.0.
able Zielona Góra 1 (84)
Al. Wojska Polskiego 23
65-077 Zielona Góra
NIP 521-008-81-10
2018-02-11
280504
PARAGON FISKALNY
*WrapChrup Klas 2 x6,00 12,00B
Jalapeno Burger
1 x4,00 4,00B
Fech?
SPRZEDAZ OPODATK. B
16,00
PTU B 8,00 %
1,19
SUMA PTU
1,19
SUMA PLN 16,00
00095 #6 -*001*-
15:15
82360A0OCBF5F9FCD1F 198615F9FBC5FAEC26360
E BAF 09221759
Nr sys. #6, Kod: 1234
GOTÓWKA
Karta Mastercard
0,00 PLN
16,00 PLN
02aq-asar-ben
IT",
        new[] {
            new PurchaseInformation { Amount = 600, Description = "*WrapChrup Klas" },
            new PurchaseInformation { Amount = 600, Description = "*WrapChrup Klas" },
            new PurchaseInformation { Amount = 400, Description = "Jalapeno Burger" }
        }
        };
      }
      IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
      }
    }
  }
}
