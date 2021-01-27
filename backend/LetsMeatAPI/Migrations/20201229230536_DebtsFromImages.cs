using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LetsMeatAPI.Migrations {
  public partial class DebtsFromImages : Migration {
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.CreateTable(
          name: "DebtsFromImages",
          columns: table => new {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Amount = table.Column<long>(type: "bigint", nullable: false),
            Description = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_DebtsFromImages", x => x.Id);
            table.ForeignKey(
                      name: "FK_DebtsFromImages_Images_ImageId",
                      column: x => x.ImageId,
                      principalTable: "Images",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_DebtsFromImages_ImageId",
          table: "DebtsFromImages",
          column: "ImageId");
    }

    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropTable(
          name: "DebtsFromImages");
    }
  }
}
