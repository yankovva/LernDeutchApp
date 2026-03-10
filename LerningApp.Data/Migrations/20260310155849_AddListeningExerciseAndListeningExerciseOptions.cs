using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddListeningExerciseAndListeningExerciseOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListeningExercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false, comment: "The Question for the exercise"),
                    AudioPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "The Audio path of the exercise"),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false, comment: "The difficulty level of the exercise"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Shows if the exercise is deleted"),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to Lesson"),
                    PublisherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to ApplicationUser")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListeningExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListeningExercises_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListeningExercises_Teachers_PublisherId",
                        column: x => x.PublisherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ListeningExerciseOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "The answer of the exercise"),
                    isCorrect = table.Column<bool>(type: "bit", nullable: false, comment: "Whether the answer is true or false"),
                    OrderIndex = table.Column<int>(type: "int", nullable: false, comment: "Order index of the answer"),
                    ListeningExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to ListeningExercise")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListeningExerciseOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListeningExerciseOptions_ListeningExercises_ListeningExerciseId",
                        column: x => x.ListeningExerciseId,
                        principalTable: "ListeningExercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListeningExerciseOptions_ListeningExerciseId",
                table: "ListeningExerciseOptions",
                column: "ListeningExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ListeningExerciseOptions_ListeningExerciseId_OrderIndex",
                table: "ListeningExerciseOptions",
                columns: new[] { "ListeningExerciseId", "OrderIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListeningExercises_LessonId",
                table: "ListeningExercises",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_ListeningExercises_PublisherId",
                table: "ListeningExercises",
                column: "PublisherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListeningExerciseOptions");

            migrationBuilder.DropTable(
                name: "ListeningExercises");
        }
    }
}
