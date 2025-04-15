using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Assessment_LocationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                schema: "Participant",
                table: "Assessment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Assessment_LocationId",
                schema: "Participant",
                table: "Assessment",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessment_Location_LocationId",
                schema: "Participant",
                table: "Assessment",
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
                name: "FK_Assessment_Location_LocationId",
                schema: "Participant",
                table: "Assessment");

            migrationBuilder.DropIndex(
                name: "IX_Assessment_LocationId",
                schema: "Participant",
                table: "Assessment");

            migrationBuilder.DropColumn(
                name: "LocationId",
                schema: "Participant",
                table: "Assessment");
        }
    }
}
