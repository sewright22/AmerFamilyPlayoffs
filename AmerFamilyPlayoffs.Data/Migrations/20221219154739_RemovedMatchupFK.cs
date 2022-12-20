using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmerFamilyPlayoffs.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedMatchupFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchupPrediction_Matchups_MatchupId",
                table: "MatchupPrediction");

            migrationBuilder.DropIndex(
                name: "IX_MatchupPrediction_MatchupId",
                table: "MatchupPrediction");

            migrationBuilder.DropColumn(
                name: "MatchupId",
                table: "MatchupPrediction");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MatchupId",
                table: "MatchupPrediction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MatchupPrediction_MatchupId",
                table: "MatchupPrediction",
                column: "MatchupId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchupPrediction_Matchups_MatchupId",
                table: "MatchupPrediction",
                column: "MatchupId",
                principalTable: "Matchups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
