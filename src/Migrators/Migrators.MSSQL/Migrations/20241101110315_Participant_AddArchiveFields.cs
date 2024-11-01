using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class Participant_AddArchiveFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArchiveJustification",
                schema: "Participant",
                table: "Participant",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ArchiveReason",
                schema: "Participant",
                table: "Participant",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchiveJustification",
                schema: "Participant",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "ArchiveReason",
                schema: "Participant",
                table: "Participant");
        }
    }
}
