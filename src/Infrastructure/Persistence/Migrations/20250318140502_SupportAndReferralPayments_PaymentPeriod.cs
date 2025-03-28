using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SupportAndReferralPayments_PaymentPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActivityInput",
                schema: "Mi",
                table: "SupportAndReferralPayment",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentPeriod",
                schema: "Mi",
                table: "SupportAndReferralPayment",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityInput",
                schema: "Mi",
                table: "SupportAndReferralPayment");

            migrationBuilder.DropColumn(
                name: "PaymentPeriod",
                schema: "Mi",
                table: "SupportAndReferralPayment");
        }
    }
}
