using Google.Apis.Auth;
using LetsMeatAPI;
using LetsMeatAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LetsMeatAPITests {
  public class GroupsControllerTest : TestBase {
    public GroupsControllerTest(ITestOutputHelper output) : base(output) { }
    [Fact]
    public async Task UsersCanJoinOtherGroups() {
      var (context, connection) = GetDb();
      var userManager = new UserManager(
        context,
        Mock.Of<ILogger<UserManager>>()
      );
      var data = UsersWithTokens(643, 2).First();
      var token1 = (data[0] as object[])[0] as string;
      var jwt1 = (data[1] as object[])[0] as GoogleJsonWebSignature.Payload;
      var token2 = (data[0] as object[])[1] as string;
      var jwt2 = (data[1] as object[])[1] as GoogleJsonWebSignature.Payload;

      await Task.WhenAll(
        userManager.OnTokenGranted(token1, jwt1),
        userManager.OnTokenGranted(token2, jwt2)
      );

      var groupController = new GroupsController(
        userManager,
        context,
        Mock.Of<ILogger<GroupsController>>()
      );
      var grp = await groupController.Create(token2, new() { name = "ASD" });
      var response = grp.Value;
      await groupController.Join(token2, new() { id = response.id });

      Assert.Equal(1, context.Groups.Count());
      Assert.Equal(2, context.Groups.First().Users.Count());

      context.Dispose();
      connection.Dispose();
    }
    [Fact]
    public async Task UsersCanCreateGroups() {
      var (context, connection) = GetDb();
      var userManager = new UserManager(
        context,
        Mock.Of<ILogger<UserManager>>()
      );
      var data = UsersWithTokens(643, 1).First();
      var token1 = (data[0] as object[])[0] as string;
      var jwt1 = (data[1] as object[])[0] as GoogleJsonWebSignature.Payload;

      await userManager.OnTokenGranted(token1, jwt1);

      var groupController = new GroupsController(
        userManager,
        context,
        Mock.Of<ILogger<GroupsController>>()
      );
      var createRes = await groupController.Create(token1, new() { name = "ASD" });
      Assert.IsType<GroupsController.GroupCreatedResponse>(createRes.Value);
      var grpCreated = createRes.Value;
      Assert.Equal("ASD", grpCreated.name);
      var grp = context.Groups.Find(grpCreated.id);
      Assert.NotNull(grp);
      Assert.Equal(grpCreated.name, grp.Name);
      Assert.Equal(jwt1.Subject, grp.OwnerId);
      Assert.Collection(grp.Users, user => Assert.Equal(jwt1.Subject, user.Id));

      context.Dispose();
      connection.Dispose();
    }
    [Fact]
    public async Task UsersCanDeleteGroupTheyOwn() {
      var (context, connection) = GetDb();
      var userManager = new UserManager(
        context,
        Mock.Of<ILogger<UserManager>>()
      );
      var data = UsersWithTokens(643, 3).First();
      var token1 = (data[0] as object[])[0] as string;
      var jwt1 = (data[1] as object[])[0] as GoogleJsonWebSignature.Payload;
      var token2 = (data[0] as object[])[1] as string;
      var jwt2 = (data[1] as object[])[1] as GoogleJsonWebSignature.Payload;
      var token3 = (data[0] as object[])[2] as string;
      var jwt3 = (data[1] as object[])[2] as GoogleJsonWebSignature.Payload;

      await Task.WhenAll(
        userManager.OnTokenGranted(token1, jwt1),
        userManager.OnTokenGranted(token2, jwt2),
        userManager.OnTokenGranted(token3, jwt3)
      );
      Assert.Equal(3, context.Users.Count());

      var groupController = new GroupsController(
        userManager,
        context,
        Mock.Of<ILogger<GroupsController>>()
      );
      var invitationController = new InvitationsController(
        userManager,
        context,
        Mock.Of<ILogger<InvitationsController>>()
      );
      var eventsController = new EventsController(
        userManager,
        context,
        Mock.Of<ILogger<EventsController>>()
      );
      var debtsController = new DebtsController(
        userManager,
        context,
        Mock.Of<ILogger<DebtsController>>()
      );
      var locationsController = new LocationsController(
        userManager,
        context,
        Mock.Of<ILogger<LocationsController>>()
      );

      var grp = await groupController.Create(token1, new() { name = "ASD" });
      var groupCreatedResponse = grp.Value;
      await invitationController.Send(token1, new() {
        to_id = jwt2.Subject,
        group_id = groupCreatedResponse.id
      });
      await groupController.Join(token3, new() { id = groupCreatedResponse.id });
      var ev = await eventsController.Create(token1, new() {
        deadline = DateTime.Now,
        group_id = groupCreatedResponse.id,
        name = "ASD"
      });
      var eventCreatedResponse = ev.Value;
      Assert.Equal(1, context.Events.Count());
      var loc = await locationsController.CreateCustom(token1, new() {
        Address = "sesame street",
        group_id = groupCreatedResponse.id,
        Name = "mcdonalds",
      });
      Assert.Equal(1, context.CustomLocations.Count());
      var locationCreatedResponse = loc.Value;
      await eventsController.Update(token1, new EventsController.EventUpdateBody() {
        custom_locations_ids = new[] { locationCreatedResponse.id },
        id = eventCreatedResponse.id,
      });
      await debtsController.Add(token1, new() {
        amount = 20,
        group_id = groupCreatedResponse.id,
        from_id = jwt1.Subject,
        to_id = jwt3.Subject,
      });
      Assert.Equal(1, context.Debts.Count());
      var deleteRes = await groupController.Delete(token1, new() { id = groupCreatedResponse.id });
      Assert.IsType<OkResult>(deleteRes);

      Assert.Empty(context.CustomLocations);
      Assert.Empty(context.Debts);
      Assert.Empty(context.Events);
      Assert.Empty(context.Groups);
      var user1 = await context.Users.FindAsync(jwt1.Subject);
      Assert.Empty(user1.OwnedGroups);
      Assert.Empty(user1.Groups);
      Assert.Empty(context.Invitations);
      var user3 = await context.Users.FindAsync(jwt3.Subject);
      Assert.Empty(user3.Groups);

      context.Dispose();
      connection.Dispose();
    }
  }
}
