using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AmerFamilyPlayoffs.Data.Migrations
{
    public partial class AddedConferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConferenceId",
                table: "SeasonTeams",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Conferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConferenceTeams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ConferenceId = table.Column<int>(type: "int", nullable: false),
                    SeasonTeamId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConferenceTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConferenceTeams_Conferences_ConferenceId",
                        column: x => x.ConferenceId,
                        principalTable: "Conferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConferenceTeams_SeasonTeams_SeasonTeamId",
                        column: x => x.SeasonTeamId,
                        principalTable: "SeasonTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeasonTeams_ConferenceId",
                table: "SeasonTeams",
                column: "ConferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_ConferenceTeams_ConferenceId",
                table: "ConferenceTeams",
                column: "ConferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_ConferenceTeams_SeasonTeamId",
                table: "ConferenceTeams",
                column: "SeasonTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonTeams_Conferences_ConferenceId",
                table: "SeasonTeams",
                column: "ConferenceId",
                principalTable: "Conferences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonTeams_Conferences_ConferenceId",
                table: "SeasonTeams");

            migrationBuilder.DropTable(
                name: "ConferenceTeams");

            migrationBuilder.DropTable(
                name: "Conferences");

            migrationBuilder.DropIndex(
                name: "IX_SeasonTeams_ConferenceId",
                table: "SeasonTeams");

            migrationBuilder.DropColumn(
                name: "ConferenceId",
                table: "SeasonTeams");
        }
    }
}
