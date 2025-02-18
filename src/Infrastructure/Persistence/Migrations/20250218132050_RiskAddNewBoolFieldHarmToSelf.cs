using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RiskAddNewBoolFieldHarmToSelf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RiskToSelfInCommunityNew",
                schema: "Participant",
                table: "Risk",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RiskToSelfInCustodyNew",
                schema: "Participant",
                table: "Risk",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RiskToSelfInCommunityNew",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "RiskToSelfInCustodyNew",
                schema: "Participant",
                table: "Risk");
        }
    }
}
