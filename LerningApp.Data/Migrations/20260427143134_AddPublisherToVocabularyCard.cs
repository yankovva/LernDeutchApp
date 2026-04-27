using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPublisherToVocabularyCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PublisherId",
                table: "VocabularyCards",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "The Publisher of the Card");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyCards_PublisherId",
                table: "VocabularyCards",
                column: "PublisherId");

            migrationBuilder.AddForeignKey(
                name: "FK_VocabularyCards_Teachers_PublisherId",
                table: "VocabularyCards",
                column: "PublisherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VocabularyCards_Teachers_PublisherId",
                table: "VocabularyCards");

            migrationBuilder.DropIndex(
                name: "IX_VocabularyCards_PublisherId",
                table: "VocabularyCards");

            migrationBuilder.DropColumn(
                name: "PublisherId",
                table: "VocabularyCards");
        }
    }
}
