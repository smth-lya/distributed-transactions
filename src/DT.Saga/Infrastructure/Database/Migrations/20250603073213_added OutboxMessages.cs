using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DT.Saga.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class addedOutboxMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "messaging");

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                schema: "messaging",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    message_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    retry_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    exchange = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    routing_key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_correlation_id",
                schema: "messaging",
                table: "outbox_messages",
                column: "correlation_id");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_created_at",
                schema: "messaging",
                table: "outbox_messages",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_message_type",
                schema: "messaging",
                table: "outbox_messages",
                column: "message_type");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_processed_at",
                schema: "messaging",
                table: "outbox_messages",
                column: "processed_at",
                filter: "processed_at IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_messages",
                schema: "messaging");
        }
    }
}
