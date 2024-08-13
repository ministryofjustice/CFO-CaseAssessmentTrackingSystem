using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class Objectives_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Due",
                schema: "Participant",
                table: "ObjectiveTask",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Cancelled",
                schema: "Participant",
                table: "ObjectiveTask",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledBy",
                schema: "Participant",
                table: "ObjectiveTask",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledReason",
                schema: "Participant",
                table: "ObjectiveTask",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cancelled",
                schema: "Participant",
                table: "ObjectiveTask");

            migrationBuilder.DropColumn(
                name: "CancelledBy",
                schema: "Participant",
                table: "ObjectiveTask");

            migrationBuilder.DropColumn(
                name: "CancelledReason",
                schema: "Participant",
                table: "ObjectiveTask");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Due",
                schema: "Participant",
                table: "ObjectiveTask",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
