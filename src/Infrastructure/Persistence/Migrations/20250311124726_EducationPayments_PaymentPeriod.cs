using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EducationPayments_PaymentPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_ActivityPayment_ParticipantId",
                schema: "Mi",
                table: "EducationPayment");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivityInput",
                schema: "Mi",
                table: "EducationPayment",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CommencedDate",
                schema: "Mi",
                table: "EducationPayment",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentPeriod",
                schema: "Mi",
                table: "EducationPayment",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "ix_ActivityPayment_ParticipantId",
                schema: "Mi",
                table: "EducationPayment",
                columns: new[] { "ParticipantId", "ContractId", "CourseLevel", "CourseTitle", "EligibleForPayment" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_ActivityPayment_ParticipantId",
                schema: "Mi",
                table: "EducationPayment");

            migrationBuilder.DropColumn(
                name: "ActivityInput",
                schema: "Mi",
                table: "EducationPayment");

            migrationBuilder.DropColumn(
                name: "CommencedDate",
                schema: "Mi",
                table: "EducationPayment");

            migrationBuilder.DropColumn(
                name: "PaymentPeriod",
                schema: "Mi",
                table: "EducationPayment");

            migrationBuilder.CreateIndex(
                name: "ix_ActivityPayment_ParticipantId",
                schema: "Mi",
                table: "EducationPayment",
                columns: new[] { "ParticipantId", "ContractId", "ActivityApproved" });
        }
    }
}
