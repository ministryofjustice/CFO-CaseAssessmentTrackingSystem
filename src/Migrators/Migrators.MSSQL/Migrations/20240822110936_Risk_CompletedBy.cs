using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class Risk_CompletedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Completed",
                schema: "Participant",
                table: "Risk",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompletedBy",
                schema: "Participant",
                table: "Risk",
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
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "CompletedBy",
                schema: "Participant",
                table: "Risk");
        }
    }
}
