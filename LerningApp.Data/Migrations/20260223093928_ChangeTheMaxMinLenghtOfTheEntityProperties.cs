using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTheMaxMinLenghtOfTheEntityProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Word",
                table: "VocabularyTerms",
                type: "nvarchar(63)",
                maxLength: 63,
                nullable: false,
                comment: "The Word of the VocabularyTerm",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldComment: "The Word of the VocabularyTerm");

            migrationBuilder.AlterColumn<string>(
                name: "Target",
                table: "Lessons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "The Target of the Lesson",
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldComment: "The Target of the Lesson");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Courses",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                comment: "The Description of the Course",
                oldClrType: typeof(string),
                oldType: "nvarchar(3000)",
                oldMaxLength: 3000,
                oldComment: "The Description of the Course");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Word",
                table: "VocabularyTerms",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                comment: "The Word of the VocabularyTerm",
                oldClrType: typeof(string),
                oldType: "nvarchar(63)",
                oldMaxLength: 63,
                oldComment: "The Word of the VocabularyTerm");

            migrationBuilder.AlterColumn<string>(
                name: "Target",
                table: "Lessons",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                comment: "The Target of the Lesson",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldComment: "The Target of the Lesson");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Courses",
                type: "nvarchar(3000)",
                maxLength: 3000,
                nullable: false,
                comment: "The Description of the Course",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldComment: "The Description of the Course");
        }
    }
}
