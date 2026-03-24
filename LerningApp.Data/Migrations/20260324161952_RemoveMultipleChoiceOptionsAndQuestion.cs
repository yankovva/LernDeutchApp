using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMultipleChoiceOptionsAndQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectAnswer",
                table: "MultipleChoiceExercises");

            migrationBuilder.DropColumn(
                name: "FirstWrongAnswer",
                table: "MultipleChoiceExercises");

            migrationBuilder.DropColumn(
                name: "Question",
                table: "MultipleChoiceExercises");

            migrationBuilder.DropColumn(
                name: "SecondWrongAnswer",
                table: "MultipleChoiceExercises");

            migrationBuilder.DropColumn(
                name: "ThirdWrongAnswer",
                table: "MultipleChoiceExercises");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrectAnswer",
                table: "MultipleChoiceExercises",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                comment: "The Correct answer of the exercise");

            migrationBuilder.AddColumn<string>(
                name: "FirstWrongAnswer",
                table: "MultipleChoiceExercises",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                comment: "The First wrong answer of the exercise");

            migrationBuilder.AddColumn<string>(
                name: "Question",
                table: "MultipleChoiceExercises",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                comment: "The Question for the exercise");

            migrationBuilder.AddColumn<string>(
                name: "SecondWrongAnswer",
                table: "MultipleChoiceExercises",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "The Second wrong answer of the exercise");

            migrationBuilder.AddColumn<string>(
                name: "ThirdWrongAnswer",
                table: "MultipleChoiceExercises",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "The Third wrong answer of the exercise if needed");
        }
    }
}
