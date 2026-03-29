using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.Migrations
{
    /// <inheritdoc />
    public partial class ModfyGamecodeModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UrlDataModels");

            migrationBuilder.CreateTable(
                name: "GameCodeModels",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Url = table.Column<string>(type: "TEXT", maxLength: 65535, nullable: true),
                    ContentType = table.Column<string>(type: "TEXT", nullable: false),
                    DiscordUser = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Ttl = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Publish = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameCodeModels", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameCodeModels");

            migrationBuilder.CreateTable(
                name: "UrlDataModels",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscordUser = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Publish = table.Column<bool>(type: "INTEGER", nullable: false),
                    Ttl = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 65535, nullable: true),
                    UrlType = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlDataModels", x => x.Id);
                });
        }
    }
}
