using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
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
                    ActivityRestrictions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalInformation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseConditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRelevantToCustody = table.Column<bool>(type: "bit", nullable: true),
                    IsRelevantToCommunity = table.Column<bool>(type: "bit", nullable: true),
                    IsSubjectToSHPO = table.Column<bool>(type: "bit", nullable: true),
                    MappaCategory = table.Column<int>(type: "int", nullable: true),
                    MappaLevel = table.Column<int>(type: "int", nullable: true),
                    NSDCase = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", nullable: false),
                    RiskToChildren = table.Column<int>(type: "int", nullable: true),
                    RiskToPublic = table.Column<int>(type: "int", nullable: true),
                    RiskToKnownAdult = table.Column<int>(type: "int", nullable: true),
                    RiskToStaff = table.Column<int>(type: "int", nullable: true),
                    RiskToOtherPrisoners = table.Column<int>(type: "int", nullable: true),
                    RiskToSelf = table.Column<int>(type: "int", nullable: true),
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
