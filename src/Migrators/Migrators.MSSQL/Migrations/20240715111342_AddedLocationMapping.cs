using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddedLocationMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dms");

            migrationBuilder.CreateTable(
                name: "LocationMapping",
                schema: "dms",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CodeType = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DeliveryRegion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationMapping", x => new { x.Code, x.CodeType });
                    table.ForeignKey(
                        name: "FK_LocationMapping_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationMapping_LocationId",
                schema: "dms",
                table: "LocationMapping",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationMapping",
                schema: "dms");
        }
    }
}
