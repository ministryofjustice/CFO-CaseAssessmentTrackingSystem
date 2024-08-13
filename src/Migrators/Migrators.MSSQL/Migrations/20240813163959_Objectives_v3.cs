using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class Objectives_v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CancelledReason",
                schema: "Participant",
                table: "ObjectiveTask",
                newName: "Justification");

            migrationBuilder.RenameColumn(
                name: "CancelledBy",
                schema: "Participant",
                table: "ObjectiveTask",
                newName: "ClosedBy");

            migrationBuilder.RenameColumn(
                name: "Cancelled",
                schema: "Participant",
                table: "ObjectiveTask",
                newName: "Closed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Justification",
                schema: "Participant",
                table: "ObjectiveTask",
                newName: "CancelledReason");

            migrationBuilder.RenameColumn(
                name: "ClosedBy",
                schema: "Participant",
                table: "ObjectiveTask",
                newName: "CancelledBy");

            migrationBuilder.RenameColumn(
                name: "Closed",
                schema: "Participant",
                table: "ObjectiveTask",
                newName: "Cancelled");
        }
    }
}
