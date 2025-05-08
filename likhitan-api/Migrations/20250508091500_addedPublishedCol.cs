using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace likhitan_api.Migrations
{
    /// <inheritdoc />
    public partial class addedPublishedCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Blogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Published",
                table: "Blogs",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "Published",
                table: "Blogs");
        }
    }
}
