using Microsoft.EntityFrameworkCore.Migrations;

namespace VirusHack.WebApp.Migrations
{
    public partial class session : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventSessionId",
                table: "Webinars",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventSessionId",
                table: "Webinars");
        }
    }
}
