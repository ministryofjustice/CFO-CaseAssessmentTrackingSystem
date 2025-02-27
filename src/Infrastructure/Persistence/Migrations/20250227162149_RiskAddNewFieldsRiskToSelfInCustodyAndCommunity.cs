using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RiskAddNewFieldsRiskToSelfInCustodyAndCommunity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RiskToSelfInCommunityNew",
                schema: "Participant",
                table: "Risk",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RiskToSelfInCustodyNew",
                schema: "Participant",
                table: "Risk",
                type: "int",
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
