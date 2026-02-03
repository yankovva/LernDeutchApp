using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPartOfSpeech : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PartOfSpeechId",
                table: "VocabularyItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "PartsOfSpeech",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartsOfSpeech", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyItems_PartOfSpeechId",
                table: "VocabularyItems",
                column: "PartOfSpeechId");

            migrationBuilder.AddForeignKey(
                name: "FK_VocabularyItems_PartsOfSpeech_PartOfSpeechId",
                table: "VocabularyItems",
                column: "PartOfSpeechId",
                principalTable: "PartsOfSpeech",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VocabularyItems_PartsOfSpeech_PartOfSpeechId",
                table: "VocabularyItems");

            migrationBuilder.DropTable(
                name: "PartsOfSpeech");

            migrationBuilder.DropIndex(
                name: "IX_VocabularyItems_PartOfSpeechId",
                table: "VocabularyItems");

            migrationBuilder.DropColumn(
                name: "PartOfSpeechId",
                table: "VocabularyItems");
        }
    }
}
