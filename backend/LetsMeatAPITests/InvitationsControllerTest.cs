using Google.Apis.Auth;
using LetsMeatAPI;
using LetsMeatAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LetsMeatAPITests {
  public class InvitationsControllerTest : TestBase {
    public InvitationsControllerTest(ITestOutputHelper output) : base(output) { }
    [Fact]
    public async Task UsersCanInviteOthersToGroupsTheyAreIn() {
      var (context, connection) = GetDb();
      var userManager = new LetsMeatAPI.UserManager(
        context,
        Mock.Of<ILogger<UserManager>>()
      );
      var data = UsersWithTokens(643, 2).First();
      var token1 = (data[0] as object[])[0] as string;
      var jwt1 = (data[1] as object[])[0] as GoogleJsonWebSignature.Payload;
      var token2 = (data[0] as object[])[1] as string;
      var jwt2 = (data[1] as object[])[1] as GoogleJsonWebSignature.Payload;

      await userManager.OnTokenGranted(token1, jwt1);
      await userManager.OnTokenGranted(token2, jwt2);

      var groupController = new GroupsController(
        userManager,
        context,
        Mock.Of<ILogger<GroupsController>>()
      );
      var grp = await groupController.Create(token2, new() { name = "ASD" });
      var invitationController = new InvitationsController(
        userManager,
        context,
        Mock.Of<ILogger<InvitationsController>>()
      );
      var groupCreatedResponse = (GroupsController.GroupCreatedResponse)((OkObjectResult)grp.Result).Value;
      await invitationController.Send(token2, new() {
        to_id = jwt1.Subject,
        group_id = groupCreatedResponse.id
      });

      Assert.Collection(context.Invitations, inv => Assert.Equal(jwt1.Subject, inv.ToId));
    }
  }
}
