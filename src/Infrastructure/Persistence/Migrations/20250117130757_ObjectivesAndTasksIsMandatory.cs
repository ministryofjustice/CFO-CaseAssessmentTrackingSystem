using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ObjectivesAndTasksIsMandatory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMandatory",
                schema: "Participant",
                table: "ObjectiveTask",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMandatory",
                schema: "Participant",
                table: "Objective",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMandatory",
                schema: "Participant",
                table: "ObjectiveTask");

            migrationBuilder.DropColumn(
                name: "IsMandatory",
                schema: "Participant",
                table: "Objective");
        }
    }
}
