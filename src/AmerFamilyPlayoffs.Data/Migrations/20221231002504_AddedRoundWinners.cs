using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmerFamilyPlayoffs.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedRoundWinners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoundWinners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayoffRoundId = table.Column<int>(type: "int", nullable: false),
                    PlayoffTeamId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoundWinners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoundWinners_PlayoffRounds_PlayoffRoundId",
                        column: x => x.PlayoffRoundId,
                        principalTable: "PlayoffRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoundWinners_PlayoffTeams_PlayoffTeamId",
                        column: x => x.PlayoffTeamId,
                        principalTable: "PlayoffTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_RoundWinners_PlayoffRoundId",
                table: "RoundWinners",
                column: "PlayoffRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_RoundWinners_PlayoffTeamId",
                table: "RoundWinners",
                column: "PlayoffTeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoundWinners");
        }
    }
}
