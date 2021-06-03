using Microsoft.EntityFrameworkCore.Migrations;

namespace Bloggy.Migrations
{
    public partial class AuthorAlias : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorAlias",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorAlias",
                table: "AspNetUsers");
        }
    }
}
