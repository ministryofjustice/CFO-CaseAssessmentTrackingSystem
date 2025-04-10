using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReassessmentPayments_AdditionalColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AssessmentCreated",
                schema: "Mi",
                table: "ReassessmentPayment",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                schema: "Mi",
                table: "ReassessmentPayment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LocationType",
                schema: "Mi",
                table: "ReassessmentPayment",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssessmentCreated",
                schema: "Mi",
                table: "ReassessmentPayment");

            migrationBuilder.DropColumn(
                name: "LocationId",
                schema: "Mi",
                table: "ReassessmentPayment");

            migrationBuilder.DropColumn(
                name: "LocationType",
                schema: "Mi",
                table: "ReassessmentPayment");
        }
    }
}
