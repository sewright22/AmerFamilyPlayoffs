using Microsoft.EntityFrameworkCore.Migrations;

namespace AmerFamilyPlayoffs.Data.Migrations
{
    public partial class MovedSomeForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayoffTeams_Teams_TeamId",
                table: "PlayoffTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonTeams_Conferences_ConferenceId",
                table: "SeasonTeams");

            migrationBuilder.DropIndex(
                name: "IX_SeasonTeams_ConferenceId",
                table: "SeasonTeams");

            migrationBuilder.DropIndex(
                name: "IX_ConferenceTeams_SeasonTeamId",
                table: "ConferenceTeams");

            migrationBuilder.DropColumn(
                name: "ConferenceId",
                table: "SeasonTeams");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "PlayoffTeams",
                newName: "SeasonTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_PlayoffTeams_TeamId",
                table: "PlayoffTeams",
                newName: "IX_PlayoffTeams_SeasonTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ConferenceTeams_SeasonTeamId",
                table: "ConferenceTeams",
                column: "SeasonTeamId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayoffTeams_SeasonTeams_SeasonTeamId",
                table: "PlayoffTeams",
                column: "SeasonTeamId",
                principalTable: "SeasonTeams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayoffTeams_SeasonTeams_SeasonTeamId",
                table: "PlayoffTeams");

            migrationBuilder.DropIndex(
                name: "IX_ConferenceTeams_SeasonTeamId",
                table: "ConferenceTeams");

            migrationBuilder.RenameColumn(
                name: "SeasonTeamId",
                table: "PlayoffTeams",
                newName: "TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_PlayoffTeams_SeasonTeamId",
                table: "PlayoffTeams",
                newName: "IX_PlayoffTeams_TeamId");

            migrationBuilder.AddColumn<int>(
                name: "ConferenceId",
                table: "SeasonTeams",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeasonTeams_ConferenceId",
                table: "SeasonTeams",
                column: "ConferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_ConferenceTeams_SeasonTeamId",
                table: "ConferenceTeams",
                column: "SeasonTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayoffTeams_Teams_TeamId",
                table: "PlayoffTeams",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonTeams_Conferences_ConferenceId",
                table: "SeasonTeams",
                column: "ConferenceId",
                principalTable: "Conferences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
