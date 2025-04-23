using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace likhitan.Migrations
{
    /// <inheritdoc />
    public partial class AddedOtpColumnUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OTP",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "OTPExpire",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OTP",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OTPExpire",
                table: "Users");
        }
    }
}
