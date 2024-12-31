using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EmploymentPaymentfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmploymentPayments",
                table: "EmploymentPayments");

            migrationBuilder.RenameTable(
                name: "EmploymentPayments",
                newName: "EmploymentPayment",
                newSchema: "Mi");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                schema: "Mi",
                table: "EmploymentPayment",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ParticipantId",
                schema: "Mi",
                table: "EmploymentPayment",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LocationType",
                schema: "Mi",
                table: "EmploymentPayment",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "IneligibilityReason",
                schema: "Mi",
                table: "EmploymentPayment",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContractId",
                schema: "Mi",
                table: "EmploymentPayment",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActivityApproved",
                schema: "Mi",
                table: "EmploymentPayment",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmploymentPayment",
                schema: "Mi",
                table: "EmploymentPayment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "ix_ActivityPayment_ParticipantId",
                schema: "Mi",
                table: "EmploymentPayment",
                columns: new[] { "ParticipantId", "ContractId", "ActivityApproved" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmploymentPayment",
                schema: "Mi",
                table: "EmploymentPayment");

            migrationBuilder.DropIndex(
                name: "ix_ActivityPayment_ParticipantId",
                schema: "Mi",
                table: "EmploymentPayment");

            migrationBuilder.RenameTable(
                name: "EmploymentPayment",
                schema: "Mi",
                newName: "EmploymentPayments");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "EmploymentPayments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ParticipantId",
                table: "EmploymentPayments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(9)",
                oldMaxLength: 9);

            migrationBuilder.AlterColumn<string>(
                name: "LocationType",
                table: "EmploymentPayments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "IneligibilityReason",
                table: "EmploymentPayments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContractId",
                table: "EmploymentPayments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(12)",
                oldMaxLength: 12);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActivityApproved",
                table: "EmploymentPayments",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmploymentPayments",
                table: "EmploymentPayments",
                column: "Id");
        }
    }
}
