using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsMeatAPI.Migrations {
  public partial class LocationsRating : Migration {
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropColumn(
          name: "Rating",
          table: "GoogleMapsLocations");

      migrationBuilder.DropColumn(
          name: "Rating",
          table: "CustomLocations");

      migrationBuilder.AddColumn<decimal>(
          name: "AmountOfFood",
          table: "GoogleMapsLocations",
          type: "decimal(20,0)",
          nullable: false,
          defaultValue: 0m);

      migrationBuilder.AddColumn<decimal>(
          name: "AmountOfFoodVotes",
          table: "GoogleMapsLocations",
          type: "decimal(20,0)",
          nullable: false,
          defaultValue: 0m);

      migrationBuilder.AddColumn<decimal>(
          name: "Price",
          table: "GoogleMapsLocations",
          type: "decimal(20,0)",
          nullable: false,
          defaultValue: 0m);

      migrationBuilder.AddColumn<decimal>(
          name: "PriceVotes",
          table: "GoogleMapsLocations",
          type: "decimal(20,0)",
          nullable: false,
          defaultValue: 0m);

      migrationBuilder.AddColumn<decimal>(
          name: "Taste",
          table: "GoogleMapsLocations",
          type: "decimal(20,0)",
          nullable: false,
          defaultValue: 0m);

      migrationBuilder.AddColumn<decimal>(
          name: "TasteVotes",
          table: "GoogleMapsLocations",
          type: "decimal(20,0)",
          nullable: false,
          defaultValue: 0m);

      migrationBuilder.AddColumn<decimal>(
          name: "WaitingTime",
          table: "GoogleMapsLocations",
          type: "decimal(20,0)",
          nullable: false,
          defaultValue: 0m);

      migrationBuilder.AddColumn<decimal>(
          name: "WaitingTimeVotes",
          table: "GoogleMapsLocations",
          type: "decimal(20,0)",
          nullable: false,
          defaultValue: 0m);

      migrationBuilder.AddColumn<decimal>(
          name: "AmountOfFood",
          table: "CustomLocations",
          type: "decimal(20,0)",
          nullable: false,
          defaultValue: 0m);

      migrationBuilder.AddColumn<decimal>(
          name: "AmountOfFoodVotes",
          table: "CustomLocations",
          type: "decimal(20,0)",
          nullable: false,
          defaultValue: 0m);

      migrationBuilder.AddColumn<decimal>(
          name: "Price",
          table: "CustomLocations",
          type: "decimal(20,0)",
          nullable: false,
          defaultValue: 0m);

      migrationBuilder.AddColumn<decimal>(
          name: "PriceVotes",
          table: "CustomLocations",
          type: "decimal(20,0)",
          nullable: false,
          defaultValue: 0m);

      migrationBuilder.AddColumn<decimal>(
          name: "Taste",
          table: "CustomLocations",
          type: "decimal(20,0)",
          nullable: false,
          defaultValue: 0m);

      migrationBuilder.AddColumn<decimal>(
          name: "TasteVotes",
          table: "CustomLocations",
          type: "decimal(20,0)",
          nullable: false,
          defaultValue: 0m);

      migrationBuilder.AddColumn<decimal>(
          name: "WaitingTime",
          table: "CustomLocations",
          type: "decimal(20,0)",
          nullable: false,
          defaultValue: 0m);

      migrationBuilder.AddColumn<decimal>(
          name: "WaitingTimeVotes",
          table: "CustomLocations",
          type: "decimal(20,0)",
          nullable: false,
          defaultValue: 0m);
    }

    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropColumn(
          name: "AmountOfFood",
          table: "GoogleMapsLocations");

      migrationBuilder.DropColumn(
          name: "AmountOfFoodVotes",
          table: "GoogleMapsLocations");

      migrationBuilder.DropColumn(
          name: "Price",
          table: "GoogleMapsLocations");

      migrationBuilder.DropColumn(
          name: "PriceVotes",
          table: "GoogleMapsLocations");

      migrationBuilder.DropColumn(
          name: "Taste",
          table: "GoogleMapsLocations");

      migrationBuilder.DropColumn(
          name: "TasteVotes",
          table: "GoogleMapsLocations");

      migrationBuilder.DropColumn(
          name: "WaitingTime",
          table: "GoogleMapsLocations");

      migrationBuilder.DropColumn(
          name: "WaitingTimeVotes",
          table: "GoogleMapsLocations");

      migrationBuilder.DropColumn(
          name: "AmountOfFood",
          table: "CustomLocations");

      migrationBuilder.DropColumn(
          name: "AmountOfFoodVotes",
          table: "CustomLocations");

      migrationBuilder.DropColumn(
          name: "Price",
          table: "CustomLocations");

      migrationBuilder.DropColumn(
          name: "PriceVotes",
          table: "CustomLocations");

      migrationBuilder.DropColumn(
          name: "Taste",
          table: "CustomLocations");

      migrationBuilder.DropColumn(
          name: "TasteVotes",
          table: "CustomLocations");

      migrationBuilder.DropColumn(
          name: "WaitingTime",
          table: "CustomLocations");

      migrationBuilder.DropColumn(
          name: "WaitingTimeVotes",
          table: "CustomLocations");

      migrationBuilder.AddColumn<string>(
          name: "Rating",
          table: "GoogleMapsLocations",
          type: "nvarchar(max)",
          nullable: false,
          defaultValue: "");

      migrationBuilder.AddColumn<string>(
          name: "Rating",
          table: "CustomLocations",
          type: "nvarchar(max)",
          nullable: false,
          defaultValue: "");
    }
  }
}
