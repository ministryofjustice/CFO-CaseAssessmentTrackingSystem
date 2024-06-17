using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class EnrolmentLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnrolmentLocationId",
                table: "Participant",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnrolmentLocationJustification",
                table: "Participant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participant_EnrolmentLocationId",
                table: "Participant",
                column: "EnrolmentLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_EnrolmentLocation",
                table: "Participant",
                column: "EnrolmentLocationId",
                principalTable: "Location",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participant_EnrolmentLocation",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_Participant_EnrolmentLocationId",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "EnrolmentLocationId",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "EnrolmentLocationJustification",
                table: "Participant");
        }
    }
}
