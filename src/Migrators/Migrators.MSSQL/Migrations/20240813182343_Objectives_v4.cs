using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class Objectives_v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Closed",
                schema: "Participant",
                table: "ObjectiveTask");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                schema: "Participant",
                table: "ObjectiveTask");

            migrationBuilder.AddColumn<int>(
                name: "CompletedStatus",
                schema: "Participant",
                table: "ObjectiveTask",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedStatus",
                schema: "Participant",
                table: "ObjectiveTask");

            migrationBuilder.AddColumn<DateTime>(
                name: "Closed",
                schema: "Participant",
                table: "ObjectiveTask",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                schema: "Participant",
                table: "ObjectiveTask",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
