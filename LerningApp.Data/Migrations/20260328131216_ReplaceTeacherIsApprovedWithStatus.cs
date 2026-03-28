using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceTeacherIsApprovedWithStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Teachers");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Teachers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Teachers");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Teachers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
