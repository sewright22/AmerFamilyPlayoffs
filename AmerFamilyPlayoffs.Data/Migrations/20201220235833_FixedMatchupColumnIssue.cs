using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AmerFamilyPlayoffs.Data.Migrations
{
    public partial class FixedMatchupColumnIssue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matchups_PlayoffRounds_PlayoffRoundId1",
                table: "Matchups");

            migrationBuilder.DropForeignKey(
                name: "FK_Matchups_PlayoffTeams_AwayTeamId",
                table: "Matchups");

            migrationBuilder.DropForeignKey(
                name: "FK_Matchups_PlayoffTeams_HomeTeamId",
                table: "Matchups");

            migrationBuilder.DropIndex(
                name: "IX_Matchups_PlayoffRoundId1",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "PlayoffRoundId1",
                table: "Matchups");

            migrationBuilder.AlterColumn<int>(
                name: "HomeTeamId",
                table: "Matchups",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AwayTeamId",
                table: "Matchups",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "MatchupPrediction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BracketPredictionId = table.Column<int>(type: "int", nullable: false),
                    MatchupId = table.Column<int>(type: "int", nullable: false),
                    WinningTeamId = table.Column<int>(type: "int", nullable: true),
                    PredictedWinnerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchupPrediction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchupPrediction_BracketPredictions_BracketPredictionId",
                        column: x => x.BracketPredictionId,
                        principalTable: "BracketPredictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchupPrediction_Matchups_MatchupId",
                        column: x => x.MatchupId,
                        principalTable: "Matchups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchupPrediction_PlayoffTeams_PredictedWinnerId",
                        column: x => x.PredictedWinnerId,
                        principalTable: "PlayoffTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchupPrediction_BracketPredictionId",
                table: "MatchupPrediction",
                column: "BracketPredictionId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchupPrediction_MatchupId",
                table: "MatchupPrediction",
                column: "MatchupId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchupPrediction_PredictedWinnerId",
                table: "MatchupPrediction",
                column: "PredictedWinnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matchups_PlayoffTeams_AwayTeamId",
                table: "Matchups",
                column: "AwayTeamId",
                principalTable: "PlayoffTeams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matchups_PlayoffTeams_HomeTeamId",
                table: "Matchups",
                column: "HomeTeamId",
                principalTable: "PlayoffTeams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matchups_PlayoffTeams_AwayTeamId",
                table: "Matchups");

            migrationBuilder.DropForeignKey(
                name: "FK_Matchups_PlayoffTeams_HomeTeamId",
                table: "Matchups");

            migrationBuilder.DropTable(
                name: "MatchupPrediction");

            migrationBuilder.AlterColumn<int>(
                name: "HomeTeamId",
                table: "Matchups",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AwayTeamId",
                table: "Matchups",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayoffRoundId1",
                table: "Matchups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_PlayoffRoundId1",
                table: "Matchups",
                column: "PlayoffRoundId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Matchups_PlayoffRounds_PlayoffRoundId1",
                table: "Matchups",
                column: "PlayoffRoundId1",
                principalTable: "PlayoffRounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matchups_PlayoffTeams_AwayTeamId",
                table: "Matchups",
                column: "AwayTeamId",
                principalTable: "PlayoffTeams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matchups_PlayoffTeams_HomeTeamId",
                table: "Matchups",
                column: "HomeTeamId",
                principalTable: "PlayoffTeams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
