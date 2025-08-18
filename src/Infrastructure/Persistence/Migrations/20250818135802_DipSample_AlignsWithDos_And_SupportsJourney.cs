using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DipSample_AlignsWithDos_And_SupportsJourney : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SupportsJourneyAndAlignsWithDoS",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                newName: "SupportsJourney");

            migrationBuilder.AddColumn<int>(
                name: "AlignsWithDoS",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlignsWithDoS",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.RenameColumn(
                name: "SupportsJourney",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                newName: "SupportsJourneyAndAlignsWithDoS");
        }
    }
}
