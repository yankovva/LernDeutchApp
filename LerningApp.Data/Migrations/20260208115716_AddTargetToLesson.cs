using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTargetToLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OrderIndex",
                table: "Lessons",
                type: "int",
                nullable: false,
                comment: "The OrderIndex of the Lesson",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "The OrderIndex of the Course");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Lessons",
                type: "nvarchar(max)",
                maxLength: 10000,
                nullable: false,
                comment: "The Content of the Lesson",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 10000,
                oldComment: "The Content of the Course");

            migrationBuilder.AddColumn<string>(
                name: "Target",
                table: "Lessons",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                defaultValue: "",
                comment: "The Target of the Lesson");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Target",
                table: "Lessons");

            migrationBuilder.AlterColumn<int>(
                name: "OrderIndex",
                table: "Lessons",
                type: "int",
                nullable: false,
                comment: "The OrderIndex of the Course",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "The OrderIndex of the Lesson");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Lessons",
                type: "nvarchar(max)",
                maxLength: 10000,
                nullable: false,
                comment: "The Content of the Course",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 10000,
                oldComment: "The Content of the Lesson");
        }
    }
}
