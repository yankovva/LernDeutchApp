using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPublisherToExercies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PublisherId",
                table: "TranslationExercises",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "Foreign key to ApplicationUser");

            migrationBuilder.AddColumn<Guid>(
                name: "PublisherId",
                table: "MultipleChoiceExercises",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "Foreign key to ApplicationUser");

            migrationBuilder.CreateIndex(
                name: "IX_TranslationExercises_PublisherId",
                table: "TranslationExercises",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_MultipleChoiceExercises_PublisherId",
                table: "MultipleChoiceExercises",
                column: "PublisherId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MultipleChoiceExercises_AspNetUsers_PublisherId",
                table: "MultipleChoiceExercises");

            migrationBuilder.DropForeignKey(
                name: "FK_TranslationExercises_AspNetUsers_PublisherId",
                table: "TranslationExercises");

            migrationBuilder.DropIndex(
                name: "IX_TranslationExercises_PublisherId",
                table: "TranslationExercises");

            migrationBuilder.DropIndex(
                name: "IX_MultipleChoiceExercises_PublisherId",
                table: "MultipleChoiceExercises");

            migrationBuilder.DropColumn(
                name: "PublisherId",
                table: "TranslationExercises");

            migrationBuilder.DropColumn(
                name: "PublisherId",
                table: "MultipleChoiceExercises");
        }
    }
}
