using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace likhitan.Migrations
{
    /// <inheritdoc />
    public partial class updateKeysOnCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "BlogCategories");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "BlogCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "BlogCategories");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "BlogCategories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
