using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserGeneratedDocuments_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentAuditTrail_GeneratedDocument_DocumentId",
                schema: "Audit",
                table: "DocumentAuditTrail");

            migrationBuilder.AlterColumn<string>(
                name: "RequestType",
                schema: "Audit",
                table: "DocumentAuditTrail",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentAuditTrail_Document_DocumentId",
                schema: "Audit",
                table: "DocumentAuditTrail",
                column: "DocumentId",
                principalSchema: "Document",
                principalTable: "Document",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentAuditTrail_Document_DocumentId",
                schema: "Audit",
                table: "DocumentAuditTrail");

            migrationBuilder.AlterColumn<string>(
                name: "RequestType",
                schema: "Audit",
                table: "DocumentAuditTrail",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentAuditTrail_GeneratedDocument_DocumentId",
                schema: "Audit",
                table: "DocumentAuditTrail",
                column: "DocumentId",
                principalSchema: "Document",
                principalTable: "GeneratedDocument",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
