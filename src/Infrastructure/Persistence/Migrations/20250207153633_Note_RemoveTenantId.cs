using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Note_RemoveTenantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Enrolment",
                table: "Qa2QueueNote");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Activities",
                table: "Qa2QueueNote");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Enrolment",
                table: "Qa1QueueNote");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Activities",
                table: "Qa1QueueNote");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Enrolment",
                table: "PqaQueueNote");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Activities",
                table: "PqaQueueNote");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Participant",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Identity",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Enrolment",
                table: "EscalationNote");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Activities",
                table: "EscalationNote");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Enrolment",
                table: "Qa2QueueNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Activities",
                table: "Qa2QueueNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Enrolment",
                table: "Qa1QueueNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Activities",
                table: "Qa1QueueNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Enrolment",
                table: "PqaQueueNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Activities",
                table: "PqaQueueNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Participant",
                table: "Note",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Identity",
                table: "Note",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Enrolment",
                table: "EscalationNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Activities",
                table: "EscalationNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
