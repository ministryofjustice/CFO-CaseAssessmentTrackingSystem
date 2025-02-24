using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SupportWorkerId_Enrolment_Queues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupportWorkerId",
                schema: "Enrolment",
                table: "Qa2Queue",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SupportWorkerId",
                schema: "Enrolment",
                table: "Qa1Queue",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SupportWorkerId",
                schema: "Enrolment",
                table: "PqaQueue",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SupportWorkerId",
                schema: "Enrolment",
                table: "EscalationQueue",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupportWorkerId",
                schema: "Enrolment",
                table: "Qa2Queue");

            migrationBuilder.DropColumn(
                name: "SupportWorkerId",
                schema: "Enrolment",
                table: "Qa1Queue");

            migrationBuilder.DropColumn(
                name: "SupportWorkerId",
                schema: "Enrolment",
                table: "PqaQueue");

            migrationBuilder.DropColumn(
                name: "SupportWorkerId",
                schema: "Enrolment",
                table: "EscalationQueue");
        }
    }
}
