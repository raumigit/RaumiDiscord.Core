using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaumiDiscord.Core.Server.Migrations
{
    /// <inheritdoc />
    public partial class DiscordComponentModel : Migration
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
                    ChannnelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MessegeId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    DeltaRaumiComponentType = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.CustomId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Components");
        }
    }
}
