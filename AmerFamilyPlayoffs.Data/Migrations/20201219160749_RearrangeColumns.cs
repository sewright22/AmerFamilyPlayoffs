using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AmerFamilyPlayoffs.Data.Migrations
{
    public partial class RearrangeColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matchups_Rounds_RoundId",
                table: "Matchups");

            migrationBuilder.DropForeignKey(
                name: "FK_Matchups_Teams_AwayTeamId",
                table: "Matchups");

            migrationBuilder.DropForeignKey(
                name: "FK_Matchups_Teams_HomeTeamId",
                table: "Matchups");

            migrationBuilder.DropIndex(
                name: "IX_Matchups_RoundId",
                table: "Matchups");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "Matchups",
                newName: "WinningTeamId");

            migrationBuilder.AddColumn<int>(
                name: "PointValue",
                table: "PlayoffRounds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayoffRoundId1",
                table: "Matchups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WinnerId",
                table: "Matchups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BracketPredictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BracketId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BracketPredictions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brackets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayoffId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brackets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Brackets_Playoffs_PlayoffId",
                        column: x => x.PlayoffId,
                        principalTable: "Playoffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_PlayoffRoundId1",
                table: "Matchups",
                column: "PlayoffRoundId1");

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_WinnerId",
                table: "Matchups",
                column: "WinnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Brackets_PlayoffId",
                table: "Brackets",
                column: "PlayoffId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Matchups_PlayoffTeams_WinnerId",
                table: "Matchups",
                column: "WinnerId",
                principalTable: "PlayoffTeams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropForeignKey(
                name: "FK_Matchups_PlayoffTeams_WinnerId",
                table: "Matchups");

            migrationBuilder.DropTable(
                name: "BracketPredictions");

            migrationBuilder.DropTable(
                name: "Brackets");

            migrationBuilder.DropIndex(
                name: "IX_Matchups_PlayoffRoundId1",
                table: "Matchups");

            migrationBuilder.DropIndex(
                name: "IX_Matchups_WinnerId",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "PointValue",
                table: "PlayoffRounds");

            migrationBuilder.DropColumn(
                name: "PlayoffRoundId1",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "WinnerId",
                table: "Matchups");

            migrationBuilder.RenameColumn(
                name: "WinningTeamId",
                table: "Matchups",
                newName: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_RoundId",
                table: "Matchups",
                column: "RoundId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matchups_Rounds_RoundId",
                table: "Matchups",
                column: "RoundId",
                principalTable: "Rounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matchups_Teams_AwayTeamId",
                table: "Matchups",
                column: "AwayTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matchups_Teams_HomeTeamId",
                table: "Matchups",
                column: "HomeTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
