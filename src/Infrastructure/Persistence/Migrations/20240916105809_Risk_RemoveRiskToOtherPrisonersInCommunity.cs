using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Risk_RemoveRiskToOtherPrisonersInCommunity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RiskToOtherPrisonersInCommunity",
                schema: "Participant",
                table: "Risk");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RiskToOtherPrisonersInCommunity",
                schema: "Participant",
                table: "Risk",
                type: "int",
                nullable: true);
        }
    }
}
