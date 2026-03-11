using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProviderFeedbackEnrolmentAndActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoteCreatedBy",
                schema: "Mi",
                table: "ProviderFeedbackEnrolment");

            migrationBuilder.DropColumn(
                name: "NoteCreatedDate",
                schema: "Mi",
                table: "ProviderFeedbackEnrolment");

            migrationBuilder.DropColumn(
                name: "NoteCreatedBy",
                schema: "Mi",
                table: "ProviderFeedbackActivity");

            migrationBuilder.DropColumn(
                name: "NoteCreatedDate",
                schema: "Mi",
                table: "ProviderFeedbackActivity");

            migrationBuilder.AddColumn<string>(
                name: "ActivityId",
                schema: "Mi",
                table: "ProviderFeedbackActivity",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ActivityType",
                schema: "Mi",
                table: "ProviderFeedbackActivity",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityId",
                schema: "Mi",
                table: "ProviderFeedbackActivity");

            migrationBuilder.DropColumn(
                name: "ActivityType",
                schema: "Mi",
                table: "ProviderFeedbackActivity");

            migrationBuilder.AddColumn<string>(
                name: "NoteCreatedBy",
                schema: "Mi",
                table: "ProviderFeedbackEnrolment",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NoteCreatedDate",
                schema: "Mi",
                table: "ProviderFeedbackEnrolment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NoteCreatedBy",
                schema: "Mi",
                table: "ProviderFeedbackActivity",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NoteCreatedDate",
                schema: "Mi",
                table: "ProviderFeedbackActivity",
                type: "datetime2",
                nullable: true);
        }
    }
}
