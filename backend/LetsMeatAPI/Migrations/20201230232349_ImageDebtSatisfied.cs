using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LetsMeatAPI.Migrations {
  public partial class ImageDebtSatisfied : Migration {
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.AddColumn<bool>(
          name: "Satisfied",
          table: "DebtsFromImages",
          type: "bit",
          nullable: false,
          defaultValue: false);

      migrationBuilder.CreateTable(
          name: "PendingDebtFromImageBounds",
          columns: table => new {
            DebtFromImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            PendingDebtId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_PendingDebtFromImageBounds", x => new { x.DebtFromImageId, x.PendingDebtId });
            table.ForeignKey(
                      name: "FK_PendingDebtFromImageBounds_DebtsFromImages_DebtFromImageId",
                      column: x => x.DebtFromImageId,
                      principalTable: "DebtsFromImages",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_PendingDebtFromImageBounds_PendingDebts_PendingDebtId",
                      column: x => x.PendingDebtId,
                      principalTable: "PendingDebts",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_PendingDebtFromImageBounds_DebtFromImageId",
          table: "PendingDebtFromImageBounds",
          column: "DebtFromImageId",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_PendingDebtFromImageBounds_PendingDebtId",
          table: "PendingDebtFromImageBounds",
          column: "PendingDebtId",
          unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropTable(
          name: "PendingDebtFromImageBounds");

      migrationBuilder.DropColumn(
          name: "Satisfied",
          table: "DebtsFromImages");
    }
  }
}
