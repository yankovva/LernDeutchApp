using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakingTeacherTheCreatorOfAListeningQestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListeningQuestions_AspNetUsers_PublisherId",
                table: "ListeningQuestions");

            migrationBuilder.AddForeignKey(
                name: "FK_ListeningQuestions_Teachers_PublisherId",
                table: "ListeningQuestions",
                column: "PublisherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListeningQuestions_Teachers_PublisherId",
                table: "ListeningQuestions");

            migrationBuilder.AddForeignKey(
                name: "FK_ListeningQuestions_AspNetUsers_PublisherId",
                table: "ListeningQuestions",
                column: "PublisherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
