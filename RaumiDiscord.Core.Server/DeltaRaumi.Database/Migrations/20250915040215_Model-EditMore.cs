using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.Migrations
{
    /// <inheritdoc />
    public partial class ModelEditMore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "guId",
                table: "UserGuildData",
                newName: "GuId");

            migrationBuilder.RenameColumn(
                name: "Latest_Exp",
                table: "UserGuildData",
                newName: "LatestExp");

            migrationBuilder.RenameColumn(
                name: "Guild_Exp",
                table: "UserGuildData",
                newName: "GuildExp");

            migrationBuilder.RenameColumn(
                name: "TTL",
                table: "UrlDataModels",
                newName: "Ttl");

            migrationBuilder.RenameColumn(
                name: "WelcomeChannnelID",
                table: "GuildBases",
                newName: "WelcomeChannnelId");

            migrationBuilder.RenameColumn(
                name: "OwnerID",
                table: "GuildBases",
                newName: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GuId",
                table: "UserGuildData",
                newName: "guId");

            migrationBuilder.RenameColumn(
                name: "LatestExp",
                table: "UserGuildData",
                newName: "Latest_Exp");

            migrationBuilder.RenameColumn(
                name: "GuildExp",
                table: "UserGuildData",
                newName: "Guild_Exp");

            migrationBuilder.RenameColumn(
                name: "Ttl",
                table: "UrlDataModels",
                newName: "TTL");

            migrationBuilder.RenameColumn(
                name: "WelcomeChannnelId",
                table: "GuildBases",
                newName: "WelcomeChannnelID");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "GuildBases",
                newName: "OwnerID");
        }
    }
}
