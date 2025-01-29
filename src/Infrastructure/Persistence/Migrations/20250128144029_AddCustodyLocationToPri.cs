using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCustodyLocationToPri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustodyLocationId",
                schema: "PRI",
                table: "PRI",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PRI_CustodyLocationId",
                schema: "PRI",
                table: "PRI",
                column: "CustodyLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_PRI_Location_CustodyLocationId",
                schema: "PRI",
                table: "PRI",
                column: "CustodyLocationId",
                principalSchema: "Configuration",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PRI_Location_CustodyLocationId",
                schema: "PRI",
                table: "PRI");

            migrationBuilder.DropIndex(
                name: "IX_PRI_CustodyLocationId",
                schema: "PRI",
                table: "PRI");

            migrationBuilder.DropColumn(
                name: "CustodyLocationId",
                schema: "PRI",
                table: "PRI");
        }
    }
}
