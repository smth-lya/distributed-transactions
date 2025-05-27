using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DT.Saga.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "saga");

            migrationBuilder.CreateTable(
                name: "saga_states",
                schema: "saga",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_state = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    saga_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    row_version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saga_states", x => x.correlation_id);
                });

            migrationBuilder.CreateTable(
                name: "saga_commands",
                schema: "saga",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    command_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    payload = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saga_commands", x => x.id);
                    table.ForeignKey(
                        name: "fk_saga_commands_states",
                        column: x => x.CorrelationId,
                        principalSchema: "saga",
                        principalTable: "saga_states",
                        principalColumn: "correlation_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "saga_events",
                schema: "saga",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    payload = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsProcessed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saga_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_saga_events_states",
                        column: x => x.CorrelationId,
                        principalSchema: "saga",
                        principalTable: "saga_states",
                        principalColumn: "correlation_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_saga_commands_correlation_status",
                schema: "saga",
                table: "saga_commands",
                columns: new[] { "CorrelationId", "status" });

            migrationBuilder.CreateIndex(
                name: "idx_saga_events_correlation_processed",
                schema: "saga",
                table: "saga_events",
                columns: new[] { "CorrelationId", "IsProcessed" });

            migrationBuilder.CreateIndex(
                name: "idx_saga_states_order_id",
                schema: "saga",
                table: "saga_states",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "idx_saga_states_type_completed",
                schema: "saga",
                table: "saga_states",
                columns: new[] { "saga_type", "IsCompleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "saga_commands",
                schema: "saga");

            migrationBuilder.DropTable(
                name: "saga_events",
                schema: "saga");

            migrationBuilder.DropTable(
                name: "saga_states",
                schema: "saga");
        }
    }
}
