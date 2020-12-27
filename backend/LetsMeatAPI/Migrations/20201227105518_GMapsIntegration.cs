using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LetsMeatAPI.Migrations {
  public partial class GMapsIntegration : Migration {
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropColumn(
          name: "Latitude",
          table: "GoogleMapsLocations");

      migrationBuilder.DropColumn(
          name: "Longitude",
          table: "GoogleMapsLocations");

      migrationBuilder.DropColumn(
          name: "Address",
          table: "GoogleMapsLocations");

      migrationBuilder.AddColumn<string>(
          name: "BusinessStatus",
          table: "GoogleMapsLocations",
          type: "nvarchar(max)",
          nullable: true);

      migrationBuilder.AddColumn<DateTime>(
          name: "ExpirationDate",
          table: "GoogleMapsLocations",
          type: "datetime2",
          nullable: false,
          defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

      migrationBuilder.AddColumn<string>(
          name: "FormattedAddress",
          table: "GoogleMapsLocations",
          type: "nvarchar(max)",
          nullable: false,
          defaultValue: "");

      migrationBuilder.AddColumn<string>(
          name: "Icon",
          table: "GoogleMapsLocations",
          type: "nvarchar(max)",
          nullable: false,
          defaultValue: "");

      migrationBuilder.AddColumn<string>(
          name: "Url",
          table: "GoogleMapsLocations",
          type: "nvarchar(max)",
          nullable: false,
          defaultValue: "");

      migrationBuilder.AddColumn<string>(
          name: "Vicinity",
          table: "GoogleMapsLocations",
          type: "nvarchar(max)",
          nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropColumn(
          name: "BusinessStatus",
          table: "GoogleMapsLocations");

      migrationBuilder.DropColumn(
          name: "ExpirationDate",
          table: "GoogleMapsLocations");

      migrationBuilder.DropColumn(
          name: "FormattedAddress",
          table: "GoogleMapsLocations");

      migrationBuilder.DropColumn(
          name: "Icon",
          table: "GoogleMapsLocations");

      migrationBuilder.DropColumn(
          name: "Url",
          table: "GoogleMapsLocations");

      migrationBuilder.DropColumn(
          name: "Vicinity",
          table: "GoogleMapsLocations");

      migrationBuilder.AddColumn<float>(
          name: "Latitude",
          table: "GoogleMapsLocations",
          type: "real",
          nullable: false,
          defaultValue: 0f);

      migrationBuilder.AddColumn<float>(
          name: "Longitude",
          table: "GoogleMapsLocations",
          type: "real",
          nullable: false,
          defaultValue: 0f);

      migrationBuilder.AddColumn<string>(
          name: "Address",
          table: "GoogleMapsLocations",
          type: "nvarchar(max)",
          nullable: true);
    }
  }
}
