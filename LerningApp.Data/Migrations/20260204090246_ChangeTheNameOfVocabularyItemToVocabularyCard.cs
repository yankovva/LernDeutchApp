using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTheNameOfVocabularyItemToVocabularyCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VocabularyTerms_VocabularyItems_VocabularyItemId",
                table: "VocabularyTerms");

            migrationBuilder.DropTable(
                name: "VocabularyItems");

            migrationBuilder.RenameColumn(
                name: "VocabularyItemId",
                table: "VocabularyTerms",
                newName: "VocabularyCardId");

            migrationBuilder.RenameIndex(
                name: "IX_VocabularyTerms_VocabularyItemId_Side",
                table: "VocabularyTerms",
                newName: "IX_VocabularyTerms_VocabularyCardId_Side");

            migrationBuilder.CreateTable(
                name: "VocabularyCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartOfSpeechId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularyCards_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VocabularyCards_PartsOfSpeech_PartOfSpeechId",
                        column: x => x.PartOfSpeechId,
                        principalTable: "PartsOfSpeech",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyCards_LessonId",
                table: "VocabularyCards",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyCards_PartOfSpeechId",
                table: "VocabularyCards",
                column: "PartOfSpeechId");

            migrationBuilder.AddForeignKey(
                name: "FK_VocabularyTerms_VocabularyCards_VocabularyCardId",
                table: "VocabularyTerms",
                column: "VocabularyCardId",
                principalTable: "VocabularyCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VocabularyTerms_VocabularyCards_VocabularyCardId",
                table: "VocabularyTerms");

            migrationBuilder.DropTable(
                name: "VocabularyCards");

            migrationBuilder.RenameColumn(
                name: "VocabularyCardId",
                table: "VocabularyTerms",
                newName: "VocabularyItemId");

            migrationBuilder.RenameIndex(
                name: "IX_VocabularyTerms_VocabularyCardId_Side",
                table: "VocabularyTerms",
                newName: "IX_VocabularyTerms_VocabularyItemId_Side");

            migrationBuilder.CreateTable(
                name: "VocabularyItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartOfSpeechId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularyItems_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VocabularyItems_PartsOfSpeech_PartOfSpeechId",
                        column: x => x.PartOfSpeechId,
                        principalTable: "PartsOfSpeech",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyItems_LessonId",
                table: "VocabularyItems",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyItems_PartOfSpeechId",
                table: "VocabularyItems",
                column: "PartOfSpeechId");

            migrationBuilder.AddForeignKey(
                name: "FK_VocabularyTerms_VocabularyItems_VocabularyItemId",
                table: "VocabularyTerms",
                column: "VocabularyItemId",
                principalTable: "VocabularyItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
