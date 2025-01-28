using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaumiDiscord.Core.Server.Migrations
{
    /// <inheritdoc />
    public partial class userguilddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildBases",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildName = table.Column<string>(type: "TEXT", nullable: false),
                    IconUrl = table.Column<string>(type: "TEXT", nullable: true),
                    BannerUrl = table.Column<string>(type: "TEXT", nullable: true),
                    OwnerID = table.Column<ulong>(type: "INTEGER", nullable: false),
                    WelcomeChannnelID = table.Column<ulong>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    MaxUploadLimit = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MemberCount = table.Column<int>(type: "INTEGER", nullable: false),
                    PremiumSubscriptionCount = table.Column<int>(type: "INTEGER", nullable: false),
                    PremiumTier = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildBases", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "UserBases",
                columns: table => new
                {
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    AvatarId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Isbot = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsWebhook = table.Column<bool>(type: "INTEGER", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Barthday = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBases", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "UserGuildData",
                columns: table => new
                {
                    Guildid = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Userid = table.Column<ulong>(type: "INTEGER", nullable: false),
                    GuildAvatarId = table.Column<string>(type: "TEXT", nullable: true),
                    GuildUserFlags = table.Column<int>(type: "INTEGER", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TimedOutUntil = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuildLebel = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalMessage = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_UserGuildData_GuildBases_Guildid",
                        column: x => x.Guildid,
                        principalTable: "GuildBases",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGuildData_UserBases_Userid",
                        column: x => x.Userid,
                        principalTable: "UserBases",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGuildData_Guildid",
                table: "UserGuildData",
                column: "Guildid");

            migrationBuilder.CreateIndex(
                name: "IX_UserGuildData_Userid",
                table: "UserGuildData",
                column: "Userid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGuildData");

            migrationBuilder.DropTable(
                name: "GuildBases");

            migrationBuilder.DropTable(
                name: "UserBases");
        }
    }
}
