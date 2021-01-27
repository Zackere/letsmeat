using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsMeatAPI.Migrations {
  public partial class Prefs : Migration {
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropColumn(
          name: "Prefs",
          table: "Users");

      migrationBuilder.AddColumn<int>(
          name: "AmountOfFoodPref",
          table: "Users",
          type: "int",
          nullable: false,
          defaultValue: 50);

      migrationBuilder.AddColumn<int>(
          name: "PricePref",
          table: "Users",
          type: "int",
          nullable: false,
          defaultValue: 50);

      migrationBuilder.AddColumn<int>(
          name: "TastePref",
          table: "Users",
          type: "int",
          nullable: false,
          defaultValue: 50);

      migrationBuilder.AddColumn<int>(
          name: "WaitingTimePref",
          table: "Users",
          type: "int",
          nullable: false,
          defaultValue: 50);
    }

    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropColumn(
          name: "AmountOfFoodPref",
          table: "Users");

      migrationBuilder.DropColumn(
          name: "PricePref",
          table: "Users");

      migrationBuilder.DropColumn(
          name: "TastePref",
          table: "Users");

      migrationBuilder.DropColumn(
          name: "WaitingTimePref",
          table: "Users");

      migrationBuilder.AddColumn<string>(
          name: "Prefs",
          table: "Users",
          type: "nvarchar(max)",
          nullable: false,
          defaultValue: "{}");
    }
  }
}
