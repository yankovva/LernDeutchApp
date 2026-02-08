using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LerningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGenderAndExampleSentenceToVocabularyTermAndComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "VocabularyTerms");

            migrationBuilder.AlterColumn<Guid>(
                name: "VocabularyCardId",
                table: "VocabularyTerms",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Foreign key to VocabularyCard",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Side",
                table: "VocabularyTerms",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                comment: "The Side of the VocabularyTerm(en/de)",
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPrimary",
                table: "VocabularyTerms",
                type: "bit",
                nullable: false,
                comment: "Is the VocabularyTerm primary for the VocabularyCard",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "VocabularyTerms",
                type: "uniqueidentifier",
                nullable: false,
                comment: "PK Unique Identifier",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "ExampleSentence",
                table: "VocabularyTerms",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "Example Sentence for the VocabularyTerm in German");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "VocabularyTerms",
                type: "nchar(3)",
                fixedLength: true,
                maxLength: 3,
                nullable: true,
                comment: "German gender (der/die/das), only for Side = 'de'");

            migrationBuilder.AddColumn<string>(
                name: "Word",
                table: "VocabularyTerms",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "",
                comment: "The Word of the VocabularyTerm");

            migrationBuilder.AlterColumn<Guid>(
                name: "PartOfSpeechId",
                table: "VocabularyCards",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Foreign key to PartOfSpeech",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "LessonId",
                table: "VocabularyCards",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Foreign key to Lesson",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "VocabularyCards",
                type: "uniqueidentifier",
                nullable: false,
                comment: "PK Unique Identifier",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "UsersCourses",
                type: "datetime2",
                nullable: false,
                comment: "Course started at (UTC)",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "UsersCourses",
                type: "datetime2",
                nullable: true,
                comment: "Course completed at (UTC)",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CourseId",
                table: "UsersCourses",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Foreign key to Course",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "UsersCourses",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Foreign key to user",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PartsOfSpeech",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                comment: "The Name of the PartOfSpeech",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PartsOfSpeech",
                type: "uniqueidentifier",
                nullable: false,
                comment: "PK Unique Identifier",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Levels",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                comment: "The Name of the Level",
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Levels",
                type: "nvarchar(1500)",
                maxLength: 1500,
                nullable: false,
                comment: "The Description of the Level",
                oldClrType: typeof(string),
                oldType: "nvarchar(1500)",
                oldMaxLength: 1500);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Levels",
                type: "uniqueidentifier",
                nullable: false,
                comment: "PK Unique Identifier",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "OrderIndex",
                table: "Lessons",
                type: "int",
                nullable: false,
                comment: "The OrderIndex of the Course",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Lessons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "The Name of the Lesson",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Lessons",
                type: "datetime2",
                nullable: false,
                comment: "The Creation Date of the Lesson",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourseId",
                table: "Lessons",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Foreign key to Course",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Lessons",
                type: "nvarchar(max)",
                maxLength: 10000,
                nullable: false,
                comment: "The Content of the Course",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 10000);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Lessons",
                type: "uniqueidentifier",
                nullable: false,
                comment: "PK Unique Identifier",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Courses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "The Name of the Course",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "LevelId",
                table: "Courses",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Foreign key to Level",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPublished",
                table: "Courses",
                type: "bit",
                nullable: false,
                comment: "The status of the Course",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Courses",
                type: "nvarchar(3000)",
                maxLength: 3000,
                nullable: false,
                comment: "The Description of the Course",
                oldClrType: typeof(string),
                oldType: "nvarchar(3000)",
                oldMaxLength: 3000);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Courses",
                type: "datetime2",
                nullable: false,
                comment: "The Creation Date of the Course",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Courses",
                type: "uniqueidentifier",
                nullable: false,
                comment: "PK Unique Identifier",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExampleSentence",
                table: "VocabularyTerms");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "VocabularyTerms");

            migrationBuilder.DropColumn(
                name: "Word",
                table: "VocabularyTerms");

            migrationBuilder.AlterColumn<Guid>(
                name: "VocabularyCardId",
                table: "VocabularyTerms",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key to VocabularyCard");

            migrationBuilder.AlterColumn<string>(
                name: "Side",
                table: "VocabularyTerms",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5,
                oldComment: "The Side of the VocabularyTerm(en/de)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPrimary",
                table: "VocabularyTerms",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Is the VocabularyTerm primary for the VocabularyCard");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "VocabularyTerms",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "PK Unique Identifier");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "VocabularyTerms",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "PartOfSpeechId",
                table: "VocabularyCards",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key to PartOfSpeech");

            migrationBuilder.AlterColumn<Guid>(
                name: "LessonId",
                table: "VocabularyCards",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key to Lesson");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "VocabularyCards",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "PK Unique Identifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "UsersCourses",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Course started at (UTC)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "UsersCourses",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Course completed at (UTC)");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourseId",
                table: "UsersCourses",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key to Course");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "UsersCourses",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key to user");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PartsOfSpeech",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldComment: "The Name of the PartOfSpeech");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PartsOfSpeech",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "PK Unique Identifier");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Levels",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3,
                oldComment: "The Name of the Level");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Levels",
                type: "nvarchar(1500)",
                maxLength: 1500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1500)",
                oldMaxLength: 1500,
                oldComment: "The Description of the Level");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Levels",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "PK Unique Identifier");

            migrationBuilder.AlterColumn<int>(
                name: "OrderIndex",
                table: "Lessons",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "The OrderIndex of the Course");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Lessons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "The Name of the Lesson");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Lessons",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "The Creation Date of the Lesson");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourseId",
                table: "Lessons",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Foreign key to Course");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Lessons",
                type: "nvarchar(max)",
                maxLength: 10000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 10000,
                oldComment: "The Content of the Course");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Lessons",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "PK Unique Identifier");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Courses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "The Name of the Course");

            migrationBuilder.AlterColumn<Guid>(
                name: "LevelId",
                table: "Courses",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key to Level");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPublished",
                table: "Courses",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "The status of the Course");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Courses",
                type: "nvarchar(3000)",
                maxLength: 3000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3000)",
                oldMaxLength: 3000,
                oldComment: "The Description of the Course");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Courses",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "The Creation Date of the Course");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Courses",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "PK Unique Identifier");
        }
    }
}
