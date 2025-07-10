using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DipSamples : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DipSample",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DipSampleParticipant",
                schema: "Mi");

            migrationBuilder.DropTable(
                name: "DipSample",
                schema: "Mi");
        }
    }
}
