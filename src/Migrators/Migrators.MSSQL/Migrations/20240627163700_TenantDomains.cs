using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable


namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class TenantDomains : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantDomain",
                columns: table => new
                {
                    TenantId = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Domain = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantDomain", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_TenantDomain_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantDomain");
        }
    }
}

