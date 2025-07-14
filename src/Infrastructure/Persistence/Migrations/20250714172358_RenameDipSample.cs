using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameDipSample : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DipSampleParticipant",
                schema: "Mi");

            migrationBuilder.DropTable(
                name: "DipSample",
                schema: "Mi");

            migrationBuilder.CreateTable(
                name: "OutcomeQualityDipSample",
                schema: "Mi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractId = table.Column<string>(type: "nvarchar(12)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedBy = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    ScoreAvg = table.Column<double>(type: "float", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutcomeQualityDipSample", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutcomeQualityDipSample_Contract_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "Configuration",
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutcomeQualityDipSample_User_CompletedBy",
                        column: x => x.CompletedBy,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OutcomeQualityDipSampleParticipant",
                schema: "Mi",
                columns: table => new
                {
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", nullable: false),
                    DipSampleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    HasClearParticipantJourney = table.Column<bool>(type: "bit", nullable: true),
                    ShowsTaskProgression = table.Column<bool>(type: "bit", nullable: true),
                    ActivitiesLinkToTasks = table.Column<bool>(type: "bit", nullable: true),
                    TTGDemonstratesGoodPRIProcess = table.Column<bool>(type: "bit", nullable: true),
                    TemplatesAlignWithREG = table.Column<bool>(type: "bit", nullable: true),
                    SupportsJourneyAndAlignsWithDoS = table.Column<bool>(type: "bit", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsCompliant = table.Column<bool>(type: "bit", nullable: true),
                    ReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutcomeQualityDipSampleParticipant", x => new { x.DipSampleId, x.ParticipantId });
                    table.ForeignKey(
                        name: "FK_OutcomeQualityDipSampleParticipant_OutcomeQualityDipSample_DipSampleId",
                        column: x => x.DipSampleId,
                        principalSchema: "Mi",
                        principalTable: "OutcomeQualityDipSample",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutcomeQualityDipSampleParticipant_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutcomeQualityDipSampleParticipant_User_ReviewedBy",
                        column: x => x.ReviewedBy,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeQualityDipSample_CompletedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                column: "CompletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeQualityDipSample_ContractId",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeQualityDipSampleParticipant_IsCompliant",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                column: "IsCompliant");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeQualityDipSampleParticipant_ParticipantId",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeQualityDipSampleParticipant_ReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                column: "ReviewedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutcomeQualityDipSampleParticipant",
                schema: "Mi");

            migrationBuilder.DropTable(
                name: "OutcomeQualityDipSample",
                schema: "Mi");

            migrationBuilder.CreateTable(
                name: "DipSample",
                schema: "Mi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompletedBy = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractId = table.Column<string>(type: "nvarchar(12)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScoreAvg = table.Column<double>(type: "float", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DipSample", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DipSample_Contract_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "Configuration",
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DipSample_User_CompletedBy",
                        column: x => x.CompletedBy,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DipSampleParticipant",
                schema: "Mi",
                columns: table => new
                {
                    DipSampleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", nullable: false),
                    ActivitiesLinkToTasks = table.Column<bool>(type: "bit", nullable: true),
                    HasClearParticipantJourney = table.Column<bool>(type: "bit", nullable: true),
                    IsCompliant = table.Column<bool>(type: "bit", nullable: true),
                    LocationType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    ReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShowsTaskProgression = table.Column<bool>(type: "bit", nullable: true),
                    SupportsJourneyAndAlignsWithDoS = table.Column<bool>(type: "bit", nullable: true),
                    TTGDemonstratesGoodPRIProcess = table.Column<bool>(type: "bit", nullable: true),
                    TemplatesAlignWithREG = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DipSampleParticipant", x => new { x.DipSampleId, x.ParticipantId });
                    table.ForeignKey(
                        name: "FK_DipSampleParticipant_DipSample_DipSampleId",
                        column: x => x.DipSampleId,
                        principalSchema: "Mi",
                        principalTable: "DipSample",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DipSampleParticipant_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DipSampleParticipant_User_ReviewedBy",
                        column: x => x.ReviewedBy,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DipSample_CompletedBy",
                schema: "Mi",
                table: "DipSample",
                column: "CompletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DipSample_ContractId",
                schema: "Mi",
                table: "DipSample",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_DipSampleParticipant_IsCompliant",
                schema: "Mi",
                table: "DipSampleParticipant",
                column: "IsCompliant");

            migrationBuilder.CreateIndex(
                name: "IX_DipSampleParticipant_ParticipantId",
                schema: "Mi",
                table: "DipSampleParticipant",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_DipSampleParticipant_ReviewedBy",
                schema: "Mi",
                table: "DipSampleParticipant",
                column: "ReviewedBy");
        }
    }
}
