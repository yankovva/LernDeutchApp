using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingTeacherProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PendingBiography",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingFirstName",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingLastName",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingPhoneNumber",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingProfileImage",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingQualification",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PendingBiography",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "PendingFirstName",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "PendingLastName",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "PendingPhoneNumber",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "PendingProfileImage",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "PendingQualification",
                table: "Teachers");
        }
    }
}
