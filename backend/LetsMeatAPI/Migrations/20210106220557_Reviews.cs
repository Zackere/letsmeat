using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LetsMeatAPI.Migrations {
  public partial class Reviews : Migration {
    protected override void Up(MigrationBuilder migrationBuilder) {
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

      migrationBuilder.RenameColumn(
          name: "WaitingTimeVotes",
          table: "GoogleMapsLocations",
          newName: "UserRatingsTotal");

      migrationBuilder.AddColumn<int>(
          name: "PriceLevel",
          table: "GoogleMapsLocations",
          type: "int",
          nullable: false,
          defaultValue: 0);

      migrationBuilder.AddColumn<double>(
          name: "Rating",
          table: "GoogleMapsLocations",
          type: "float",
          nullable: false,
          defaultValue: 0.0);

      migrationBuilder.CreateTable(
          name: "CustomLocationReviews",
          columns: table => new {
            UserId = table.Column<string>(type: "nvarchar(128)", nullable: false),
            CustomLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Taste = table.Column<int>(type: "int", nullable: false),
            Price = table.Column<int>(type: "int", nullable: false),
            AmountOfFood = table.Column<int>(type: "int", nullable: false),
            WaitingTime = table.Column<int>(type: "int", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_CustomLocationReviews", x => new { x.CustomLocationId, x.UserId });
            table.ForeignKey(
                      name: "FK_CustomLocationReviews_CustomLocations_CustomLocationId",
                      column: x => x.CustomLocationId,
                      principalTable: "CustomLocations",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_CustomLocationReviews_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "GoogleMapsLocationReviews",
          columns: table => new {
            UserId = table.Column<string>(type: "nvarchar(128)", nullable: false),
            GoogleMapsLocationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            Taste = table.Column<int>(type: "int", nullable: false),
            Price = table.Column<int>(type: "int", nullable: false),
            AmountOfFood = table.Column<int>(type: "int", nullable: false),
            WaitingTime = table.Column<int>(type: "int", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_GoogleMapsLocationReviews", x => new { x.GoogleMapsLocationId, x.UserId });
            table.ForeignKey(
                      name: "FK_GoogleMapsLocationReviews_GoogleMapsLocations_GoogleMapsLocationId",
                      column: x => x.GoogleMapsLocationId,
                      principalTable: "GoogleMapsLocations",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_GoogleMapsLocationReviews_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_CustomLocationReviews_UserId",
          table: "CustomLocationReviews",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_GoogleMapsLocationReviews_UserId",
          table: "GoogleMapsLocationReviews",
          column: "UserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropTable(
          name: "CustomLocationReviews");

      migrationBuilder.DropTable(
          name: "GoogleMapsLocationReviews");

      migrationBuilder.DropColumn(
          name: "PriceLevel",
          table: "GoogleMapsLocations");

      migrationBuilder.DropColumn(
          name: "Rating",
          table: "GoogleMapsLocations");

      migrationBuilder.RenameColumn(
          name: "UserRatingsTotal",
          table: "GoogleMapsLocations",
          newName: "WaitingTimeVotes");

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
  }
}
