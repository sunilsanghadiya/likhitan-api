using Microsoft.EntityFrameworkCore.Migrations;

namespace likhitan.Migrations
{
    public partial class AddDateColumnUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Users",
                nullable: false
            );
        }
    }
}
