using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class participantEnrolmentHistory_AddVirtualParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_EnrolmentHistory_Participant_ParticipantId",
                schema: "Participant",
                table: "EnrolmentHistory",
                column: "ParticipantId",
                principalSchema: "Participant",
                principalTable: "Participant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnrolmentHistory_Participant_ParticipantId",
                schema: "Participant",
                table: "EnrolmentHistory");
        }
    }
}
