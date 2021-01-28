using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using LetsMeatAPI;
using LetsMeatAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LetsMeatAPITests {
  public class GroupsControllerTest : TestBase {
    public GroupsControllerTest(ITestOutputHelper output) : base(output) { }
    [Fact]
    public async Task UsersCanJoinOtherGroups() {
      var connection = GetDb();
      var users = CreateUsers(777, 2);
      var group = CreateGroups(890, 1)[0];
      group.OwnerId = users[0].Id;
      group.Users.Add(users[0]);
      using(var contextSetup = CreateContextForConnection(connection)) {
        await contextSetup.Users.AddRangeAsync(users);
        await contextSetup.Groups.AddAsync(group);
        await contextSetup.SaveChangesAsync();
      }
      var userManager = UserManagerMock(users);
      using(var context = CreateContextForConnection(connection)) {
        var controller = new GroupsController(
          userManager.Object,
          context,
          Mock.Of<IBlobClientFactory>(),
          Mock.Of<ILogger<GroupsController>>()
        );
        var res = await controller.Join(users[1].Token, new() { id = group.Id });
        Assert.IsType<OkResult>(res);
      }
      userManager.Verify(m => m.IsLoggedIn(users[1].Token), Times.Once);
      using var contextVerify = CreateContextForConnection(connection);
      group = await contextVerify.Groups.FindAsync(group.Id);
      Assert.Equal(
        group.Users.Select(u => u.Id).OrderBy(i => i).ToArray(),
        users.Select(u => u.Id).OrderBy(i => i).ToArray()
      );
    }
    [Fact]
    public async Task UsersCanCreateGroups() {
      var connection = GetDb();
      var user = CreateUsers(564, 1)[0];
      using(var contextSetup = CreateContextForConnection(connection)) {
        await contextSetup.AddAsync(user);
        await contextSetup.SaveChangesAsync();
      }
      var userManager = UserManagerMock(new[] { user });
      GroupsController.GroupCreatedResponse groupResponse;
      using(var context = CreateContextForConnection(connection)) {
        var controller = new GroupsController(
          userManager.Object,
          context,
          Mock.Of<IBlobClientFactory>(),
          Mock.Of<ILogger<GroupsController>>()
        );
        var res = await controller.Create(user.Token, new() {
          name = "dhushdw"
        });
        groupResponse = res.Value;
      }
      Assert.NotNull(groupResponse);
      userManager.Verify(m => m.IsLoggedIn(user.Token), Times.Once);
      using var contextVerify = CreateContextForConnection(connection);
      var group = await contextVerify.Groups.FindAsync(groupResponse.id);
      Assert.NotNull(group);
      Assert.Equal("dhushdw", group.Name);
      Assert.Equal(user.Id, group.OwnerId);
      Assert.Equal(new[] { user.Id }, group.Users.Select(u => u.Id).ToArray());
    }
    [Fact]
    public async Task UsersCanDeleteGroupTheyOwn() {
      var connection = GetDb();
      var (users, group, events, customLocations, googleMapsLocations, invitations, images, debts, pendingDebts)
        = await SeedDbWithOneGroup(connection);
      var userManager = UserManagerMock(users);
      var blobClient = new Mock<BlobClient>(MockBehavior.Strict);
      var blobClientFactory = new Mock<IBlobClientFactory>(MockBehavior.Strict);
      foreach(var image in images) {
        blobClientFactory
          .Setup(b => b.GetClientFromUri(new Uri(image.Url)))
          .Returns(blobClient.Object);
        blobClient
          .Setup(b => b.DeleteIfExistsAsync(
            DeleteSnapshotsOption.IncludeSnapshots,
            null,
            It.IsAny<CancellationToken>()))
          .Returns(Task.FromResult(Mock.Of<Azure.Response<bool>>()));
      }
      using(var context = CreateContextForConnection(connection)) {
        var controller = new GroupsController(
          userManager.Object,
          context,
          blobClientFactory.Object,
          Mock.Of<ILogger<GroupsController>>()
        );
        var res = await controller.Delete(users[0].Token, new() { id = group.Id });
        Assert.IsType<OkResult>(res);
      }
      userManager.Verify(m => m.IsLoggedIn(users[0].Token), Times.Once);
      foreach(var image in images) {
        blobClientFactory.Verify(b => b.GetClientFromUri(new Uri(image.Url)), Times.Once);
      }
      blobClient.Verify(b => b.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, null, default), Times.Exactly(images.Count()));
      using var contextVerify = CreateContextForConnection(connection);
      Assert.Empty(contextVerify.CustomLocations);
      Assert.NotEmpty(contextVerify.GoogleMapsLocations);
      Assert.Empty(contextVerify.Groups);
      Assert.Empty(contextVerify.Images);
      Assert.Empty(contextVerify.Invitations);
      Assert.Empty(contextVerify.PendingDebts);
      Assert.NotEmpty(contextVerify.Users);
      Assert.Empty(contextVerify.Votes);
    }
    [Fact]
    public async Task UsersCanLeaveGroupsTheyAreIn() {
      var connection = GetDb();
      var (users, group, events, customLocations, googleMapsLocations, invitations, images, debts, pendingDebts)
        = await SeedDbWithOneGroup(connection);
      using(var cc = CreateContextForConnection(connection)) {
        cc.RemoveRange(cc.Debts);
        await cc.SaveChangesAsync();
      }
      var userManager = UserManagerMock(users);
      var blobClientFactory = new Mock<IBlobClientFactory>(MockBehavior.Strict);
      using(var context = CreateContextForConnection(connection)) {
        var controller = new GroupsController(
          userManager.Object,
          context,
          blobClientFactory.Object,
          Mock.Of<ILogger<GroupsController>>()
        );
        var res = await controller.Leave(users[1].Token, new() { id = group.Id });
        Assert.IsType<OkResult>(res);
      }
      userManager.Verify(m => m.IsLoggedIn(users[1].Token), Times.Once);
      var contextVerify = CreateContextForConnection(connection);
      var grp = await contextVerify.Groups.FindAsync(group.Id);
      Assert.Equal(
        new[] { users[0].Id, users[2].Id }.OrderBy(i => i),
        (from user in grp.Users
         orderby user.Id
         select user.Id).ToArray()
      );
    }
  }
}
