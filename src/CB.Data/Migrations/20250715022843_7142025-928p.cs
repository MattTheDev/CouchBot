using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CB.Data.Migrations
{
    /// <inheritdoc />
    public partial class _7142025928p : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Users_UserId",
                table: "Guilds");

            migrationBuilder.DropIndex(
                name: "IX_Guilds_UserId",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Guilds");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_OwnerId",
                table: "Guilds",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Users_OwnerId",
                table: "Guilds",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Users_OwnerId",
                table: "Guilds");

            migrationBuilder.DropIndex(
                name: "IX_Guilds_OwnerId",
                table: "Guilds");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Guilds",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_UserId",
                table: "Guilds",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Users_UserId",
                table: "Guilds",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
