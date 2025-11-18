using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LessonService.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            // Tables are already created by existing database scripts
            // This migration just establishes EF Core tracking
            
            // If you need to create tables from scratch, uncomment the following:
            /*
            migrationBuilder.CreateTable(
                name: "syllabus",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    title = table.Column<string>(maxLength: 255, nullable: false),
                    academic_year = table.Column<string>(maxLength: 10, nullable: false),
                    semester = table.Column<string>(maxLength: 30, nullable: false),
                    description = table.Column<string>(nullable: true),
                    owner_id = table.Column<string>(maxLength: 36, nullable: false),
                    is_active = table.Column<bool>(nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("syllabus_pkey", x => x.id);
                });
            
            // ... other tables (courses, sessions, lesson_context, activity)
            */
            
            migrationBuilder.CreateTable(
                name: "outbox_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    type = table.Column<string>(maxLength: 255, nullable: false),
                    exchange = table.Column<string>(maxLength: 255, nullable: false),
                    payload = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    processed_at = table.Column<DateTime>(nullable: true),
                    error = table.Column<string>(type: "text", nullable: true),
                    retry_count = table.Column<int>(nullable: false, defaultValue: 0),
                    is_processed = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("outbox_message_pkey", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_outbox_is_processed",
                table: "outbox_messages",
                column: "is_processed");

            migrationBuilder.CreateIndex(
                name: "idx_outbox_created_at",
                table: "outbox_messages",
                column: "created_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "outbox_messages");
        }
    }
}

