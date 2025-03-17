using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EmploymentPayments_PaymentPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_ActivityPayment_ParticipantId",
                schema: "Mi",
                table: "EmploymentPayment");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivityInput",
                schema: "Mi",
                table: "EmploymentPayment",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CommencedDate",
                schema: "Mi",
                table: "EmploymentPayment",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentPeriod",
                schema: "Mi",
                table: "EmploymentPayment",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "ix_ActivityPayment_ParticipantId",
                schema: "Mi",
                table: "EmploymentPayment",
                columns: new[] { "ParticipantId", "ContractId", "CommencedDate", "EligibleForPayment" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_ActivityPayment_ParticipantId",
                schema: "Mi",
                table: "EmploymentPayment");

            migrationBuilder.DropColumn(
                name: "ActivityInput",
                schema: "Mi",
                table: "EmploymentPayment");

            migrationBuilder.DropColumn(
                name: "CommencedDate",
                schema: "Mi",
                table: "EmploymentPayment");

            migrationBuilder.DropColumn(
                name: "PaymentPeriod",
                schema: "Mi",
                table: "EmploymentPayment");

            migrationBuilder.CreateIndex(
                name: "ix_ActivityPayment_ParticipantId",
                schema: "Mi",
                table: "EmploymentPayment",
                columns: new[] { "ParticipantId", "ContractId", "ActivityApproved" });
        }
    }
}
