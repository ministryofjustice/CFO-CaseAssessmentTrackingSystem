using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Participant_LicenseDeactivation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "DeactivatedInFeed",
                schema: "Participant",
                table: "Participant",
                type: "date",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE Participant.Participant
                SET DeactivatedInFeed = GETUTCDATE()
                WHERE ActiveInFeed = 0");

            migrationBuilder.DropColumn(
                name: "ActiveInFeed",
                schema: "Participant",
                table: "Participant");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ActiveInFeed",
                schema: "Participant",
                table: "Participant",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"
                UPDATE Participant.Participant
                SET ActiveInFeed = CASE WHEN DeactivatedInFeed IS NULL THEN 1 ELSE 0 END");

            migrationBuilder.DropColumn(
                name: "DeactivatedInFeed",
                schema: "Participant",
                table: "Participant");
        }
    }
}
