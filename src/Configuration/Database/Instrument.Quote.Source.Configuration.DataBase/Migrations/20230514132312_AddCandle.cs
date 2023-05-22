using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Instrument.Quote.Source.Configuration.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddCandle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OpenValue = table.Column<int>(type: "integer", nullable: false),
                    OpenDecimal = table.Column<int>(type: "integer", nullable: false),
                    CloseValue = table.Column<int>(type: "integer", nullable: false),
                    CloseDecimal = table.Column<int>(type: "integer", nullable: false),
                    HighValue = table.Column<int>(type: "integer", nullable: false),
                    HighDecimal = table.Column<int>(type: "integer", nullable: false),
                    LowValue = table.Column<int>(type: "integer", nullable: false),
                    LowDecimal = table.Column<int>(type: "integer", nullable: false),
                    VolumeValue = table.Column<int>(type: "integer", nullable: false),
                    VolumeDecimal = table.Column<int>(type: "integer", nullable: false),
                    TimeFrameId = table.Column<int>(type: "integer", nullable: false),
                    InstrumentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candles_Instruments_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Candles_TimeFrames_TimeFrameId",
                        column: x => x.TimeFrameId,
                        principalTable: "TimeFrames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candles_InstrumentId",
                table: "Candles",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Candles_TimeFrameId",
                table: "Candles",
                column: "TimeFrameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candles");
        }
    }
}
