using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLessonSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonsSections");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Lessons",
                type: "nvarchar(1500)",
                maxLength: 1500,
                nullable: false,
                comment: "The Content of the Lesson",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldComment: "The Content of the Lesson");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Lessons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "The Content of the Lesson",
                oldClrType: typeof(string),
                oldType: "nvarchar(1500)",
                oldMaxLength: 1500,
                oldComment: "The Content of the Lesson");

            migrationBuilder.CreateTable(
                name: "LessonsSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "PK Unique Identifier"),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to Lesson"),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 7000, nullable: false, comment: "The Content of the LessonSection"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Shows if the Lesson Section is deleted"),
                    OrderIndex = table.Column<int>(type: "int", nullable: false, comment: "The Order Index of the LessonSection"),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "The Type of the LessonSection - grammar/exercise")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonsSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonsSections_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonsSections_LessonId_OrderIndex",
                table: "LessonsSections",
                columns: new[] { "LessonId", "OrderIndex" });
        }
    }
}
