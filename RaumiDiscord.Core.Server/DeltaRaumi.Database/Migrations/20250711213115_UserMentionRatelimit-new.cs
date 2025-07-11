using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.Migrations
{
    /// <inheritdoc />
    public partial class UserMentionRatelimitnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StatUlid",
                table: "UserGuildStats",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AddColumn<int>(
                name: "SetToMention",
                table: "UserGuildData",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SetToMention",
                table: "UserBases",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Userstatus",
                table: "UserBases",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LinkCode",
                table: "Components",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeToLive",
                table: "Components",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MentionedUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MentionedUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserGuildStatsModelStatUlid = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentionedUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MentionedUsers_UserGuildStats_UserGuildStatsModelStatUlid",
                        column: x => x.UserGuildStatsModelStatUlid,
                        principalTable: "UserGuildStats",
                        principalColumn: "StatUlid");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MentionedUsers_UserGuildStatsModelStatUlid",
                table: "MentionedUsers",
                column: "UserGuildStatsModelStatUlid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MentionedUsers");

            migrationBuilder.DropColumn(
                name: "SetToMention",
                table: "UserGuildData");

            migrationBuilder.DropColumn(
                name: "SetToMention",
                table: "UserBases");

            migrationBuilder.DropColumn(
                name: "Userstatus",
                table: "UserBases");

            migrationBuilder.DropColumn(
                name: "LinkCode",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "TimeToLive",
                table: "Components");

            migrationBuilder.AlterColumn<byte[]>(
                name: "StatUlid",
                table: "UserGuildStats",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
