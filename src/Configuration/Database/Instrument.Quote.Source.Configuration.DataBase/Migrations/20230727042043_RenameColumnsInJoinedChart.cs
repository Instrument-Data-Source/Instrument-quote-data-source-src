using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instrument.Quote.Source.Configuration.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnsInJoinedChart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JoinedCharts_Charts_ChartId",
                table: "JoinedCharts");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinedCharts_TimeFrames_TimeFrameId",
                table: "JoinedCharts");

            migrationBuilder.RenameColumn(
                name: "TimeFrameId",
                table: "JoinedCharts",
                newName: "TargetTimeFrameId");

            migrationBuilder.RenameColumn(
                name: "ChartId",
                table: "JoinedCharts",
                newName: "StepChartId");

            migrationBuilder.RenameIndex(
                name: "IX_JoinedCharts_TimeFrameId",
                table: "JoinedCharts",
                newName: "IX_JoinedCharts_TargetTimeFrameId");

            migrationBuilder.RenameIndex(
                name: "IX_JoinedCharts_ChartId_TimeFrameId",
                table: "JoinedCharts",
                newName: "IX_JoinedCharts_StepChartId_TargetTimeFrameId");

            migrationBuilder.AddForeignKey(
                name: "FK_JoinedCharts_Charts_StepChartId",
                table: "JoinedCharts",
                column: "StepChartId",
                principalTable: "Charts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinedCharts_TimeFrames_TargetTimeFrameId",
                table: "JoinedCharts",
                column: "TargetTimeFrameId",
                principalTable: "TimeFrames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JoinedCharts_Charts_StepChartId",
                table: "JoinedCharts");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinedCharts_TimeFrames_TargetTimeFrameId",
                table: "JoinedCharts");

            migrationBuilder.RenameColumn(
                name: "TargetTimeFrameId",
                table: "JoinedCharts",
                newName: "TimeFrameId");

            migrationBuilder.RenameColumn(
                name: "StepChartId",
                table: "JoinedCharts",
                newName: "ChartId");

            migrationBuilder.RenameIndex(
                name: "IX_JoinedCharts_TargetTimeFrameId",
                table: "JoinedCharts",
                newName: "IX_JoinedCharts_TimeFrameId");

            migrationBuilder.RenameIndex(
                name: "IX_JoinedCharts_StepChartId_TargetTimeFrameId",
                table: "JoinedCharts",
                newName: "IX_JoinedCharts_ChartId_TimeFrameId");

            migrationBuilder.AddForeignKey(
                name: "FK_JoinedCharts_Charts_ChartId",
                table: "JoinedCharts",
                column: "ChartId",
                principalTable: "Charts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinedCharts_TimeFrames_TimeFrameId",
                table: "JoinedCharts",
                column: "TimeFrameId",
                principalTable: "TimeFrames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
