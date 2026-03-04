using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTheTypeOfPublisherToTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_PublisherId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_AspNetUsers_PublisherId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_MultipleChoiceExercises_AspNetUsers_PublisherId",
                table: "MultipleChoiceExercises");

            migrationBuilder.DropForeignKey(
                name: "FK_TranslationExercises_AspNetUsers_PublisherId",
                table: "TranslationExercises");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Teacher_PublisherId",
                table: "Courses",
                column: "PublisherId",
                principalTable: "Teacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Teacher_PublisherId",
                table: "Lessons",
                column: "PublisherId",
                principalTable: "Teacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MultipleChoiceExercises_Teacher_PublisherId",
                table: "MultipleChoiceExercises",
                column: "PublisherId",
                principalTable: "Teacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationExercises_Teacher_PublisherId",
                table: "TranslationExercises",
                column: "PublisherId",
                principalTable: "Teacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Teacher_PublisherId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Teacher_PublisherId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_MultipleChoiceExercises_Teacher_PublisherId",
                table: "MultipleChoiceExercises");

            migrationBuilder.DropForeignKey(
                name: "FK_TranslationExercises_Teacher_PublisherId",
                table: "TranslationExercises");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_PublisherId",
                table: "Courses",
                column: "PublisherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_AspNetUsers_PublisherId",
                table: "Lessons",
                column: "PublisherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MultipleChoiceExercises_AspNetUsers_PublisherId",
                table: "MultipleChoiceExercises",
                column: "PublisherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationExercises_AspNetUsers_PublisherId",
                table: "TranslationExercises",
                column: "PublisherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
