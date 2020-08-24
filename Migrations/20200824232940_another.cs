using Microsoft.EntityFrameworkCore.Migrations;

namespace EgyptTourism.Migrations
{
    public partial class another : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Destinations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Destinations");
        }
    }
}
