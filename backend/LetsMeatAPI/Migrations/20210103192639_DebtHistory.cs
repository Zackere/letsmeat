using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LetsMeatAPI.Migrations {
  public partial class DebtHistory : Migration {
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.CreateTable(
          name: "DebtHistory",
          columns: table => new {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            PendingDebtId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            FromId = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ToId = table.Column<string>(type: "nvarchar(max)", nullable: false),
            GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Amount = table.Column<long>(type: "bigint", nullable: false),
            Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
            Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
            ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ImageDebtId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Type = table.Column<int>(type: "int", nullable: false),
            HistoryEntryCreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_DebtHistory", x => x.Id);
          });
    }

    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropTable(
          name: "DebtHistory");
    }
  }
}
