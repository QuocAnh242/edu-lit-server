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

            // First, add a temporary column for the new integer values
            migrationBuilder.AddColumn<int>(
                name: "question_type_new",
                schema: "public",
                table: "questions",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            // Convert existing string values to integer enum values
            migrationBuilder.Sql(@"
                UPDATE public.questions 
                SET question_type_new = CASE 
                    WHEN question_type = 'Paragraph' THEN 1
                    WHEN question_type = 'Multichoice' THEN 2
                    ELSE 1
                END");

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "question_type",
                schema: "public",
                table: "questions");

            // Rename the new column to the original name
            migrationBuilder.RenameColumn(
                name: "question_type_new",
                schema: "public",
                table: "questions",
                newName: "question_type");
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

            // First, add a temporary column for the new string values
            migrationBuilder.AddColumn<string>(
                name: "question_type_new",
                table: "questions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Paragraph");

            // Convert existing integer values back to string enum values
            migrationBuilder.Sql(@"
                UPDATE questions 
                SET question_type_new = CASE 
                    WHEN question_type = 1 THEN 'Paragraph'
                    WHEN question_type = 2 THEN 'Multichoice'
                    ELSE 'Paragraph'
                END");

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "question_type",
                table: "questions");

            // Rename the new column to the original name
            migrationBuilder.RenameColumn(
                name: "question_type_new",
                table: "questions",
                newName: "question_type");
        }
    }
}
