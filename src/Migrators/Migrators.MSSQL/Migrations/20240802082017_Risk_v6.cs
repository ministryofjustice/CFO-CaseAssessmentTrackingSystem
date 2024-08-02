using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class Risk_v6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReferredOn",
                schema: "Participant",
                table: "Risk",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferrerEmail",
                schema: "Participant",
                table: "Risk",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferrerName",
                schema: "Participant",
                table: "Risk",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferredOn",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "ReferrerEmail",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "ReferrerName",
                schema: "Participant",
                table: "Risk");
        }
    }
}
