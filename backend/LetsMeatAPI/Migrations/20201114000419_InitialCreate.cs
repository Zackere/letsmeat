using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsMeatAPI.Migrations {
  public partial class InitialCreate : Migration {
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.CreateTable(
          name: "Users",
          columns: table => new {
            Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            PictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Prefs = table.Column<string>(type: "nvarchar(max)", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_Users", x => x.Id);
          });
    }

    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropTable(
          name: "Users");
    }
  }
}
