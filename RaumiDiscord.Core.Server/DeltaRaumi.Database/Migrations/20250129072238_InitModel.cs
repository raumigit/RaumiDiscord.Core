using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaumiDiscord.Core.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    CustomId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MessageId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    DeltaRaumiComponentType = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.CustomId);
                });

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
                name: "UrlDataModels",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    UrlType = table.Column<string>(type: "TEXT", nullable: true),
                    TTL = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlDataModels", x => x.Id);
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
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false),
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
                        name: "FK_UserGuildData_GuildBases_GuildId",
                        column: x => x.GuildId,
                        principalTable: "GuildBases",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGuildData_UserBases_UserId",
                        column: x => x.UserId,
                        principalTable: "UserBases",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGuildData_GuildId",
                table: "UserGuildData",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGuildData_UserId",
                table: "UserGuildData",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Components");

            migrationBuilder.DropTable(
                name: "UrlDataModels");

            migrationBuilder.DropTable(
                name: "UserGuildData");

            migrationBuilder.DropTable(
                name: "GuildBases");

            migrationBuilder.DropTable(
                name: "UserBases");
        }
    }
}
