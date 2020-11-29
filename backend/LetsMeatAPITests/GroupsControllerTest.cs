using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
      var userManager = new LetsMeatAPI.UserManager(
        context,
        Moq.Mock.Of<ILogger<LetsMeatAPI.UserManager>>()
      );
      var data = UsersWithTokens(643, 2).First();
      var token1 = (data[0] as object[])[0] as string;
      var jwt1 = (data[1] as object[])[0] as GoogleJsonWebSignature.Payload;
      var token2 = (data[0] as object[])[1] as string;
      var jwt2 = (data[1] as object[])[1] as GoogleJsonWebSignature.Payload;

      await userManager.OnTokenGranted(token1, jwt1);
      await userManager.OnTokenGranted(token2, jwt2);

      var groupController = new LetsMeatAPI.Controllers.GroupsController(
        userManager,
        context,
        Moq.Mock.Of<ILogger<LetsMeatAPI.Controllers.GroupsController>>()
      );
      var grp = await groupController.Create(token2, new() { name = "ASD" });
      var grpOk = grp.Result as OkObjectResult;
      var response = grpOk.Value as LetsMeatAPI.Controllers.GroupsController.GroupCreatedResponse;
      await groupController.Join(token2, new() { id = response.id });

      Assert.Equal(1, context.Groups.Count());
      Assert.Equal(2, context.Groups.First().Users.Count());

      context.Dispose();
      connection.Dispose();
    }
    [Fact]
    public async Task UsersCanCreateGroups() {
      var (context, connection) = GetDb();
      var userManager = new LetsMeatAPI.UserManager(
        context,
        Moq.Mock.Of<ILogger<LetsMeatAPI.UserManager>>()
      );
      var data = UsersWithTokens(643, 1).First();
      var token1 = (data[0] as object[])[0] as string;
      var jwt1 = (data[1] as object[])[0] as GoogleJsonWebSignature.Payload;

      await userManager.OnTokenGranted(token1, jwt1);

      var groupController = new LetsMeatAPI.Controllers.GroupsController(
        userManager,
        context,
        Moq.Mock.Of<ILogger<LetsMeatAPI.Controllers.GroupsController>>()
      );
      var createRes = await groupController.Create(token1, new() { name = "ASD" });
      Assert.IsType<OkObjectResult>(createRes.Result);
      var grpOk = createRes.Result as OkObjectResult;
      Assert.IsType<LetsMeatAPI.Controllers.GroupsController.GroupCreatedResponse>(
        grpOk.Value
      );
      var grpCreated = grpOk.Value as LetsMeatAPI.Controllers.GroupsController.GroupCreatedResponse;
      Assert.Equal("ASD", grpCreated.name);
      var grp = context.Groups.Find(grpCreated.id);
      Assert.NotNull(grp);
      Assert.Equal(jwt1.Subject, grp.OwnerId);
      Assert.Collection(context.Users, user => Assert.Equal(jwt1.Subject, user.Id));
    }
  }
}
