using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CB.Data.Migrations
{
    /// <inheritdoc />
    public partial class PendingMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChannelConfigurations_Channels_ChannelId",
                table: "ChannelConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_ChannelConfigurations_ChannelId",
                table: "ChannelConfigurations");

            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "ChannelConfigurations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChannelId",
                table: "ChannelConfigurations",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelConfigurations_ChannelId",
                table: "ChannelConfigurations",
                column: "ChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelConfigurations_Channels_ChannelId",
                table: "ChannelConfigurations",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id");
        }
    }
}
