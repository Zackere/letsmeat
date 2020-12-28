using LetsMeatAPI;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace LetsMeatAPITests {
  public class ElectionHolderTest : TestBase {
    public ElectionHolderTest(ITestOutputHelper output) : base(output) { }
    [Fact]
    public void RanksCandidatesByTheirPopularity1() {
      var votes = new[] {
        new[] { 0, 1, 2 },
        new[] { 0, 1, 2 },
        new[] { 1, 0, 2 },
      };
      var order = new ElectionHolder().DecideWinner(3, votes).ToArray();
      Assert.Equal(new[] { 0, 1, 2 }, order);
    }
  }
}
