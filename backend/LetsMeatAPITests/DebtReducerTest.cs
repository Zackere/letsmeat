using LetsMeatAPI;
using LetsMeatAPI.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LetsMeatAPITests {
  public class DebtReducerTest : TestBase {
    public DebtReducerTest(ITestOutputHelper output) : base(output) { }
    [Fact]
    public async Task CyclesAreBeingReduced() {
      var connection = GetDb();
      var (users, group, events, customLocations, googleMapsLocations, invitations, images, debts, pendingDebts)
        = await SeedDbWithOneGroup(connection);
      using(var setupContext = CreateContextForConnection(connection)) {
        setupContext.RemoveRange(debts);
        var userIds = from user in users
                      where @group.Users.Contains(user)
                      orderby user.Id
                      select user.Id;
        userIds = userIds.Append(userIds.First());
        foreach(var (v, w) in userIds.Zip(userIds.Skip(1))) {
          setupContext.Debts.Add(new Debt {
            Amount = 100,
            FromId = v,
            ToId = w,
            GroupId = group.Id,
          });
        }
        await setupContext.SaveChangesAsync();
      }
      using(var context = CreateContextForConnection(connection)) {
        var debtReducer = new DebtReducer(
          context,
          Mock.Of<ILogger<DebtReducer>>()
        );
        await debtReducer.ReduceDebts(group.Id);
      }
      using var verifyContext = CreateContextForConnection(connection);
      Assert.Empty(verifyContext.Debts);
    }
    [Fact]
    public async Task PathsAreBeingReduced() {
      var connection = GetDb();
      var (users, group, events, customLocations, googleMapsLocations, invitations, images, debts, pendingDebts)
        = await SeedDbWithOneGroup(connection);
      using(var setupContext = CreateContextForConnection(connection)) {
        setupContext.RemoveRange(debts);
        var userIds = from user in users
                      where @group.Users.Contains(user)
                      orderby user.Id
                      select user.Id;
        foreach(var (v, w) in userIds.Zip(userIds.Skip(1))) {
          setupContext.Debts.Add(new Debt {
            Amount = 100,
            FromId = v,
            ToId = w,
            GroupId = group.Id,
          });
        }
        await setupContext.SaveChangesAsync();
      }
      using(var context = CreateContextForConnection(connection)) {
        var debtReducer = new DebtReducer(
          context,
          Mock.Of<ILogger<DebtReducer>>()
        );
        await debtReducer.ReduceDebts(group.Id);
      }
      using var verifyContext = CreateContextForConnection(connection);
      Assert.Equal(1, verifyContext.Debts.Count());
    }
  }
}
