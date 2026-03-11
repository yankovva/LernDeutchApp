using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddListeningQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListeningExerciseOptions_ListeningExercises_ListeningExerciseId",
                table: "ListeningExerciseOptions");

            migrationBuilder.DropIndex(
                name: "IX_ListeningExerciseOptions_ListeningExerciseId",
                table: "ListeningExerciseOptions");

            migrationBuilder.DropIndex(
                name: "IX_ListeningExerciseOptions_ListeningExerciseId_OrderIndex",
                table: "ListeningExerciseOptions");

            migrationBuilder.DropColumn(
                name: "Question",
                table: "ListeningExercises");

            migrationBuilder.DropColumn(
                name: "ListeningExerciseId",
                table: "ListeningExerciseOptions");

            migrationBuilder.AlterColumn<bool>(
                name: "isCorrect",
                table: "ListeningExerciseOptions",
                type: "bit",
                nullable: false,
                comment: "Whether the answer is correct or not",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Whether the answer is true or false");

            migrationBuilder.AddColumn<Guid>(
                name: "ListeningQuestionId",
                table: "ListeningExerciseOptions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "Foreign key to the Listening Question");

            migrationBuilder.CreateTable(
                name: "ListeningQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PublisherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    ListeningExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListeningQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListeningQuestions_AspNetUsers_PublisherId",
                        column: x => x.PublisherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListeningQuestions_ListeningExercises_ListeningExerciseId",
                        column: x => x.ListeningExerciseId,
                        principalTable: "ListeningExercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListeningExerciseOptions_ListeningQuestionId",
                table: "ListeningExerciseOptions",
                column: "ListeningQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ListeningExerciseOptions_ListeningQuestionId_OrderIndex",
                table: "ListeningExerciseOptions",
                columns: new[] { "ListeningQuestionId", "OrderIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListeningQuestions_ListeningExerciseId",
                table: "ListeningQuestions",
                column: "ListeningExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ListeningQuestions_PublisherId",
                table: "ListeningQuestions",
                column: "PublisherId");

            migrationBuilder.AddForeignKey(
                name: "FK_ListeningExerciseOptions_ListeningQuestions_ListeningQuestionId",
                table: "ListeningExerciseOptions",
                column: "ListeningQuestionId",
                principalTable: "ListeningQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListeningExerciseOptions_ListeningQuestions_ListeningQuestionId",
                table: "ListeningExerciseOptions");

            migrationBuilder.DropTable(
                name: "ListeningQuestions");

            migrationBuilder.DropIndex(
                name: "IX_ListeningExerciseOptions_ListeningQuestionId",
                table: "ListeningExerciseOptions");

            migrationBuilder.DropIndex(
                name: "IX_ListeningExerciseOptions_ListeningQuestionId_OrderIndex",
                table: "ListeningExerciseOptions");

            migrationBuilder.DropColumn(
                name: "ListeningQuestionId",
                table: "ListeningExerciseOptions");

            migrationBuilder.AddColumn<string>(
                name: "Question",
                table: "ListeningExercises",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                comment: "The Question for the exercise");

            migrationBuilder.AlterColumn<bool>(
                name: "isCorrect",
                table: "ListeningExerciseOptions",
                type: "bit",
                nullable: false,
                comment: "Whether the answer is true or false",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Whether the answer is correct or not");

            migrationBuilder.AddColumn<Guid>(
                name: "ListeningExerciseId",
                table: "ListeningExerciseOptions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "Foreign key to ListeningExercise");

            migrationBuilder.CreateIndex(
                name: "IX_ListeningExerciseOptions_ListeningExerciseId",
                table: "ListeningExerciseOptions",
                column: "ListeningExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ListeningExerciseOptions_ListeningExerciseId_OrderIndex",
                table: "ListeningExerciseOptions",
                columns: new[] { "ListeningExerciseId", "OrderIndex" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ListeningExerciseOptions_ListeningExercises_ListeningExerciseId",
                table: "ListeningExerciseOptions",
                column: "ListeningExerciseId",
                principalTable: "ListeningExercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
