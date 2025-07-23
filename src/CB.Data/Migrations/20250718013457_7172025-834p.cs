using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CB.Data.Migrations
{
    /// <inheritdoc />
    public partial class _7172025834p : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FilterType",
                columns: new[] { "Id", "DisplayName" },
                values: new object[,]
                {
                    { 1, "Game" },
                    { 2, "Title" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FilterType",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FilterType",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
