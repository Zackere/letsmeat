using LetsMeatAPI;
using LetsMeatAPI.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LetsMeatAPITests {
  public class DebtsControllerTest : TestBase {
    public DebtsControllerTest(ITestOutputHelper output) : base(output) { }
    [Fact]
    public async Task AddsDebtsFromTrasfersToPending() {
      var connection = GetDb();
      var (
        users,
        group,
        events,
        customLocations,
        googleMapsLocations,
        invitations,
        images
        ) = await SeedDbWithOneGroupWithoutDebts(connection);
      var userManager = UserManagerMock(users);
      using(var context = CreateContextForConnection(connection)) {
        var controller = new DebtsController(
          userManager.Object,
          context,
          Mock.Of<IDebtReducer>(),
          Mock.Of<ILogger<DebtsController>>()
        );
        await controller.Add(users[0].Token, new() {
          amount = 100,
          debt_type = DebtsController.DebtType.Transfer,
          description = RandomString(new(67231), 24),
          event_id = events[0].Id,
          from_id = users[1].Id,
          to_id = users[0].Id,
          image_debt_id = null,
        });
      }
      using var contextVerify = CreateContextForConnection(connection);
      Assert.Collection(contextVerify.PendingDebts,
        e => {
          Assert.Equal(100u, e.Amount);
          Assert.Null(e.Bound);
          Assert.Equal((int)DebtsController.DebtType.Transfer, e.Type);
          Assert.Equal(RandomString(new(67231), 24), e.Description);
          Assert.Equal(events[0].Id, e.EventId);
          Assert.Equal(users[1].Id, e.FromId);
          Assert.Equal(users[0].Id, e.ToId);
          Assert.Null(e.ImageId);
        }
      );
      Assert.Empty(contextVerify.Debts);
      Assert.Empty(contextVerify.DebtHistory);
      Assert.Empty(contextVerify.DebtsFromImages);
      Assert.Empty(contextVerify.PendingDebtFromImageBounds);
    }
    [Fact]
    public async Task DebtsOnSenderAreApprovedImmediatelyWithoutSignChange() {
      var connection = GetDb();
      var (
        users,
        group,
        events,
        customLocations,
        googleMapsLocations,
        invitations,
        images
        ) = await SeedDbWithOneGroupWithoutDebts(connection);
      Assert.True(users[0].Id.CompareTo(users[1].Id) > 0);
      var userManager = UserManagerMock(users);
      using(var context = CreateContextForConnection(connection)) {
        var controller = new DebtsController(
          userManager.Object,
          context,
          Mock.Of<IDebtReducer>(),
          Mock.Of<ILogger<DebtsController>>()
        );
        await controller.Add(users[0].Token, new() {
          amount = 100,
          debt_type = DebtsController.DebtType.Transfer,
          description = RandomString(new(67231), 24),
          event_id = events[0].Id,
          from_id = users[0].Id,
          to_id = users[1].Id,
          image_debt_id = null,
        });
      }
      using var contextVerify = CreateContextForConnection(connection);
      Assert.Collection(contextVerify.Debts,
        e => {
          Assert.Equal(100, e.Amount);
          Assert.Equal(users[0].Id, e.FromId);
          Assert.Equal(users[1].Id, e.ToId);
        }
      );
      Assert.Collection(contextVerify.DebtHistory,
        e => {
          Assert.Equal(100u, e.Amount);
        }
      );
      Assert.Empty(contextVerify.PendingDebts);
      Assert.Empty(contextVerify.DebtsFromImages);
      Assert.Empty(contextVerify.PendingDebtFromImageBounds);
    }
    [Fact]
    public async Task DebtsOnSenderAreApprovedImmediatelyWithSignChange() {
      var connection = GetDb();
      var (
        users,
        group,
        events,
        customLocations,
        googleMapsLocations,
        invitations,
        images
        ) = await SeedDbWithOneGroupWithoutDebts(connection);
      Assert.True(users[0].Id.CompareTo(users[1].Id) > 0);
      var userManager = UserManagerMock(users);
      using(var context = CreateContextForConnection(connection)) {
        var controller = new DebtsController(
          userManager.Object,
          context,
          Mock.Of<IDebtReducer>(),
          Mock.Of<ILogger<DebtsController>>()
        );
        await controller.Add(users[1].Token, new() {
          amount = 100,
          debt_type = DebtsController.DebtType.Transfer,
          description = RandomString(new(67231), 24),
          event_id = events[0].Id,
          from_id = users[1].Id,
          to_id = users[0].Id,
          image_debt_id = null,
        });
      }
      using var contextVerify = CreateContextForConnection(connection);
      Assert.Collection(contextVerify.Debts,
        e => {
          Assert.Equal(-100, e.Amount);
          Assert.Equal(users[0].Id, e.FromId);
          Assert.Equal(users[1].Id, e.ToId);
        }
      );
      Assert.Collection(contextVerify.DebtHistory,
        e => {
          Assert.Equal(100u, e.Amount);
          Assert.Equal(users[1].Id, e.FromId);
          Assert.Equal(users[0].Id, e.ToId);
        }
      );
      Assert.Empty(contextVerify.PendingDebts);
      Assert.Empty(contextVerify.DebtsFromImages);
      Assert.Empty(contextVerify.PendingDebtFromImageBounds);
    }
  }
}
