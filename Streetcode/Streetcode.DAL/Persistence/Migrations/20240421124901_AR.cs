using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streetcode.DAL.Persistence.Migrations
{
    public partial class AR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InstagramARLink",
                schema: "streetcode",
                table: "streetcodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvolvedPeople",
                schema: "streetcode",
                table: "streetcodes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstagramARLink",
                schema: "streetcode",
                table: "streetcodes");

            migrationBuilder.DropColumn(
                name: "InvolvedPeople",
                schema: "streetcode",
                table: "streetcodes");
        }
    }
}
