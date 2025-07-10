using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CB.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCreators : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Users_UserId",
                table: "Guilds");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Guilds",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Guilds_UserId",
                table: "Guilds",
                newName: "IX_Guilds_OwnerId");

            migrationBuilder.CreateTable(
                name: "Creators",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChannelId = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    PlatformId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    IsLive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Creators_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Creators_UserId",
                table: "Creators",
                column: "UserId");

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

            migrationBuilder.DropTable(
                name: "Creators");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Guilds",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Guilds_OwnerId",
                table: "Guilds",
                newName: "IX_Guilds_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Users_UserId",
                table: "Guilds",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
