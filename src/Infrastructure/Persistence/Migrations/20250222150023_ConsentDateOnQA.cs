using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConsentDateOnQA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ConsentDate",
                schema: "Enrolment",
                table: "Qa2Queue",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ConsentDate",
                schema: "Enrolment",
                table: "Qa1Queue",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ConsentDate",
                schema: "Enrolment",
                table: "PqaQueue",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ConsentDate",
                schema: "Enrolment",
                table: "EscalationQueue",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsentDate",
                schema: "Enrolment",
                table: "Qa2Queue");

            migrationBuilder.DropColumn(
                name: "ConsentDate",
                schema: "Enrolment",
                table: "Qa1Queue");

            migrationBuilder.DropColumn(
                name: "ConsentDate",
                schema: "Enrolment",
                table: "PqaQueue");

            migrationBuilder.DropColumn(
                name: "ConsentDate",
                schema: "Enrolment",
                table: "EscalationQueue");
        }
    }
}
