using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace likhitan.Migrations
{
    /// <inheritdoc />
    public partial class AddedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BlogCommentsId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlogLikesId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BlogViewsId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Author",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogComments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogLikes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Viewed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogViews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Excerpt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BlogCategoryId = table.Column<int>(type: "int", nullable: false),
                    TagsId = table.Column<int>(type: "int", nullable: false),
                    BlogCommentsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BlogLikesId = table.Column<int>(type: "int", nullable: true),
                    BlogViewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blogs_Author_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Author",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Blogs_BlogCategories_BlogCategoryId",
                        column: x => x.BlogCategoryId,
                        principalTable: "BlogCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Blogs_BlogComments_BlogCommentsId",
                        column: x => x.BlogCommentsId,
                        principalTable: "BlogComments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Blogs_BlogLikes_BlogLikesId",
                        column: x => x.BlogLikesId,
                        principalTable: "BlogLikes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Blogs_BlogViews_BlogViewsId",
                        column: x => x.BlogViewsId,
                        principalTable: "BlogViews",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Blogs_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_AuthorId",
                table: "Users",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BlogCommentsId",
                table: "Users",
                column: "BlogCommentsId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BlogLikesId",
                table: "Users",
                column: "BlogLikesId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BlogViewsId",
                table: "Users",
                column: "BlogViewsId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_AuthorId",
                table: "Blogs",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_BlogCategoryId",
                table: "Blogs",
                column: "BlogCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_BlogCommentsId",
                table: "Blogs",
                column: "BlogCommentsId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_BlogLikesId",
                table: "Blogs",
                column: "BlogLikesId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_BlogViewsId",
                table: "Blogs",
                column: "BlogViewsId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_TagsId",
                table: "Blogs",
                column: "TagsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Author_AuthorId",
                table: "Users",
                column: "AuthorId",
                principalTable: "Author",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_BlogComments_BlogCommentsId",
                table: "Users",
                column: "BlogCommentsId",
                principalTable: "BlogComments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_BlogLikes_BlogLikesId",
                table: "Users",
                column: "BlogLikesId",
                principalTable: "BlogLikes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_BlogViews_BlogViewsId",
                table: "Users",
                column: "BlogViewsId",
                principalTable: "BlogViews",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Author_AuthorId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_BlogComments_BlogCommentsId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_BlogLikes_BlogLikesId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_BlogViews_BlogViewsId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Blogs");

            migrationBuilder.DropTable(
                name: "Author");

            migrationBuilder.DropTable(
                name: "BlogCategories");

            migrationBuilder.DropTable(
                name: "BlogComments");

            migrationBuilder.DropTable(
                name: "BlogLikes");

            migrationBuilder.DropTable(
                name: "BlogViews");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Users_AuthorId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BlogCommentsId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BlogLikesId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BlogViewsId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BlogCommentsId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BlogLikesId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BlogViewsId",
                table: "Users");
        }
    }
}
