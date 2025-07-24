using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CB.Data.Migrations
{
    /// <inheritdoc />
    public partial class _72320251036p : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FunStuff",
                columns: table => new
                {
                    HaiBaiCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FunStuff");
        }
    }
}
