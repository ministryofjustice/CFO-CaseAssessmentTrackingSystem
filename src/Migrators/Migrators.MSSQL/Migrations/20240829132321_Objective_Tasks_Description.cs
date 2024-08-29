using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class Objective_Tasks_Description : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                schema: "Participant",
                table: "ObjectiveTask",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Title",
                schema: "Participant",
                table: "Objective",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "Participant",
                table: "ObjectiveTask",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "Participant",
                table: "Objective",
                newName: "Title");
        }
    }
}
