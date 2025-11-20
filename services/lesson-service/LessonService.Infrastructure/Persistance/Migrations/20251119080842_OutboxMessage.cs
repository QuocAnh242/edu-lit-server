using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonService.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class OutboxMessage : Migration
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
