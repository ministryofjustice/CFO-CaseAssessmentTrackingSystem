using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class Risk_Feedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "NSDCase",
                schema: "Participant",
                table: "Risk",
                type: "bit",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivityRecommendationsReceived",
                schema: "Participant",
                table: "Risk",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivityRestrictionsReceived",
                schema: "Participant",
                table: "Risk",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseEnd",
                schema: "Participant",
                table: "Risk",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PSFRestrictions",
                schema: "Participant",
                table: "Risk",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PSFRestrictionsReceived",
                schema: "Participant",
                table: "Risk",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityRecommendationsReceived",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "ActivityRestrictionsReceived",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "LicenseEnd",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "PSFRestrictions",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "PSFRestrictionsReceived",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.AlterColumn<string>(
                name: "NSDCase",
                schema: "Participant",
                table: "Risk",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }
    }
}
