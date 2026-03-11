using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakingDefaultValueOfThePhotoNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "VocabularyCards",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "Image of the VocabularyCard",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldDefaultValue: "/images/VocabularyCardsImages/defaultcardimage.png",
                oldComment: "Image of the VocabularyCard");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "VocabularyCards",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                defaultValue: "/images/VocabularyCardsImages/defaultcardimage.png",
                comment: "Image of the VocabularyCard",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "Image of the VocabularyCard");
        }
    }
}
