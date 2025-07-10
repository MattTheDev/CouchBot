using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CB.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigsAndChannel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "AllowConfiguration",
                columns: table => new
                {
                    GuildId = table.Column<string>(type: "text", nullable: false),
                    AllowLive = table.Column<bool>(type: "boolean", nullable: false),
                    AllowPublished = table.Column<bool>(type: "boolean", nullable: false),
                    AllowThumbnails = table.Column<bool>(type: "boolean", nullable: false),
                    AllowGreetings = table.Column<bool>(type: "boolean", nullable: false),
                    AllowGoodbyes = table.Column<bool>(type: "boolean", nullable: false),
                    AllowLiveDiscovery = table.Column<bool>(type: "boolean", nullable: false),
                    AllowStreamVod = table.Column<bool>(type: "boolean", nullable: false),
                    AllowFfa = table.Column<bool>(type: "boolean", nullable: false),
                    AllowCrosspost = table.Column<bool>(type: "boolean", nullable: false),
                    AllowDiscordLive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowConfiguration", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_AllowConfiguration_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    GuildId = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Channels_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GuildConfigurations",
                columns: table => new
                {
                    GuildId = table.Column<string>(type: "text", nullable: false),
                    TextAnnouncements = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteOffline = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildConfigurations", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_GuildConfigurations_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageConfigurations",
                columns: table => new
                {
                    GuildId = table.Column<string>(type: "text", nullable: false),
                    GreetingMessage = table.Column<string>(type: "text", nullable: false),
                    GoodbyeMessage = table.Column<string>(type: "text", nullable: false),
                    LiveMessage = table.Column<string>(type: "text", nullable: false),
                    PublishedMessage = table.Column<string>(type: "text", nullable: false),
                    StreamOfflineMessage = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageConfigurations", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_MessageConfigurations_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleConfigurations",
                columns: table => new
                {
                    GuildId = table.Column<string>(type: "text", nullable: false),
                    JoinRoleId = table.Column<string>(type: "text", nullable: false),
                    DiscoveryRoleId = table.Column<string>(type: "text", nullable: false),
                    LiveDiscoveryRoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleConfigurations", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_RoleConfigurations_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelConfigurations",
                columns: table => new
                {
                    GuildId = table.Column<string>(type: "text", nullable: false),
                    GreetingChannelId = table.Column<string>(type: "text", nullable: true),
                    GoodbyeChannelId = table.Column<string>(type: "text", nullable: true),
                    LiveChannelId = table.Column<string>(type: "text", nullable: true),
                    DiscordLiveChannelId = table.Column<string>(type: "text", nullable: true),
                    ChannelId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelConfigurations", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_ChannelConfigurations_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChannelConfigurations_Channels_DiscordLiveChannelId",
                        column: x => x.DiscordLiveChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChannelConfigurations_Channels_GoodbyeChannelId",
                        column: x => x.GoodbyeChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChannelConfigurations_Channels_GreetingChannelId",
                        column: x => x.GreetingChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChannelConfigurations_Channels_LiveChannelId",
                        column: x => x.LiveChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChannelConfigurations_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_UserId",
                table: "Guilds",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelConfigurations_ChannelId",
                table: "ChannelConfigurations",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelConfigurations_DiscordLiveChannelId",
                table: "ChannelConfigurations",
                column: "DiscordLiveChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelConfigurations_GoodbyeChannelId",
                table: "ChannelConfigurations",
                column: "GoodbyeChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelConfigurations_GreetingChannelId",
                table: "ChannelConfigurations",
                column: "GreetingChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelConfigurations_LiveChannelId",
                table: "ChannelConfigurations",
                column: "LiveChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_GuildId",
                table: "Channels",
                column: "GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Users_UserId",
                table: "Guilds",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Users_UserId",
                table: "Guilds");

            migrationBuilder.DropTable(
                name: "AllowConfiguration");

            migrationBuilder.DropTable(
                name: "ChannelConfigurations");

            migrationBuilder.DropTable(
                name: "GuildConfigurations");

            migrationBuilder.DropTable(
                name: "MessageConfigurations");

            migrationBuilder.DropTable(
                name: "RoleConfigurations");

            migrationBuilder.DropTable(
                name: "Channels");

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
    }
}
