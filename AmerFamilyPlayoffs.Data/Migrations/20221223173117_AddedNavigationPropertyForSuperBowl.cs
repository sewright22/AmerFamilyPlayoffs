using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmerFamilyPlayoffs.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedNavigationPropertyForSuperBowl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MatchupPrediction_PlayoffRoundId",
                table: "MatchupPrediction",
                column: "PlayoffRoundId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchupPrediction_PlayoffRounds_PlayoffRoundId",
                table: "MatchupPrediction",
                column: "PlayoffRoundId",
                principalTable: "PlayoffRounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchupPrediction_PlayoffRounds_PlayoffRoundId",
                table: "MatchupPrediction");

            migrationBuilder.DropIndex(
                name: "IX_MatchupPrediction_PlayoffRoundId",
                table: "MatchupPrediction");
        }
    }
}
