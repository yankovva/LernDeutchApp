using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImageToVocabularyCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "VocabularyCards",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                defaultValue: "/images/VocabularyCardsImages/defaultcardimage.png",
                comment: "Image of the VocabularyCard");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "VocabularyCards");
        }
    }
}
