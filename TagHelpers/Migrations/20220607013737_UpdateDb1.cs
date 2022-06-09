using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS.Migrations
{
    public partial class UpdateDb1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_JobTitles_JobTitleId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Units_UnitId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_ApprovalById",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "JobTitles");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Posts_ApprovalById",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_JobTitleId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UnitId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ApprovalById",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ApprovalReason",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "JobTitleId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ImageCategory",
                table: "PostCategories",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageCategory",
                table: "PostCategories");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalById",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovalReason",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatus",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JobTitleId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JobTitles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Activated = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTitles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Activated = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ApprovalById",
                table: "Posts",
                column: "ApprovalById");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_JobTitleId",
                table: "AspNetUsers",
                column: "JobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UnitId",
                table: "AspNetUsers",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_JobTitles_JobTitleId",
                table: "AspNetUsers",
                column: "JobTitleId",
                principalTable: "JobTitles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Units_UnitId",
                table: "AspNetUsers",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_ApprovalById",
                table: "Posts",
                column: "ApprovalById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
