using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRiskLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                schema: "Participant",
                table: "Risk",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Risk_LocationId",
                schema: "Participant",
                table: "Risk",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Risk_Location_LocationId",
                schema: "Participant",
                table: "Risk",
                column: "LocationId",
                principalSchema: "Configuration",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Risk_Location_LocationId",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropIndex(
                name: "IX_Risk_LocationId",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "LocationId",
                schema: "Participant",
                table: "Risk");
        }
    }
}
