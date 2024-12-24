using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations.ManagementInformationDb
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Activities");

            migrationBuilder.EnsureSchema(
                name: "Attachments");

            migrationBuilder.CreateTable(
                name: "ActivityPayment",
                schema: "Activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActivityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActivityApproved = table.Column<DateTime>(type: "date", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    ContractId = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    LocationType = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EligibleForPayment = table.Column<bool>(type: "bit", nullable: false),
                    IneligibilityReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityPayment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DateDimension",
                columns: table => new
                {
                    TheDate = table.Column<DateTime>(type: "date", nullable: false),
                    TheDay = table.Column<int>(type: "int", nullable: false),
                    TheDaySuffix = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    TheDayName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TheDayOfWeek = table.Column<int>(type: "int", nullable: false),
                    TheDayOfWeekInMonth = table.Column<int>(type: "int", nullable: false),
                    TheDayOfYear = table.Column<int>(type: "int", nullable: false),
                    IsWeekend = table.Column<bool>(type: "bit", nullable: false),
                    TheWeek = table.Column<int>(type: "int", nullable: false),
                    TheISOweek = table.Column<int>(type: "int", nullable: false),
                    TheFirstOfWeek = table.Column<DateTime>(type: "date", nullable: false),
                    TheLastOfWeek = table.Column<DateTime>(type: "date", nullable: false),
                    TheWeekOfMonth = table.Column<int>(type: "int", nullable: false),
                    TheMonth = table.Column<int>(type: "int", nullable: false),
                    TheMonthName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TheFirstOfMonth = table.Column<DateTime>(type: "date", nullable: false),
                    TheLastOfMonth = table.Column<DateTime>(type: "date", nullable: false),
                    TheFirstOfNextMonth = table.Column<DateTime>(type: "date", nullable: false),
                    TheLastOfNextMonth = table.Column<DateTime>(type: "date", nullable: false),
                    TheQuarter = table.Column<int>(type: "int", nullable: false),
                    TheFirstOfQuarter = table.Column<DateTime>(type: "date", nullable: false),
                    TheLastOfQuarter = table.Column<DateTime>(type: "date", nullable: false),
                    TheYear = table.Column<int>(type: "int", nullable: false),
                    TheISOYear = table.Column<int>(type: "int", nullable: false),
                    TheFirstOfYear = table.Column<DateTime>(type: "date", nullable: false),
                    TheLastOfYear = table.Column<DateTime>(type: "date", nullable: false),
                    IsLeapYear = table.Column<bool>(type: "bit", nullable: false),
                    Has53Weeks = table.Column<bool>(type: "bit", nullable: false),
                    Has53ISOWeeks = table.Column<bool>(type: "bit", nullable: false),
                    MMYYYY = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Style101 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Style103 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Style112 = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Style120 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateDimension", x => x.TheDate);
                });

            migrationBuilder.CreateTable(
                name: "EnrolmentPayment",
                schema: "Attachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    SupportWorker = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ContractId = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    ConsentAdded = table.Column<DateTime>(type: "date", nullable: false),
                    ConsentSigned = table.Column<DateTime>(type: "date", nullable: false),
                    SubmissionToPqa = table.Column<DateTime>(type: "date", nullable: false),
                    SubmissionToAuthority = table.Column<DateTime>(type: "date", nullable: false),
                    SubmissionsToAuthority = table.Column<int>(type: "int", nullable: false),
                    Approved = table.Column<DateTime>(type: "date", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    LocationType = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false, defaultValue: ""),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReferralRoute = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EligibleForPayment = table.Column<bool>(type: "bit", nullable: false),
                    IneligibilityReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrolmentPayment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InductionPayment",
                schema: "Attachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    SupportWorker = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ContractId = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Induction = table.Column<DateTime>(type: "date", nullable: false),
                    Approved = table.Column<DateTime>(type: "date", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    LocationType = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false, defaultValue: ""),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EligibleForPayment = table.Column<bool>(type: "bit", nullable: false),
                    IneligibilityReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InductionPayment", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ActivityPayment_ParticipantId",
                schema: "Activities",
                table: "ActivityPayment",
                columns: new[] { "ParticipantId", "ContractId", "ActivityCategory", "ActivityType", "ActivityApproved" });

            migrationBuilder.CreateIndex(
                name: "IX_EnrolmentPayment_ParticipantId",
                schema: "Attachments",
                table: "EnrolmentPayment",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "ix_InductionPayment_ParticipantId",
                schema: "Attachments",
                table: "InductionPayment",
                columns: new[] { "ParticipantId", "ContractId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityPayment",
                schema: "Activities");

            migrationBuilder.DropTable(
                name: "DateDimension");

            migrationBuilder.DropTable(
                name: "EnrolmentPayment",
                schema: "Attachments");

            migrationBuilder.DropTable(
                name: "InductionPayment",
                schema: "Attachments");
        }
    }
}
