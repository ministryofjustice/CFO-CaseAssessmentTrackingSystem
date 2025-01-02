using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class QaNoteExternal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsExternal",
                schema: "Enrolment",
                table: "Qa2QueueNote",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsExternal",
                schema: "Enrolment",
                table: "Qa1QueueNote",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsExternal",
                schema: "Enrolment",
                table: "EscalationNote",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExternal",
                schema: "Enrolment",
                table: "Qa2QueueNote");

            migrationBuilder.DropColumn(
                name: "IsExternal",
                schema: "Enrolment",
                table: "Qa1QueueNote");

            migrationBuilder.DropColumn(
                name: "IsExternal",
                schema: "Enrolment",
                table: "EscalationNote");
        }
    }
}
