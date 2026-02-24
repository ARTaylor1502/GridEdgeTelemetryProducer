using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GridEdge.Telemetry.Consumer.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "MeterReadings",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                MeterId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                UsageKwh = table.Column<double>(type: "double precision", nullable: false),
                Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_MeterReadings", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_MeterReadings_Timestamp",
            table: "MeterReadings",
            column: "Timestamp");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "MeterReadings");
    }
}
