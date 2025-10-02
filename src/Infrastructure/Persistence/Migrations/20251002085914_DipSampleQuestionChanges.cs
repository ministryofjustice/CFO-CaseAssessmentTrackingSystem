using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DipSampleQuestionChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivitiesLinkToTasks",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.AddColumn<int>(
                name: "TtgObjectiveTasks",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PreReleasePractical",
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
                name: "PreReleasePractical",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropColumn(
                name: "TtgObjectiveTasks",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.AddColumn<int>(
               name: "ActivitiesLinkToTasks",
               schema: "Mi",
               table: "OutcomeQualityDipSampleParticipant",
               type: "int",
               nullable: false,
               defaultValue: 0);
        }
    }
}
