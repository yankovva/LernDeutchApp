using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleChoiceExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MultipleChoiceExercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false, comment: "The Question for the exercise"),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "The Correct answer of the exercise"),
                    FirstWrongAnswer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "The First wrong answer of the exercise"),
                    SecondWrongAnswer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "The Second wrong answer of the exercise"),
                    ThirdWrongAnswer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "The Third wrong answer of the exercise if needed"),
                    OrderIndex = table.Column<int>(type: "int", nullable: false, comment: "The order of the exercise"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to Lesson")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultipleChoiceExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MultipleChoiceExercises_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MultipleChoiceExercises_LessonId_OrderIndex",
                table: "MultipleChoiceExercises",
                columns: new[] { "LessonId", "OrderIndex" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MultipleChoiceExercises");
        }
    }
}
