using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleChoiceQuestionAndOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isCorrect",
                table: "ListeningExerciseOptions",
                newName: "IsCorrect");

            migrationBuilder.CreateTable(
                name: "MultipleChoiceQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PublisherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MultipleChoiceExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultipleChoiceQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MultipleChoiceQuestions_MultipleChoiceExercises_MultipleChoiceExerciseId",
                        column: x => x.MultipleChoiceExerciseId,
                        principalTable: "MultipleChoiceExercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MultipleChoiceQuestions_Teachers_PublisherId",
                        column: x => x.PublisherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MultipleChoiceExerciseOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "The answer of the exercise"),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false, comment: "Whether the answer is correct or not"),
                    OrderIndex = table.Column<int>(type: "int", nullable: false, comment: "Order index of the answer"),
                    MultipleChoiceQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to the MultipleChoise Question")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultipleChoiceExerciseOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MultipleChoiceExerciseOptions_MultipleChoiceQuestions_MultipleChoiceQuestionId",
                        column: x => x.MultipleChoiceQuestionId,
                        principalTable: "MultipleChoiceQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MultipleChoiceExerciseOptions_MultipleChoiceQuestionId",
                table: "MultipleChoiceExerciseOptions",
                column: "MultipleChoiceQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_MultipleChoiceExerciseOptions_MultipleChoiceQuestionId_OrderIndex",
                table: "MultipleChoiceExerciseOptions",
                columns: new[] { "MultipleChoiceQuestionId", "OrderIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MultipleChoiceQuestions_MultipleChoiceExerciseId",
                table: "MultipleChoiceQuestions",
                column: "MultipleChoiceExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_MultipleChoiceQuestions_PublisherId",
                table: "MultipleChoiceQuestions",
                column: "PublisherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MultipleChoiceExerciseOptions");

            migrationBuilder.DropTable(
                name: "MultipleChoiceQuestions");

            migrationBuilder.RenameColumn(
                name: "IsCorrect",
                table: "ListeningExerciseOptions",
                newName: "isCorrect");
        }
    }
}
