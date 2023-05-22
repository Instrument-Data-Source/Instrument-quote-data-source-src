using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Instrument.Quote.Source.Configuration.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeFrame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimeFrames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Seconds = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeFrames", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TimeFrames",
                columns: new[] { "Id", "Name", "Seconds" },
                values: new object[,]
                {
                    { 1, "M", 2592000 },
                    { 2, "W1", 604800 },
                    { 3, "D1", 86400 },
                    { 4, "H4", 14400 },
                    { 5, "H1", 3600 },
                    { 6, "m30", 1800 },
                    { 7, "m15", 900 },
                    { 8, "m10", 600 },
                    { 9, "m5", 300 },
                    { 10, "m1", 60 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeFrames");
        }
    }
}
