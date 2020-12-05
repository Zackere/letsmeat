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
      var grpOk = grp.Result as OkObjectResult;
      var response = grpOk.Value as GroupsController.GroupCreatedResponse;
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
      Assert.IsType<OkObjectResult>(createRes.Result);
      var grpOk = createRes.Result as OkObjectResult;
      Assert.IsType<GroupsController.GroupCreatedResponse>(
        grpOk.Value
      );
      var grpCreated = grpOk.Value as GroupsController.GroupCreatedResponse;
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

      var grp = await groupController.Create(token1, new() { name = "ASD" });
      var groupCreatedResponse = (GroupsController.GroupCreatedResponse)((OkObjectResult)grp.Result).Value;
      await invitationController.Send(token1, new() {
        to_id = jwt2.Subject,
        group_id = groupCreatedResponse.id
      });
      await groupController.Join(token3, new() { id = groupCreatedResponse.id });
      await eventsController.Create(token1, new() {
        deadline = DateTime.Now,
        group_id = groupCreatedResponse.id,
        name = "ASD"
      });
      var deleteRes = await groupController.Delete(token1, new() { id = groupCreatedResponse.id });
      Assert.IsType<OkResult>(deleteRes);

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
