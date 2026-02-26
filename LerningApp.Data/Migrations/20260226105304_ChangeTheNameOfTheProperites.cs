using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTheNameOfTheProperites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectTranslationBg",
                table: "TranslationExercises");

            migrationBuilder.DropColumn(
                name: "CorrectTranslationEn",
                table: "TranslationExercises");

            migrationBuilder.AlterColumn<string>(
                name: "GermanSentence",
                table: "TranslationExercises",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                comment: "The german sentence",
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldComment: "The german sentence for translation");

            migrationBuilder.AddColumn<string>(
                name: "BulgarianSentence",
                table: "TranslationExercises",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "",
                comment: "The bulgarian sentence");

            migrationBuilder.AddColumn<string>(
                name: "EnglishSentence",
                table: "TranslationExercises",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "",
                comment: "The english sentence");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BulgarianSentence",
                table: "TranslationExercises");

            migrationBuilder.DropColumn(
                name: "EnglishSentence",
                table: "TranslationExercises");

            migrationBuilder.AlterColumn<string>(
                name: "GermanSentence",
                table: "TranslationExercises",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                comment: "The german sentence for translation",
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldComment: "The german sentence");

            migrationBuilder.AddColumn<string>(
                name: "CorrectTranslationBg",
                table: "TranslationExercises",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "",
                comment: "The correct translated sentence in Bulgarian");

            migrationBuilder.AddColumn<string>(
                name: "CorrectTranslationEn",
                table: "TranslationExercises",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "",
                comment: "The correct translated sentence in English");
        }
    }
}
