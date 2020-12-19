using Microsoft.EntityFrameworkCore.Migrations;

namespace AmerFamilyPlayoffs.Data.Migrations
{
    public partial class RemovedBracketLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BracketId",
                table: "BracketPredictions",
                newName: "PlayoffId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "BracketPredictions",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BracketPredictions_PlayoffId",
                table: "BracketPredictions",
                column: "PlayoffId");

            migrationBuilder.AddForeignKey(
                name: "FK_BracketPredictions_Playoffs_PlayoffId",
                table: "BracketPredictions",
                column: "PlayoffId",
                principalTable: "Playoffs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BracketPredictions_Playoffs_PlayoffId",
                table: "BracketPredictions");

            migrationBuilder.DropIndex(
                name: "IX_BracketPredictions_PlayoffId",
                table: "BracketPredictions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "BracketPredictions");

            migrationBuilder.RenameColumn(
                name: "PlayoffId",
                table: "BracketPredictions",
                newName: "BracketId");
        }
    }
}
