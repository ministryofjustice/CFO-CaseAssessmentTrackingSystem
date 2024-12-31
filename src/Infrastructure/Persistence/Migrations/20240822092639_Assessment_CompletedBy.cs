using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Assessment_CompletedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Completed",
                schema: "Participant",
                table: "Assessment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompletedBy",
                schema: "Participant",
                table: "Assessment",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                schema: "Participant",
                table: "Assessment");

            migrationBuilder.DropColumn(
                name: "CompletedBy",
                schema: "Participant",
                table: "Assessment");
        }
    }
}
