using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.ManagementInformationMigrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Attachments");

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
                    ConsentAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsentSigned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmissionToPqa = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmissionToAuthority = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmissionsToAuthority = table.Column<int>(type: "int", nullable: false),
                    Approved = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReferralRoute = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EligibleForPayment = table.Column<bool>(type: "bit", nullable: false),
                    IneligibilityReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrolmentPayment", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnrolmentPayment_ParticipantId",
                schema: "Attachments",
                table: "EnrolmentPayment",
                column: "ParticipantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnrolmentPayment",
                schema: "Attachments");
        }
    }
}
