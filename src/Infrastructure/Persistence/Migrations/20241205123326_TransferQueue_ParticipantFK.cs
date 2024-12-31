using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TransferQueue_ParticipantFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OutgoingTransferQueue_ParticipantId",
                schema: "Participant",
                table: "OutgoingTransferQueue",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingTransferQueue_ParticipantId",
                schema: "Participant",
                table: "IncomingTransferQueue",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingTransferQueue_Participant_ParticipantId",
                schema: "Participant",
                table: "IncomingTransferQueue",
                column: "ParticipantId",
                principalSchema: "Participant",
                principalTable: "Participant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutgoingTransferQueue_Participant_ParticipantId",
                schema: "Participant",
                table: "OutgoingTransferQueue",
                column: "ParticipantId",
                principalSchema: "Participant",
                principalTable: "Participant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomingTransferQueue_Participant_ParticipantId",
                schema: "Participant",
                table: "IncomingTransferQueue");

            migrationBuilder.DropForeignKey(
                name: "FK_OutgoingTransferQueue_Participant_ParticipantId",
                schema: "Participant",
                table: "OutgoingTransferQueue");

            migrationBuilder.DropIndex(
                name: "IX_OutgoingTransferQueue_ParticipantId",
                schema: "Participant",
                table: "OutgoingTransferQueue");

            migrationBuilder.DropIndex(
                name: "IX_IncomingTransferQueue_ParticipantId",
                schema: "Participant",
                table: "IncomingTransferQueue");
        }
    }
}
