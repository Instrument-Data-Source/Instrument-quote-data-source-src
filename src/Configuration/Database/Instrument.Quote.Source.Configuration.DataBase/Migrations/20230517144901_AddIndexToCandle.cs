using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instrument.Quote.Source.Configuration.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexToCandle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Candles_InstrumentId",
                table: "Candles");

            migrationBuilder.CreateIndex(
                name: "IX_Candles_InstrumentId_TimeFrameId_DateTime",
                table: "Candles",
                columns: new[] { "InstrumentId", "TimeFrameId", "DateTime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Candles_InstrumentId_TimeFrameId_DateTime",
                table: "Candles");

            migrationBuilder.CreateIndex(
                name: "IX_Candles_InstrumentId",
                table: "Candles",
                column: "InstrumentId");
        }
    }
}
