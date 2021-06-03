using Microsoft.EntityFrameworkCore.Migrations;

namespace Bloggy.Migrations
{
    public partial class CreatedReadCountField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReadCount",
                table: "BlogPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReadCount",
                table: "BlogPosts");
        }
    }
}
