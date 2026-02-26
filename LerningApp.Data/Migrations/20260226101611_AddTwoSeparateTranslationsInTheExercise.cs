using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTwoSeparateTranslationsInTheExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChosenTranslationLanguage",
                table: "TranslationExercises");

            migrationBuilder.DropColumn(
                name: "CorrectTranslation",
                table: "TranslationExercises");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectTranslationBg",
                table: "TranslationExercises");

            migrationBuilder.DropColumn(
                name: "CorrectTranslationEn",
                table: "TranslationExercises");

            migrationBuilder.AddColumn<int>(
                name: "ChosenTranslationLanguage",
                table: "TranslationExercises",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "The chosen translation language option bg / en");

            migrationBuilder.AddColumn<string>(
                name: "CorrectTranslation",
                table: "TranslationExercises",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "",
                comment: "The correct translated sentence in the chosen language");
        }
    }
}
