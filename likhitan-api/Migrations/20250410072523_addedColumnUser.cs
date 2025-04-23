using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace likhitan.Migrations
{
    /// <inheritdoc />
    public partial class addedColumnUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTeamsAndConditionAccepted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTeamsAndConditionAccepted",
                table: "Users");
        }
    }
}
