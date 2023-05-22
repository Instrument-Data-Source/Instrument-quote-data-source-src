using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Instrument.Quote.Source.Configuration.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "InstrumentTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Currency" },
                    { 2, "Stock" },
                    { 3, "CryptoCurrency" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "InstrumentTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "InstrumentTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "InstrumentTypes",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
