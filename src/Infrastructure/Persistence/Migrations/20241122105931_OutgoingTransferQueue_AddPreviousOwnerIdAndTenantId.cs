using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OutgoingTransferQueue_AddPreviousOwnerIdAndTenantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreviousOwnerId",
                schema: "Participant",
                table: "OutgoingTransferQueue",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousTenantId",
                schema: "Participant",
                table: "OutgoingTransferQueue",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingTransferQueue_PreviousOwnerId",
                schema: "Participant",
                table: "OutgoingTransferQueue",
                column: "PreviousOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingTransferQueue_PreviousTenantId",
                schema: "Participant",
                table: "OutgoingTransferQueue",
                column: "PreviousTenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutgoingTransferQueue_Tenant_PreviousTenantId",
                schema: "Participant",
                table: "OutgoingTransferQueue",
                column: "PreviousTenantId",
                principalSchema: "Configuration",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutgoingTransferQueue_User_PreviousOwnerId",
                schema: "Participant",
                table: "OutgoingTransferQueue",
                column: "PreviousOwnerId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutgoingTransferQueue_Tenant_PreviousTenantId",
                schema: "Participant",
                table: "OutgoingTransferQueue");

            migrationBuilder.DropForeignKey(
                name: "FK_OutgoingTransferQueue_User_PreviousOwnerId",
                schema: "Participant",
                table: "OutgoingTransferQueue");

            migrationBuilder.DropIndex(
                name: "IX_OutgoingTransferQueue_PreviousOwnerId",
                schema: "Participant",
                table: "OutgoingTransferQueue");

            migrationBuilder.DropIndex(
                name: "IX_OutgoingTransferQueue_PreviousTenantId",
                schema: "Participant",
                table: "OutgoingTransferQueue");

            migrationBuilder.DropColumn(
                name: "PreviousOwnerId",
                schema: "Participant",
                table: "OutgoingTransferQueue");

            migrationBuilder.DropColumn(
                name: "PreviousTenantId",
                schema: "Participant",
                table: "OutgoingTransferQueue");
        }
    }
}
