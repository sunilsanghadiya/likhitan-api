using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace likhitan.Migrations
{
    /// <inheritdoc />
    public partial class removeBlogIdOnSomeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlogId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "BlogId",
                table: "BlogCategories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlogId",
                table: "Tags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BlogId",
                table: "BlogCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
