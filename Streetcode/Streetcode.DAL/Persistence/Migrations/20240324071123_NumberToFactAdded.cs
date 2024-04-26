using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streetcode.DAL.Persistence.Migrations
{
    public partial class NumberToFactAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Number",
                schema: "streetcode",
                table: "facts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                @"UPDATE streetcode.facts SET Number = Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                schema: "streetcode",
                table: "facts");
        }
    }
}
