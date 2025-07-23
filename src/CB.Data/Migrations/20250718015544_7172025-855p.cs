using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CB.Data.Migrations
{
    /// <inheritdoc />
    public partial class _7172025855p : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Filters_Platform_PlatformId",
                table: "Filters");

            migrationBuilder.AlterColumn<int>(
                name: "PlatformId",
                table: "Filters",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.InsertData(
                table: "Platform",
                columns: new[] { "Id", "DisplayName", "Enabled", "LogoUrl", "SiteUrl" },
                values: new object[] { 1, "All", false, null, null });

            migrationBuilder.AddForeignKey(
                name: "FK_Filters_Platform_PlatformId",
                table: "Filters",
                column: "PlatformId",
                principalTable: "Platform",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Filters_Platform_PlatformId",
                table: "Filters");

            migrationBuilder.DeleteData(
                table: "Platform",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "PlatformId",
                table: "Filters",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Filters_Platform_PlatformId",
                table: "Filters",
                column: "PlatformId",
                principalTable: "Platform",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
