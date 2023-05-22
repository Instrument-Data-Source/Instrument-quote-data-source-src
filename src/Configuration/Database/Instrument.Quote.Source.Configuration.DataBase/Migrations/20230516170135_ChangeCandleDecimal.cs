using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instrument.Quote.Source.Configuration.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCandleDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloseDecimal",
                table: "Candles");

            migrationBuilder.DropColumn(
                name: "CloseValue",
                table: "Candles");

            migrationBuilder.DropColumn(
                name: "HighDecimal",
                table: "Candles");

            migrationBuilder.DropColumn(
                name: "HighValue",
                table: "Candles");

            migrationBuilder.DropColumn(
                name: "LowDecimal",
                table: "Candles");

            migrationBuilder.RenameColumn(
                name: "VolumeValue",
                table: "Candles",
                newName: "VolumeStore");

            migrationBuilder.RenameColumn(
                name: "VolumeDecimal",
                table: "Candles",
                newName: "OpenStore");

            migrationBuilder.RenameColumn(
                name: "OpenValue",
                table: "Candles",
                newName: "LowStore");

            migrationBuilder.RenameColumn(
                name: "OpenDecimal",
                table: "Candles",
                newName: "HighStore");

            migrationBuilder.RenameColumn(
                name: "LowValue",
                table: "Candles",
                newName: "CloseStore");

            migrationBuilder.AddColumn<int>(
                name: "LoadedPeriodId",
                table: "Candles",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candles_LoadedPeriodId",
                table: "Candles",
                column: "LoadedPeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candles_LoadedPeriods_LoadedPeriodId",
                table: "Candles",
                column: "LoadedPeriodId",
                principalTable: "LoadedPeriods",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candles_LoadedPeriods_LoadedPeriodId",
                table: "Candles");

            migrationBuilder.DropIndex(
                name: "IX_Candles_LoadedPeriodId",
                table: "Candles");

            migrationBuilder.DropColumn(
                name: "LoadedPeriodId",
                table: "Candles");

            migrationBuilder.RenameColumn(
                name: "VolumeStore",
                table: "Candles",
                newName: "VolumeValue");

            migrationBuilder.RenameColumn(
                name: "OpenStore",
                table: "Candles",
                newName: "VolumeDecimal");

            migrationBuilder.RenameColumn(
                name: "LowStore",
                table: "Candles",
                newName: "OpenValue");

            migrationBuilder.RenameColumn(
                name: "HighStore",
                table: "Candles",
                newName: "OpenDecimal");

            migrationBuilder.RenameColumn(
                name: "CloseStore",
                table: "Candles",
                newName: "LowValue");

            migrationBuilder.AddColumn<int>(
                name: "CloseDecimal",
                table: "Candles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CloseValue",
                table: "Candles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HighDecimal",
                table: "Candles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HighValue",
                table: "Candles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LowDecimal",
                table: "Candles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
