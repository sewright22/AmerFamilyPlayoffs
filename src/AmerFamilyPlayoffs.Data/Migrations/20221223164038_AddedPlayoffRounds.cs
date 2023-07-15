using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmerFamilyPlayoffs.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedPlayoffRounds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matchups_PlayoffRounds_PlayoffRoundId",
                table: "Matchups");

            migrationBuilder.DropIndex(
                name: "IX_Matchups_PlayoffRoundId",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "PlayoffRoundId",
                table: "Matchups");

            migrationBuilder.AddColumn<int>(
                name: "PlayoffRoundId",
                table: "MatchupPrediction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BracketPredictions",
                type: "varchar(95)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BracketPredictions_UserId",
                table: "BracketPredictions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BracketPredictions_AspNetUsers_UserId",
                table: "BracketPredictions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BracketPredictions_AspNetUsers_UserId",
                table: "BracketPredictions");

            migrationBuilder.DropIndex(
                name: "IX_BracketPredictions_UserId",
                table: "BracketPredictions");

            migrationBuilder.DropColumn(
                name: "PlayoffRoundId",
                table: "MatchupPrediction");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BracketPredictions");

            migrationBuilder.AddColumn<int>(
                name: "PlayoffRoundId",
                table: "Matchups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_PlayoffRoundId",
                table: "Matchups",
                column: "PlayoffRoundId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matchups_PlayoffRounds_PlayoffRoundId",
                table: "Matchups",
                column: "PlayoffRoundId",
                principalTable: "PlayoffRounds",
                principalColumn: "Id");
        }
    }
}
