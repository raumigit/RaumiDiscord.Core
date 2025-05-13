using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixModelsDiscordLevelService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GuildLebel",
                table: "UserGuildData",
                newName: "Guild_Exp");

            migrationBuilder.RenameColumn(
                name: "Isbot",
                table: "UserBases",
                newName: "IsBot");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "UserBases",
                newName: "Exp");

            migrationBuilder.AddColumn<DateTime>(
                name: "Latest_Exp",
                table: "UserGuildData",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Publish",
                table: "UrlDataModels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LogChannel",
                table: "GuildBases",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latest_Exp",
                table: "UserGuildData");

            migrationBuilder.DropColumn(
                name: "Publish",
                table: "UrlDataModels");

            migrationBuilder.DropColumn(
                name: "LogChannel",
                table: "GuildBases");

            migrationBuilder.RenameColumn(
                name: "Guild_Exp",
                table: "UserGuildData",
                newName: "GuildLebel");

            migrationBuilder.RenameColumn(
                name: "IsBot",
                table: "UserBases",
                newName: "Isbot");

            migrationBuilder.RenameColumn(
                name: "Exp",
                table: "UserBases",
                newName: "Level");
        }
    }
}
