using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonPublisher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PublisherId",
                table: "Lessons",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "The Publisher of the Lesson");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_PublisherId",
                table: "Lessons",
                column: "PublisherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_AspNetUsers_PublisherId",
                table: "Lessons",
                column: "PublisherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_AspNetUsers_PublisherId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_PublisherId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "PublisherId",
                table: "Lessons");
        }
    }
}
