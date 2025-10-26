using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuestionTypeToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "questions",
                newName: "questions",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "question_options",
                newName: "question_options",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "question_banks",
                newName: "question_banks",
                newSchema: "public");

            migrationBuilder.AlterColumn<int>(
                name: "question_type",
                schema: "public",
                table: "questions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "questions",
                schema: "public",
                newName: "questions");

            migrationBuilder.RenameTable(
                name: "question_options",
                schema: "public",
                newName: "question_options");

            migrationBuilder.RenameTable(
                name: "question_banks",
                schema: "public",
                newName: "question_banks");

            migrationBuilder.AlterColumn<string>(
                name: "question_type",
                table: "questions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
