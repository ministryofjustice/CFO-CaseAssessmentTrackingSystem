using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ParticipantOwnershipHistoryTenantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Participant",
                table: "OwnershipHistory",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OwnershipHistory_TenantId",
                schema: "Participant",
                table: "OwnershipHistory",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_OwnershipHistory_Tenant_TenantId",
                schema: "Participant",
                table: "OwnershipHistory",
                column: "TenantId",
                principalSchema: "Configuration",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnershipHistory_Tenant_TenantId",
                schema: "Participant",
                table: "OwnershipHistory");

            migrationBuilder.DropIndex(
                name: "IX_OwnershipHistory_TenantId",
                schema: "Participant",
                table: "OwnershipHistory");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Participant",
                table: "OwnershipHistory");
        }
    }
}
