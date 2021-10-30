using Microsoft.EntityFrameworkCore.Migrations;

namespace MarshalProject.Migrations
{
    public partial class AddProparityToUserTabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Areas_UserId",
                table: "Areas");

            migrationBuilder.AddColumn<int>(
                name: "IdentityCardNumber",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Areas_UserId",
                table: "Areas",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Areas_UserId",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "IdentityCardNumber",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Areas_UserId",
                table: "Areas",
                column: "UserId");
        }
    }
}
