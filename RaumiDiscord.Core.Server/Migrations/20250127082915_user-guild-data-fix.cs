using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaumiDiscord.Core.Server.Migrations
{
    /// <inheritdoc />
    public partial class userguilddatafix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGuildData_GuildBases_Guildid",
                table: "UserGuildData");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGuildData_UserBases_Userid",
                table: "UserGuildData");

            migrationBuilder.RenameColumn(
                name: "Userid",
                table: "UserGuildData",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Guildid",
                table: "UserGuildData",
                newName: "GuildId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGuildData_Userid",
                table: "UserGuildData",
                newName: "IX_UserGuildData_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGuildData_Guildid",
                table: "UserGuildData",
                newName: "IX_UserGuildData_GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGuildData_GuildBases_GuildId",
                table: "UserGuildData",
                column: "GuildId",
                principalTable: "GuildBases",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGuildData_UserBases_UserId",
                table: "UserGuildData",
                column: "UserId",
                principalTable: "UserBases",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGuildData_GuildBases_GuildId",
                table: "UserGuildData");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGuildData_UserBases_UserId",
                table: "UserGuildData");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserGuildData",
                newName: "Userid");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "UserGuildData",
                newName: "Guildid");

            migrationBuilder.RenameIndex(
                name: "IX_UserGuildData_UserId",
                table: "UserGuildData",
                newName: "IX_UserGuildData_Userid");

            migrationBuilder.RenameIndex(
                name: "IX_UserGuildData_GuildId",
                table: "UserGuildData",
                newName: "IX_UserGuildData_Guildid");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGuildData_GuildBases_Guildid",
                table: "UserGuildData",
                column: "Guildid",
                principalTable: "GuildBases",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGuildData_UserBases_Userid",
                table: "UserGuildData",
                column: "Userid",
                principalTable: "UserBases",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
