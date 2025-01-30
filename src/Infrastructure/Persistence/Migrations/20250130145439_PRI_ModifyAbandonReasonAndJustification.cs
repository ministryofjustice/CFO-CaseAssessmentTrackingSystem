using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PRI_ModifyAbandonReasonAndJustification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReasonAbandoned",
                schema: "PRI",
                table: "PRI",
                newName: "AbandonJustification");

            migrationBuilder.AddColumn<int>(
                name: "AbandonReason",
                schema: "PRI",
                table: "PRI",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedOn",
                schema: "PRI",
                table: "PRI",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbandonReason",
                schema: "PRI",
                table: "PRI");

            migrationBuilder.DropColumn(
                name: "CompletedOn",
                schema: "PRI",
                table: "PRI");

            migrationBuilder.RenameColumn(
                name: "AbandonJustification",
                schema: "PRI",
                table: "PRI",
                newName: "ReasonAbandoned");
        }
    }
}
