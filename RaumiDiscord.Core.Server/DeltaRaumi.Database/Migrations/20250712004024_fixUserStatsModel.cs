using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.Migrations
{
    /// <inheritdoc />
    public partial class fixUserStatsModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MentionedUsers");

            migrationBuilder.AddColumn<string>(
                name: "MentionedUserId",
                table: "UserGuildStats",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MentionedUserId",
                table: "UserGuildStats");

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
    }
}
