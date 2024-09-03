using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class Participant_Mappa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MappaCategory",
                schema: "Participant",
                table: "Participant",
                type: "int",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddColumn<int>(
                name: "MappaLevel",
                schema: "Participant",
                table: "Participant",
                type: "int",
                nullable: false,
                defaultValue: -1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MappaCategory",
                schema: "Participant",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "MappaLevel",
                schema: "Participant",
                table: "Participant");
        }
    }
}
