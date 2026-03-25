using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMultipleChoiceQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MultipleChoiceExerciseOptions_MultipleChoiceQuestions_MultipleChoiceQuestionId",
                table: "MultipleChoiceExerciseOptions");

            migrationBuilder.DropTable(
                name: "MultipleChoiceQuestions");

            migrationBuilder.DropIndex(
                name: "IX_MultipleChoiceExerciseOptions_MultipleChoiceQuestionId",
                table: "MultipleChoiceExerciseOptions");

            migrationBuilder.DropIndex(
                name: "IX_MultipleChoiceExerciseOptions_MultipleChoiceQuestionId_OrderIndex",
                table: "MultipleChoiceExerciseOptions");

            migrationBuilder.DropColumn(
                name: "MultipleChoiceQuestionId",
                table: "MultipleChoiceExerciseOptions");

            migrationBuilder.AddColumn<Guid>(
                name: "MultipleChoiceExerciseId",
                table: "MultipleChoiceExerciseOptions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "Foreign key to the MultipleChoiceExercise");

            migrationBuilder.CreateIndex(
                name: "IX_MultipleChoiceExerciseOptions_MultipleChoiceExerciseId",
                table: "MultipleChoiceExerciseOptions",
                column: "MultipleChoiceExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_MultipleChoiceExerciseOptions_MultipleChoiceExerciseId_OrderIndex",
                table: "MultipleChoiceExerciseOptions",
                columns: new[] { "MultipleChoiceExerciseId", "OrderIndex" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MultipleChoiceExerciseOptions_MultipleChoiceExercises_MultipleChoiceExerciseId",
                table: "MultipleChoiceExerciseOptions",
                column: "MultipleChoiceExerciseId",
                principalTable: "MultipleChoiceExercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MultipleChoiceExerciseOptions_MultipleChoiceExercises_MultipleChoiceExerciseId",
                table: "MultipleChoiceExerciseOptions");

            migrationBuilder.DropIndex(
                name: "IX_MultipleChoiceExerciseOptions_MultipleChoiceExerciseId",
                table: "MultipleChoiceExerciseOptions");

            migrationBuilder.DropIndex(
                name: "IX_MultipleChoiceExerciseOptions_MultipleChoiceExerciseId_OrderIndex",
                table: "MultipleChoiceExerciseOptions");

            migrationBuilder.DropColumn(
                name: "MultipleChoiceExerciseId",
                table: "MultipleChoiceExerciseOptions");

            migrationBuilder.AddColumn<Guid>(
                name: "MultipleChoiceQuestionId",
                table: "MultipleChoiceExerciseOptions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "Foreign key to the MultipleChoise Question");

            migrationBuilder.CreateTable(
                name: "MultipleChoiceQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MultipleChoiceExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublisherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "FK_MultipleChoiceExerciseOptions_MultipleChoiceQuestions_MultipleChoiceQuestionId",
                table: "MultipleChoiceExerciseOptions",
                column: "MultipleChoiceQuestionId",
                principalTable: "MultipleChoiceQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
