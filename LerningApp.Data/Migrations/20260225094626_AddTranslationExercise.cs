using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslationExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TranslationExercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GermanSentence = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false, comment: "The german sentence for translation"),
                    ChosenTranslationLanguage = table.Column<int>(type: "int", nullable: false, comment: "The chosen translation language option bg / en"),
                    CorrectTranslation = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false, comment: "The correct translated sentence in the chosen language"),
                    OrderIndex = table.Column<int>(type: "int", nullable: false, comment: "The order of the exercise"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Shows if the exercise is deleted"),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to Lesson")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TranslationExercises_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TranslationExercises_LessonId_OrderIndex",
                table: "TranslationExercises",
                columns: new[] { "LessonId", "OrderIndex" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TranslationExercises");
        }
    }
}
