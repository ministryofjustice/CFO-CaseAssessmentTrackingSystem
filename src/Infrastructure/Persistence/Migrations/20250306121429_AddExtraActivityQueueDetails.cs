using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExtraActivityQueueDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OriginalPQASubmissionDate",
                schema: "Activities",
                table: "ActivityQa2Queue",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SupportWorkerId",
                schema: "Activities",
                table: "ActivityQa2Queue",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "OriginalPQASubmissionDate",
                schema: "Activities",
                table: "ActivityQa1Queue",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SupportWorkerId",
                schema: "Activities",
                table: "ActivityQa1Queue",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "OriginalPQASubmissionDate",
                schema: "Activities",
                table: "ActivityPqaQueue",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SupportWorkerId",
                schema: "Activities",
                table: "ActivityPqaQueue",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "OriginalPQASubmissionDate",
                schema: "Activities",
                table: "ActivityEscalationQueue",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SupportWorkerId",
                schema: "Activities",
                table: "ActivityEscalationQueue",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalPQASubmissionDate",
                schema: "Activities",
                table: "ActivityQa2Queue");

            migrationBuilder.DropColumn(
                name: "SupportWorkerId",
                schema: "Activities",
                table: "ActivityQa2Queue");

            migrationBuilder.DropColumn(
                name: "OriginalPQASubmissionDate",
                schema: "Activities",
                table: "ActivityQa1Queue");

            migrationBuilder.DropColumn(
                name: "SupportWorkerId",
                schema: "Activities",
                table: "ActivityQa1Queue");

            migrationBuilder.DropColumn(
                name: "OriginalPQASubmissionDate",
                schema: "Activities",
                table: "ActivityPqaQueue");

            migrationBuilder.DropColumn(
                name: "SupportWorkerId",
                schema: "Activities",
                table: "ActivityPqaQueue");

            migrationBuilder.DropColumn(
                name: "OriginalPQASubmissionDate",
                schema: "Activities",
                table: "ActivityEscalationQueue");

            migrationBuilder.DropColumn(
                name: "SupportWorkerId",
                schema: "Activities",
                table: "ActivityEscalationQueue");
        }
    }
}
