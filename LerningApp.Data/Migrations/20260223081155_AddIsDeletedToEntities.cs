using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "VocabularyTerms",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Shows if the VocabularyTerm is deleted");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "VocabularyCards",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Shows if the VocabularyCard is deleted");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UsersCourses",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Shows if the UserCourse is deleted");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "LessonsSections",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Shows if the Lesson Section is deleted");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Courses",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Shows if the Course is deleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "VocabularyTerms");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "VocabularyCards");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UsersCourses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "LessonsSections");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Courses");
        }
    }
}
