using Microsoft.EntityFrameworkCore.Migrations;

namespace MarshalProject.Migrations
{
    public partial class AddProparityToAppRoleAndAppUserAndAddAplicationUserRoleAndModifiedDataContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUserRoles",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUserRoles");
        }
    }
}
