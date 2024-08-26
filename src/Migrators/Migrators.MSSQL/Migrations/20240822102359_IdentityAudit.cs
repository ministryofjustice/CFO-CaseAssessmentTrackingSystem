using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class IdentityAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdentityAuditTrail",
                schema: "Audit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PerformedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityAuditTrail", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityAuditTrail",
                schema: "Audit");
        }
    }
}
