using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsMeatAPI.Migrations {
  public partial class LoginStatus : Migration {
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.AddColumn<string>(
          name: "Token",
          table: "Users",
          type: "nvarchar(128)",
          maxLength: 128,
          nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropColumn(
          name: "Token",
          table: "Users");
    }
  }
}
