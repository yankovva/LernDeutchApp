using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonSectionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonSection_Lessons_LessonId",
                table: "LessonSection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonSection",
                table: "LessonSection");

            migrationBuilder.RenameTable(
                name: "LessonSection",
                newName: "LessonsSections");

            migrationBuilder.RenameIndex(
                name: "IX_LessonSection_LessonId_OrderIndex",
                table: "LessonsSections",
                newName: "IX_LessonsSections_LessonId_OrderIndex");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonsSections",
                table: "LessonsSections",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonsSections_Lessons_LessonId",
                table: "LessonsSections",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonsSections_Lessons_LessonId",
                table: "LessonsSections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonsSections",
                table: "LessonsSections");

            migrationBuilder.RenameTable(
                name: "LessonsSections",
                newName: "LessonSection");

            migrationBuilder.RenameIndex(
                name: "IX_LessonsSections_LessonId_OrderIndex",
                table: "LessonSection",
                newName: "IX_LessonSection_LessonId_OrderIndex");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonSection",
                table: "LessonSection",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonSection_Lessons_LessonId",
                table: "LessonSection",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
