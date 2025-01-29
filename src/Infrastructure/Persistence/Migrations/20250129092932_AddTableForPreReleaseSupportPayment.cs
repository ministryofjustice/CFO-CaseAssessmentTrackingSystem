using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTableForPreReleaseSupportPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupportAndReferralPayment",
                schema: "Mi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupportType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Approved = table.Column<DateTime>(type: "date", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    SupportWorker = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContractId = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    LocationType = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EligibleForPayment = table.Column<bool>(type: "bit", nullable: false),
                    IneligibilityReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportAndReferralPayment", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ActivityPayment_ParticipantId",
                schema: "Mi",
                table: "SupportAndReferralPayment",
                columns: new[] { "ParticipantId", "ContractId", "SupportType", "Approved" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportAndReferralPayment",
                schema: "Mi");
        }
    }
}
