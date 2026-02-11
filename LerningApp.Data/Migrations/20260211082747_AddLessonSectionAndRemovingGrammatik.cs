using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonSectionAndRemovingGrammatik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gramatic",
                table: "Lessons");

            migrationBuilder.CreateTable(
                name: "LessonSection",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "PK Unique Identifier"),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to Lesson"),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "The Type of the LessonSection - grammar/exercise"),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 7000, nullable: false, comment: "The Content of the LessonSection"),
                    OrderIndex = table.Column<int>(type: "int", nullable: false, comment: "The Order Index of the LessonSection")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonSection_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonSection_LessonId_OrderIndex",
                table: "LessonSection",
                columns: new[] { "LessonId", "OrderIndex" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonSection");

            migrationBuilder.AddColumn<string>(
                name: "Gramatic",
                table: "Lessons",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: false,
                defaultValue: "",
                comment: "The Gramatic part of the Lesson");
        }
    }
}
