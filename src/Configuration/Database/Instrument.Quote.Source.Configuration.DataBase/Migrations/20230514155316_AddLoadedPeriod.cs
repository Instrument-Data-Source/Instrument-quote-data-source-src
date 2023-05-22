using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Instrument.Quote.Source.Configuration.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddLoadedPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoadedPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FromDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UntillDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeFrameId = table.Column<int>(type: "integer", nullable: false),
                    InstrumentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadedPeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoadedPeriods_Instruments_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoadedPeriods_TimeFrames_TimeFrameId",
                        column: x => x.TimeFrameId,
                        principalTable: "TimeFrames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoadedPeriods_InstrumentId",
                table: "LoadedPeriods",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_LoadedPeriods_TimeFrameId",
                table: "LoadedPeriods",
                column: "TimeFrameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoadedPeriods");
        }
    }
}
