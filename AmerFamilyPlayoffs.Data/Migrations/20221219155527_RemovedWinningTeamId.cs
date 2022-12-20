using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmerFamilyPlayoffs.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedWinningTeamId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WinningTeamId",
                table: "MatchupPrediction");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WinningTeamId",
                table: "MatchupPrediction",
                type: "int",
                nullable: true);
        }
    }
}
