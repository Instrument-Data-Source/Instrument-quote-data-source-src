using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instrument.Quote.Source.Configuration.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LoadedPeriods_InstrumentId",
                table: "LoadedPeriods");

            migrationBuilder.CreateIndex(
                name: "IX_LoadedPeriods_InstrumentId_TimeFrameId",
                table: "LoadedPeriods",
                columns: new[] { "InstrumentId", "TimeFrameId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LoadedPeriods_InstrumentId_TimeFrameId",
                table: "LoadedPeriods");

            migrationBuilder.CreateIndex(
                name: "IX_LoadedPeriods_InstrumentId",
                table: "LoadedPeriods",
                column: "InstrumentId");
        }
    }
}
