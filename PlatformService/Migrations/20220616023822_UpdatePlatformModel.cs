using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformService.Migrations
{
    public partial class UpdatePlatformModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Published",
                table: "Platforms",
                newName: "Publisher");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Publisher",
                table: "Platforms",
                newName: "Published");
        }
    }
}
