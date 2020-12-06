using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LetsMeatAPI.Migrations {
  public partial class InitialCreate : Migration {
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.CreateTable(
          name: "GoogleMapsLocations",
          columns: table => new {
            Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Latitude = table.Column<float>(type: "real", nullable: false),
            Longitude = table.Column<float>(type: "real", nullable: false),
            Rating = table.Column<string>(type: "nvarchar(max)", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_GoogleMapsLocations", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Users",
          columns: table => new {
            Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            PictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Prefs = table.Column<string>(type: "nvarchar(max)", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_Users", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Groups",
          columns: table => new {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
            OwnerId = table.Column<string>(type: "nvarchar(128)", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_Groups", x => x.Id);
            table.ForeignKey(
                      name: "FK_Groups_Users_OwnerId",
                      column: x => x.OwnerId,
                      principalTable: "Users",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "CustomLocations",
          columns: table => new {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            CreatedForId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Address = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
            Rating = table.Column<string>(type: "nvarchar(max)", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_CustomLocations", x => x.Id);
            table.ForeignKey(
                      name: "FK_CustomLocations_Groups_CreatedForId",
                      column: x => x.CreatedForId,
                      principalTable: "Groups",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "Debts",
          columns: table => new {
            FromId = table.Column<string>(type: "nvarchar(128)", nullable: false),
            ToId = table.Column<string>(type: "nvarchar(128)", nullable: false),
            GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Amount = table.Column<int>(type: "int", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_Debts", x => new { x.FromId, x.ToId, x.GroupId });
            table.ForeignKey(
                      name: "FK_Debts_Groups_GroupId",
                      column: x => x.GroupId,
                      principalTable: "Groups",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_Debts_Users_FromId",
                      column: x => x.FromId,
                      principalTable: "Users",
                      principalColumn: "Id");
            table.ForeignKey(
                      name: "FK_Debts_Users_ToId",
                      column: x => x.ToId,
                      principalTable: "Users",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "Events",
          columns: table => new {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            CreatorId = table.Column<string>(type: "nvarchar(128)", nullable: false),
            Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
            CandidateTimes = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
            Result = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table => {
            table.PrimaryKey("PK_Events", x => x.Id);
            table.ForeignKey(
                      name: "FK_Events_Groups_GroupId",
                      column: x => x.GroupId,
                      principalTable: "Groups",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_Events_Users_CreatorId",
                      column: x => x.CreatorId,
                      principalTable: "Users",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "GroupUser",
          columns: table => new {
            GroupsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UsersId = table.Column<string>(type: "nvarchar(128)", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_GroupUser", x => new { x.GroupsId, x.UsersId });
            table.ForeignKey(
                      name: "FK_GroupUser_Groups_GroupsId",
                      column: x => x.GroupsId,
                      principalTable: "Groups",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_GroupUser_Users_UsersId",
                      column: x => x.UsersId,
                      principalTable: "Users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Invitations",
          columns: table => new {
            ToId = table.Column<string>(type: "nvarchar(128)", nullable: false),
            GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            FromId = table.Column<string>(type: "nvarchar(128)", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_Invitations", x => new { x.ToId, x.GroupId });
            table.ForeignKey(
                      name: "FK_Invitations_Groups_GroupId",
                      column: x => x.GroupId,
                      principalTable: "Groups",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_Invitations_Users_FromId",
                      column: x => x.FromId,
                      principalTable: "Users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_Invitations_Users_ToId",
                      column: x => x.ToId,
                      principalTable: "Users",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "CustomLocationEvent",
          columns: table => new {
            CandidateCustomLocationsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            EventsWithMeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_CustomLocationEvent", x => new { x.CandidateCustomLocationsId, x.EventsWithMeId });
            table.ForeignKey(
                      name: "FK_CustomLocationEvent_CustomLocations_CandidateCustomLocationsId",
                      column: x => x.CandidateCustomLocationsId,
                      principalTable: "CustomLocations",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_CustomLocationEvent_Events_EventsWithMeId",
                      column: x => x.EventsWithMeId,
                      principalTable: "Events",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EventGoogleMapsLocation",
          columns: table => new {
            CandidateGoogleMapsLocationsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            EventsWithMeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_EventGoogleMapsLocation", x => new { x.CandidateGoogleMapsLocationsId, x.EventsWithMeId });
            table.ForeignKey(
                      name: "FK_EventGoogleMapsLocation_Events_EventsWithMeId",
                      column: x => x.EventsWithMeId,
                      principalTable: "Events",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_EventGoogleMapsLocation_GoogleMapsLocations_CandidateGoogleMapsLocationsId",
                      column: x => x.CandidateGoogleMapsLocationsId,
                      principalTable: "GoogleMapsLocations",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Votes",
          columns: table => new {
            EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            Order = table.Column<string>(type: "nvarchar(max)", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_Votes", x => new { x.EventId, x.UserId });
            table.ForeignKey(
                      name: "FK_Votes_Events_EventId",
                      column: x => x.EventId,
                      principalTable: "Events",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_CustomLocationEvent_EventsWithMeId",
          table: "CustomLocationEvent",
          column: "EventsWithMeId");

      migrationBuilder.CreateIndex(
          name: "IX_CustomLocations_CreatedForId",
          table: "CustomLocations",
          column: "CreatedForId");

      migrationBuilder.CreateIndex(
          name: "IX_Debts_GroupId",
          table: "Debts",
          column: "GroupId");

      migrationBuilder.CreateIndex(
          name: "IX_Debts_ToId",
          table: "Debts",
          column: "ToId");

      migrationBuilder.CreateIndex(
          name: "IX_EventGoogleMapsLocation_EventsWithMeId",
          table: "EventGoogleMapsLocation",
          column: "EventsWithMeId");

      migrationBuilder.CreateIndex(
          name: "IX_Events_CreatorId",
          table: "Events",
          column: "CreatorId");

      migrationBuilder.CreateIndex(
          name: "IX_Events_GroupId",
          table: "Events",
          column: "GroupId");

      migrationBuilder.CreateIndex(
          name: "IX_Groups_OwnerId",
          table: "Groups",
          column: "OwnerId");

      migrationBuilder.CreateIndex(
          name: "IX_GroupUser_UsersId",
          table: "GroupUser",
          column: "UsersId");

      migrationBuilder.CreateIndex(
          name: "IX_Invitations_FromId",
          table: "Invitations",
          column: "FromId");

      migrationBuilder.CreateIndex(
          name: "IX_Invitations_GroupId",
          table: "Invitations",
          column: "GroupId");
    }

    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropTable(
          name: "CustomLocationEvent");

      migrationBuilder.DropTable(
          name: "Debts");

      migrationBuilder.DropTable(
          name: "EventGoogleMapsLocation");

      migrationBuilder.DropTable(
          name: "GroupUser");

      migrationBuilder.DropTable(
          name: "Invitations");

      migrationBuilder.DropTable(
          name: "Votes");

      migrationBuilder.DropTable(
          name: "CustomLocations");

      migrationBuilder.DropTable(
          name: "GoogleMapsLocations");

      migrationBuilder.DropTable(
          name: "Events");

      migrationBuilder.DropTable(
          name: "Groups");

      migrationBuilder.DropTable(
          name: "Users");
    }
  }
}
