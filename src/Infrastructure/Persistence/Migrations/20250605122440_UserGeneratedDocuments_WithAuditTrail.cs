using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserGeneratedDocuments_WithAuditTrail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneratedDocument",
                schema: "Document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedDocument_Document_Id",
                        column: x => x.Id,
                        principalSchema: "Document",
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentAuditTrail",
                schema: "Audit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    OccurredOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentAuditTrail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentAuditTrail_GeneratedDocument_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "Document",
                        principalTable: "GeneratedDocument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentAuditTrail_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAuditTrail_DocumentId",
                schema: "Audit",
                table: "DocumentAuditTrail",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAuditTrail_UserId",
                schema: "Audit",
                table: "DocumentAuditTrail",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentAuditTrail",
                schema: "Audit");

            migrationBuilder.DropTable(
                name: "GeneratedDocument",
                schema: "Document");
        }
    }
}
