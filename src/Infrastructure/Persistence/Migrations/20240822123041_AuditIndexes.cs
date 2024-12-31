using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AuditIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_IdentityAudit_UserName_DateTime",
                schema: "Audit",
                table: "IdentityAuditTrail",
                columns: new[] { "UserName", "DateTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_IdentityAudit_UserName_DateTime",
                schema: "Audit",
                table: "IdentityAuditTrail");
        }
    }
}
