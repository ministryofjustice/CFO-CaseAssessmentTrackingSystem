using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PRI_MeetingAttendees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeetingAttendedInPerson",
                schema: "PRI",
                table: "PRI");

            migrationBuilder.RenameColumn(
                name: "MeetingNotAttendedInPersonJustification",
                schema: "PRI",
                table: "PRI",
                newName: "ReasonParticipantDidNotAttendInPerson");

            migrationBuilder.AddColumn<string>(
                name: "ReasonCommunityDidNotAttendInPerson",
                schema: "PRI",
                table: "PRI",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonCustodyDidNotAttendInPerson",
                schema: "PRI",
                table: "PRI",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReasonCommunityDidNotAttendInPerson",
                schema: "PRI",
                table: "PRI");

            migrationBuilder.DropColumn(
                name: "ReasonCustodyDidNotAttendInPerson",
                schema: "PRI",
                table: "PRI");

            migrationBuilder.RenameColumn(
                name: "ReasonParticipantDidNotAttendInPerson",
                schema: "PRI",
                table: "PRI",
                newName: "MeetingNotAttendedInPersonJustification");

            migrationBuilder.AddColumn<bool>(
                name: "MeetingAttendedInPerson",
                schema: "PRI",
                table: "PRI",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
