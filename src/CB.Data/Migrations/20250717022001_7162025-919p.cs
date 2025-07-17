using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CB.Data.Migrations
{
    /// <inheritdoc />
    public partial class _7162025919p : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllowConfiguration_Guilds_GuildId",
                table: "AllowConfiguration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AllowConfiguration",
                table: "AllowConfiguration");

            migrationBuilder.RenameTable(
                name: "AllowConfiguration",
                newName: "AllowConfigurations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AllowConfigurations",
                table: "AllowConfigurations",
                column: "GuildId");

            migrationBuilder.CreateTable(
                name: "CreatorChannels",
                columns: table => new
                {
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    ChannelId = table.Column<string>(type: "text", nullable: false),
                    ChannelTypeId = table.Column<int>(type: "integer", nullable: false),
                    CustomMessage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorChannels", x => new { x.CreatorId, x.ChannelId });
                    table.ForeignKey(
                        name: "FK_CreatorChannels_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreatorChannels_Creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DropdownPayloads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Payload = table.Column<string>(type: "text", nullable: true),
                    DropdownType = table.Column<string>(type: "text", nullable: true),
                    OriginalMessageId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DropdownPayloads", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreatorChannels_ChannelId",
                table: "CreatorChannels",
                column: "ChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AllowConfigurations_Guilds_GuildId",
                table: "AllowConfigurations",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllowConfigurations_Guilds_GuildId",
                table: "AllowConfigurations");

            migrationBuilder.DropTable(
                name: "CreatorChannels");

            migrationBuilder.DropTable(
                name: "DropdownPayloads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AllowConfigurations",
                table: "AllowConfigurations");

            migrationBuilder.RenameTable(
                name: "AllowConfigurations",
                newName: "AllowConfiguration");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AllowConfiguration",
                table: "AllowConfiguration",
                column: "GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_AllowConfiguration_Guilds_GuildId",
                table: "AllowConfiguration",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
