using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Instrument.Quote.Source.Configuration.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddJoinedChartsAggregate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candles_Instruments_InstrumentId",
                table: "Candles");

            migrationBuilder.DropForeignKey(
                name: "FK_Candles_TimeFrames_TimeFrameId",
                table: "Candles");

            migrationBuilder.DropIndex(
                name: "IX_Candles_InstrumentId",
                table: "Candles");

            migrationBuilder.DropIndex(
                name: "IX_Candles_TimeFrameId",
                table: "Candles");

            migrationBuilder.DropColumn(
                name: "InstrumentId",
                table: "Candles");

            migrationBuilder.DropColumn(
                name: "TimeFrameId",
                table: "Candles");

            migrationBuilder.CreateTable(
                name: "JoinedCharts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FromDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UntillDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TargetTimeFrameId = table.Column<int>(type: "integer", nullable: false),
                    BaseChartId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinedCharts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JoinedCharts_Charts_BaseChartId",
                        column: x => x.BaseChartId,
                        principalTable: "Charts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinedCharts_TimeFrames_TargetTimeFrameId",
                        column: x => x.TargetTimeFrameId,
                        principalTable: "TimeFrames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JoinedCandles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StepDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TargetDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Open = table.Column<int>(type: "integer", nullable: false),
                    Close = table.Column<int>(type: "integer", nullable: false),
                    High = table.Column<int>(type: "integer", nullable: false),
                    Low = table.Column<int>(type: "integer", nullable: false),
                    Volume = table.Column<int>(type: "integer", nullable: false),
                    IsLast = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedChartId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinedCandles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JoinedCandles_JoinedCharts_JoinedChartId",
                        column: x => x.JoinedChartId,
                        principalTable: "JoinedCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JoinedCandles_JoinedChartId_StepDateTime",
                table: "JoinedCandles",
                columns: new[] { "JoinedChartId", "StepDateTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JoinedCharts_BaseChartId_TargetTimeFrameId",
                table: "JoinedCharts",
                columns: new[] { "BaseChartId", "TargetTimeFrameId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JoinedCharts_TargetTimeFrameId",
                table: "JoinedCharts",
                column: "TargetTimeFrameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JoinedCandles");

            migrationBuilder.DropTable(
                name: "JoinedCharts");

            migrationBuilder.AddColumn<int>(
                name: "InstrumentId",
                table: "Candles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeFrameId",
                table: "Candles",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candles_InstrumentId",
                table: "Candles",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Candles_TimeFrameId",
                table: "Candles",
                column: "TimeFrameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candles_Instruments_InstrumentId",
                table: "Candles",
                column: "InstrumentId",
                principalTable: "Instruments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Candles_TimeFrames_TimeFrameId",
                table: "Candles",
                column: "TimeFrameId",
                principalTable: "TimeFrames",
                principalColumn: "Id");
        }
    }
}
