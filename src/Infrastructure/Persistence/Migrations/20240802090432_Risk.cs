using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Risk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Risk",
                schema: "Participant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityRecommendations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityRecommendationsReceived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActivityRestrictions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityRestrictionsReceived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdditionalInformation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeclarationSigned = table.Column<bool>(type: "bit", nullable: false),
                    LicenseConditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsRelevantToCustody = table.Column<bool>(type: "bit", nullable: true),
                    IsRelevantToCommunity = table.Column<bool>(type: "bit", nullable: true),
                    IsSubjectToSHPO = table.Column<int>(type: "int", nullable: true),
                    MappaCategory = table.Column<int>(type: "int", nullable: true),
                    MappaLevel = table.Column<int>(type: "int", nullable: true),
                    NSDCase = table.Column<int>(type: "int", nullable: true),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", nullable: false),
                    PSFRestrictions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PSFRestrictionsReceived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReferrerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferrerEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferredOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewReason = table.Column<int>(type: "int", nullable: false),
                    ReviewJustification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RiskToChildrenInCustody = table.Column<int>(type: "int", nullable: true),
                    RiskToPublicInCustody = table.Column<int>(type: "int", nullable: true),
                    RiskToKnownAdultInCustody = table.Column<int>(type: "int", nullable: true),
                    RiskToStaffInCustody = table.Column<int>(type: "int", nullable: true),
                    RiskToOtherPrisonersInCustody = table.Column<int>(type: "int", nullable: true),
                    RiskToSelfInCustody = table.Column<int>(type: "int", nullable: true),
                    RiskToChildrenInCommunity = table.Column<int>(type: "int", nullable: true),
                    RiskToPublicInCommunity = table.Column<int>(type: "int", nullable: true),
                    RiskToKnownAdultInCommunity = table.Column<int>(type: "int", nullable: true),
                    RiskToStaffInCommunity = table.Column<int>(type: "int", nullable: true),
                    RiskToOtherPrisonersInCommunity = table.Column<int>(type: "int", nullable: true),
                    RiskToSelfInCommunity = table.Column<int>(type: "int", nullable: true),
                    SpecificRisk = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Risk", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Risk_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Risk_ParticipantId",
                schema: "Participant",
                table: "Risk",
                column: "ParticipantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Risk",
                schema: "Participant");
        }
    }
}
