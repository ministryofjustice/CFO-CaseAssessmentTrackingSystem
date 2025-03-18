using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InductionPaymentAddNewFieldsForPaymentPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CommencedDate",
                schema: "Mi",
                table: "InductionPayment",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "InductionInput",
                schema: "Mi",
                table: "InductionPayment",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentPeriod",
                schema: "Mi",
                table: "InductionPayment",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommencedDate",
                schema: "Mi",
                table: "InductionPayment");

            migrationBuilder.DropColumn(
                name: "InductionInput",
                schema: "Mi",
                table: "InductionPayment");

            migrationBuilder.DropColumn(
                name: "PaymentPeriod",
                schema: "Mi",
                table: "InductionPayment");
        }
    }
}
