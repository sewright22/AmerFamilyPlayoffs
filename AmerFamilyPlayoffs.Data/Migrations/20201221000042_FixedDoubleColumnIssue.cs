using Microsoft.EntityFrameworkCore.Migrations;

namespace AmerFamilyPlayoffs.Data.Migrations
{
    public partial class FixedDoubleColumnIssue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matchups_PlayoffTeams_WinnerId",
                table: "Matchups");

            migrationBuilder.DropIndex(
                name: "IX_Matchups_WinnerId",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "WinnerId",
                table: "Matchups");

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_WinningTeamId",
                table: "Matchups",
                column: "WinningTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matchups_PlayoffTeams_WinningTeamId",
                table: "Matchups",
                column: "WinningTeamId",
                principalTable: "PlayoffTeams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matchups_PlayoffTeams_WinningTeamId",
                table: "Matchups");

            migrationBuilder.DropIndex(
                name: "IX_Matchups_WinningTeamId",
                table: "Matchups");

            migrationBuilder.AddColumn<int>(
                name: "WinnerId",
                table: "Matchups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_WinnerId",
                table: "Matchups",
                column: "WinnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matchups_PlayoffTeams_WinnerId",
                table: "Matchups",
                column: "WinnerId",
                principalTable: "PlayoffTeams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
