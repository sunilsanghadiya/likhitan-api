using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace likhitan_api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKeyBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlogId",
                table: "BlogComments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlogId",
                table: "BlogComments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
