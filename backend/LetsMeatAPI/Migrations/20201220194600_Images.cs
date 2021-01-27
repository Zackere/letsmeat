using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LetsMeatAPI.Migrations {
  public partial class Images : Migration {
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.CreateTable(
          name: "Images",
          columns: table => new {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Url = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
            EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UploadedById = table.Column<string>(type: "nvarchar(128)", nullable: false),
            UploadTime = table.Column<DateTime>(type: "datetime2", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_Images", x => x.Id);
            table.ForeignKey(
                      name: "FK_Images_Events_EventId",
                      column: x => x.EventId,
                      principalTable: "Events",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.SetNull);
            table.ForeignKey(
                      name: "FK_Images_Groups_GroupId",
                      column: x => x.GroupId,
                      principalTable: "Groups",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "FK_Images_Users_UploadedById",
                      column: x => x.UploadedById,
                      principalTable: "Users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_Images_EventId",
          table: "Images",
          column: "EventId");

      migrationBuilder.CreateIndex(
          name: "IX_Images_GroupId",
          table: "Images",
          column: "GroupId");

      migrationBuilder.CreateIndex(
          name: "IX_Images_UploadedById",
          table: "Images",
          column: "UploadedById");
    }

    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropTable(
          name: "Images");
    }
  }
}
