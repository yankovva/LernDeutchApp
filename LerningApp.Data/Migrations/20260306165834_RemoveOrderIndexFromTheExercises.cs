using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOrderIndexFromTheExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TranslationExercises_LessonId_OrderIndex",
                table: "TranslationExercises");

            migrationBuilder.DropIndex(
                name: "IX_MultipleChoiceExercises_LessonId_OrderIndex",
                table: "MultipleChoiceExercises");

            migrationBuilder.DropColumn(
                name: "OrderIndex",
                table: "TranslationExercises");

            migrationBuilder.DropColumn(
                name: "OrderIndex",
                table: "MultipleChoiceExercises");

            migrationBuilder.CreateIndex(
                name: "IX_TranslationExercises_LessonId",
                table: "TranslationExercises",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_MultipleChoiceExercises_LessonId",
                table: "MultipleChoiceExercises",
                column: "LessonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TranslationExercises_LessonId",
                table: "TranslationExercises");

            migrationBuilder.DropIndex(
                name: "IX_MultipleChoiceExercises_LessonId",
                table: "MultipleChoiceExercises");

            migrationBuilder.AddColumn<int>(
                name: "OrderIndex",
                table: "TranslationExercises",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "The order of the exercise");

            migrationBuilder.AddColumn<int>(
                name: "OrderIndex",
                table: "MultipleChoiceExercises",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "The order of the exercise");

            migrationBuilder.CreateIndex(
                name: "IX_TranslationExercises_LessonId_OrderIndex",
                table: "TranslationExercises",
                columns: new[] { "LessonId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_MultipleChoiceExercises_LessonId_OrderIndex",
                table: "MultipleChoiceExercises",
                columns: new[] { "LessonId", "OrderIndex" });
        }
    }
}
