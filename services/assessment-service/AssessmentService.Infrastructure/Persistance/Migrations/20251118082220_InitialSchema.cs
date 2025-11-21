using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssessmentService.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "latin1");

            migrationBuilder.CreateTable(
                name: "assessments",
                columns: table => new
                {
                    assessment_id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    course_id = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    creator_id = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    title = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    description = table.Column<string>(type: "longtext", nullable: true, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    total_questions = table.Column<int>(type: "int(11)", nullable: false),
                    duration_minutes = table.Column<int>(type: "int(11)", nullable: false),
                    status = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'1'"),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.assessment_id);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "assessment_question",
                columns: table => new
                {
                    assessment_question_id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    assessment_id = table.Column<int>(type: "int(11)", nullable: false),
                    question_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, comment: "Reference to Question Service", collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'1'"),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.assessment_question_id);
                    table.ForeignKey(
                        name: "assessment_question_ibfk_1",
                        column: x => x.assessment_id,
                        principalTable: "assessments",
                        principalColumn: "assessment_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "assignment_attempts",
                columns: table => new
                {
                    attempts_id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    assessment_id = table.Column<int>(type: "int(11)", nullable: false),
                    user_id = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    started_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    completed_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    attempt_number = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'1'"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.attempts_id);
                    table.ForeignKey(
                        name: "assignment_attempts_ibfk_1",
                        column: x => x.assessment_id,
                        principalTable: "assessments",
                        principalColumn: "assessment_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "assessment_answer",
                columns: table => new
                {
                    answer_id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    assessment_question_id = table.Column<int>(type: "int(11)", nullable: false),
                    attempts_id = table.Column<int>(type: "int(11)", nullable: false),
                    selected_option_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, comment: "Reference to QuestionOption in Question Service", collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    is_correct = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.answer_id);
                    table.ForeignKey(
                        name: "assessment_answer_ibfk_1",
                        column: x => x.assessment_question_id,
                        principalTable: "assessment_question",
                        principalColumn: "assessment_question_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "assessment_answer_ibfk_2",
                        column: x => x.attempts_id,
                        principalTable: "assignment_attempts",
                        principalColumn: "attempts_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "grading_feedback",
                columns: table => new
                {
                    feedback_id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    attempts_id = table.Column<int>(type: "int(11)", nullable: false),
                    total_score = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false, comment: "Tổng điểm trên thang 10"),
                    correct_count = table.Column<int>(type: "int(11)", nullable: false, comment: "Số câu trả lời đúng"),
                    wrong_count = table.Column<int>(type: "int(11)", nullable: false, comment: "Số câu trả lời sai"),
                    correct_percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false, comment: "Phần trăm câu đúng (%)"),
                    wrong_percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false, comment: "Phần trăm câu sai (%)"),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.feedback_id);
                    table.ForeignKey(
                        name: "grading_feedback_ibfk_1",
                        column: x => x.attempts_id,
                        principalTable: "assignment_attempts",
                        principalColumn: "attempts_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateIndex(
                name: "assessment_question_id",
                table: "assessment_answer",
                column: "assessment_question_id");

            migrationBuilder.CreateIndex(
                name: "attempts_id",
                table: "assessment_answer",
                column: "attempts_id");

            migrationBuilder.CreateIndex(
                name: "idx_selected_option",
                table: "assessment_answer",
                column: "selected_option_id");

            migrationBuilder.CreateIndex(
                name: "assessment_id",
                table: "assessment_question",
                column: "assessment_id");

            migrationBuilder.CreateIndex(
                name: "idx_assessment_question",
                table: "assessment_question",
                columns: new[] { "assessment_id", "question_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "assessment_id1",
                table: "assignment_attempts",
                column: "assessment_id");

            migrationBuilder.CreateIndex(
                name: "attempts_id1",
                table: "grading_feedback",
                column: "attempts_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assessment_answer");

            migrationBuilder.DropTable(
                name: "grading_feedback");

            migrationBuilder.DropTable(
                name: "assessment_question");

            migrationBuilder.DropTable(
                name: "assignment_attempts");

            migrationBuilder.DropTable(
                name: "assessments");
        }
    }
}
