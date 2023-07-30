using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instrument.Quote.Source.Configuration.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnFullCalc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullCalc",
                table: "JoinedCandles",
                newName: "IsFullCalc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsFullCalc",
                table: "JoinedCandles",
                newName: "FullCalc");
        }
    }
}
