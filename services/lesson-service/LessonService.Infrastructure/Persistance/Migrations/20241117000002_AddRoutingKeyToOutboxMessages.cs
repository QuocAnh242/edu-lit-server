using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonService.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddRoutingKeyToOutboxMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "routing_key",
                table: "outbox_messages",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            // Update existing records to have default routing key pattern
            migrationBuilder.Sql(@"
                UPDATE outbox_messages 
                SET routing_key = CASE
                    WHEN type LIKE '%Created' THEN LOWER(REPLACE(REPLACE(type, 'Created', ''), 'Syllabus', 'syllabus')) || '.created'
                    WHEN type LIKE '%Updated' THEN LOWER(REPLACE(REPLACE(type, 'Updated', ''), 'Syllabus', 'syllabus')) || '.updated'
                    WHEN type LIKE '%Deleted' THEN LOWER(REPLACE(REPLACE(type, 'Deleted', ''), 'Syllabus', 'syllabus')) || '.deleted'
                    ELSE '#'
                END
                WHERE routing_key = '';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "routing_key",
                table: "outbox_messages");
        }
    }
}

