using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsMeatAPI.Migrations {
  public partial class GroupOwner : Migration {
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.AddColumn<string>(
          name: "OwnerId",
          table: "Groups",
          type: "nvarchar(128)",
          nullable: false,
          defaultValue: "");

      migrationBuilder.CreateIndex(
          name: "IX_Groups_OwnerId",
          table: "Groups",
          column: "OwnerId");

      migrationBuilder.AddForeignKey(
          name: "FK_Groups_Users_OwnerId",
          table: "Groups",
          column: "OwnerId",
          principalTable: "Users",
          principalColumn: "Id");
    }

    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropForeignKey(
          name: "FK_Groups_Users_OwnerId",
          table: "Groups");

      migrationBuilder.DropIndex(
          name: "IX_Groups_OwnerId",
          table: "Groups");

      migrationBuilder.DropColumn(
          name: "OwnerId",
          table: "Groups");
    }
  }
}
