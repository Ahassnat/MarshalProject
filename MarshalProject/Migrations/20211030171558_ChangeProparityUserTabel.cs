using Microsoft.EntityFrameworkCore.Migrations;

namespace MarshalProject.Migrations
{
    public partial class ChangeProparityUserTabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AreaName",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaName",
                table: "AspNetUsers");
        }
    }
}
