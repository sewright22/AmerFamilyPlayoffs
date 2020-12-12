using Microsoft.EntityFrameworkCore.Migrations;

namespace AmerFamilyPlayoffs.Data.Migrations
{
    public partial class AddPlayoffFieldToSeason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Playoffs_SeasonId",
                table: "Playoffs");

            migrationBuilder.CreateIndex(
                name: "IX_Playoffs_SeasonId",
                table: "Playoffs",
                column: "SeasonId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Playoffs_SeasonId",
                table: "Playoffs");

            migrationBuilder.CreateIndex(
                name: "IX_Playoffs_SeasonId",
                table: "Playoffs",
                column: "SeasonId");
        }
    }
}
