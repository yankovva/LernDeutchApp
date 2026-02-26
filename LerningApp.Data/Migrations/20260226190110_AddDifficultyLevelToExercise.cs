using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDifficultyLevelToExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DifficultyLevel",
                table: "TranslationExercises",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "The difficulty level of the exercise");

            migrationBuilder.AddColumn<int>(
                name: "DifficultyLevel",
                table: "MultipleChoiceExercises",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "The difficulty level of the exercise");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DifficultyLevel",
                table: "TranslationExercises");

            migrationBuilder.DropColumn(
                name: "DifficultyLevel",
                table: "MultipleChoiceExercises");
        }
    }
}
