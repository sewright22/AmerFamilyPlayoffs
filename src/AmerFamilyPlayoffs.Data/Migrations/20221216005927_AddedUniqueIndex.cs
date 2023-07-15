using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmerFamilyPlayoffs.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Seasons_Year",
                table: "Seasons",
                column: "Year",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Seasons_Year",
                table: "Seasons");
        }
    }
}
