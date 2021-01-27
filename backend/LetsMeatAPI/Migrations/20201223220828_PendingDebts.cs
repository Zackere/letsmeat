using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LetsMeatAPI.Migrations {
  public partial class PendingDebts : Migration {
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropForeignKey(
          name: "FK_Images_Users_UploadedById",
          table: "Images");

      migrationBuilder.CreateTable(
          name: "PendingDebts",
          columns: table => new {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            FromId = table.Column<string>(type: "nvarchar(128)", nullable: false),
            ToId = table.Column<string>(type: "nvarchar(128)", nullable: false),
            GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Amount = table.Column<long>(type: "bigint", nullable: false),
            Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
            Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
            ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table => {
            table.PrimaryKey("PK_PendingDebts", x => x.Id);
            table.ForeignKey(
                      name: "FK_PendingDebts_Events_EventId",
                      column: x => x.EventId,
                      principalTable: "Events",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.SetNull);
            table.ForeignKey(
                      name: "FK_PendingDebts_Groups_GroupId",
                      column: x => x.GroupId,
                      principalTable: "Groups",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "FK_PendingDebts_Images_ImageId",
                      column: x => x.ImageId,
                      principalTable: "Images",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.SetNull);
            table.ForeignKey(
                      name: "FK_PendingDebts_Users_FromId",
                      column: x => x.FromId,
                      principalTable: "Users",
                      principalColumn: "Id");
            table.ForeignKey(
                      name: "FK_PendingDebts_Users_ToId",
                      column: x => x.ToId,
                      principalTable: "Users",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateIndex(
          name: "IX_PendingDebts_EventId",
          table: "PendingDebts",
          column: "EventId");

      migrationBuilder.CreateIndex(
          name: "IX_PendingDebts_FromId",
          table: "PendingDebts",
          column: "FromId");

      migrationBuilder.CreateIndex(
          name: "IX_PendingDebts_GroupId",
          table: "PendingDebts",
          column: "GroupId");

      migrationBuilder.CreateIndex(
          name: "IX_PendingDebts_ImageId",
          table: "PendingDebts",
          column: "ImageId");

      migrationBuilder.CreateIndex(
          name: "IX_PendingDebts_ToId",
          table: "PendingDebts",
          column: "ToId");

      migrationBuilder.AddForeignKey(
          name: "FK_Images_Users_UploadedById",
          table: "Images",
          column: "UploadedById",
          principalTable: "Users",
          principalColumn: "Id");
    }

    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropForeignKey(
          name: "FK_Images_Users_UploadedById",
          table: "Images");

      migrationBuilder.DropTable(
          name: "PendingDebts");

      migrationBuilder.AddForeignKey(
          name: "FK_Images_Users_UploadedById",
          table: "Images",
          column: "UploadedById",
          principalTable: "Users",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);
    }
  }
}
