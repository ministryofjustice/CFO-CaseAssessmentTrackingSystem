using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class Participant_RemoveMappa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MappaCategory",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "MappaLevel",
                schema: "Participant",
                table: "Risk");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MappaCategory",
                schema: "Participant",
                table: "Risk",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MappaLevel",
                schema: "Participant",
                table: "Risk",
                type: "int",
                nullable: true);
        }
    }
}
