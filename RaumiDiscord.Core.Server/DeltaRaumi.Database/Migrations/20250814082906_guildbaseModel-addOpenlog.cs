using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.Migrations
{
    /// <inheritdoc />
    public partial class guildbaseModeladdOpenlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OpenLogChannel",
                table: "GuildBases",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenLogChannel",
                table: "GuildBases");
        }
    }
}
