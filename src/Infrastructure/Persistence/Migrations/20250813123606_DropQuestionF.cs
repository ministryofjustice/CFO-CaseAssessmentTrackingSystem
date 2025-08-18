using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DropQuestionF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemplatesAlignWithREG",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TemplatesAlignWithREG",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
