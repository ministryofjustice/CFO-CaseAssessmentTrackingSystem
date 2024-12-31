using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Activities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Payables");

            migrationBuilder.CreateTable(
                name: "Activities",
                schema: "Payables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Definition = table.Column<int>(type: "int", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", nullable: false),
                    TookPlaceAtLocationId = table.Column<int>(type: "int", nullable: false),
                    TookPlaceAtContractId = table.Column<string>(type: "nvarchar(12)", nullable: false),
                    ParticipantCurrentLocationId = table.Column<int>(type: "int", nullable: false),
                    ParticipantCurrentContractId = table.Column<string>(type: "nvarchar(12)", nullable: true),
                    ParticipantStatus = table.Column<int>(type: "int", nullable: false),
                    AdditionalInformation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Completed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_Contract_ParticipantCurrentContractId",
                        column: x => x.ParticipantCurrentContractId,
                        principalSchema: "Configuration",
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activities_Contract_TookPlaceAtContractId",
                        column: x => x.TookPlaceAtContractId,
                        principalSchema: "Configuration",
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activities_Location_ParticipantCurrentLocationId",
                        column: x => x.ParticipantCurrentLocationId,
                        principalSchema: "Configuration",
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activities_Location_TookPlaceAtLocationId",
                        column: x => x.TookPlaceAtLocationId,
                        principalSchema: "Configuration",
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activities_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activities_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "Configuration",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationTrainingActivities",
                schema: "Payables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CourseLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseCommencedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CourseCompletionStatus = table.Column<int>(type: "int", nullable: false),
                    Passed = table.Column<bool>(type: "bit", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationTrainingActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationTrainingActivities_Activities_Id",
                        column: x => x.Id,
                        principalSchema: "Payables",
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EducationTrainingActivities_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "Document",
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentActivities",
                schema: "Payables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmploymentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobTitleCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salary = table.Column<double>(type: "float", nullable: true),
                    EmploymentCommenced = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentActivities_Activities_Id",
                        column: x => x.Id,
                        principalSchema: "Payables",
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmploymentActivities_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "Document",
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ISWActivities",
                schema: "Payables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WraparoundSupportStartedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoursPerformedPre = table.Column<int>(type: "int", nullable: false),
                    HoursPerformedDuring = table.Column<int>(type: "int", nullable: false),
                    HoursPerformedPost = table.Column<int>(type: "int", nullable: false),
                    BaselineAchievedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ISWActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ISWActivities_Activities_Id",
                        column: x => x.Id,
                        principalSchema: "Payables",
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ISWActivities_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "Document",
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NonISWActivities",
                schema: "Payables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonISWActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NonISWActivities_Activities_Id",
                        column: x => x.Id,
                        principalSchema: "Payables",
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ParticipantCurrentContractId",
                schema: "Payables",
                table: "Activities",
                column: "ParticipantCurrentContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ParticipantCurrentLocationId",
                schema: "Payables",
                table: "Activities",
                column: "ParticipantCurrentLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ParticipantId",
                schema: "Payables",
                table: "Activities",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_TenantId",
                schema: "Payables",
                table: "Activities",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_TookPlaceAtContractId",
                schema: "Payables",
                table: "Activities",
                column: "TookPlaceAtContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_TookPlaceAtLocationId",
                schema: "Payables",
                table: "Activities",
                column: "TookPlaceAtLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationTrainingActivities_DocumentId",
                schema: "Payables",
                table: "EducationTrainingActivities",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentActivities_DocumentId",
                schema: "Payables",
                table: "EmploymentActivities",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ISWActivities_DocumentId",
                schema: "Payables",
                table: "ISWActivities",
                column: "DocumentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducationTrainingActivities",
                schema: "Payables");

            migrationBuilder.DropTable(
                name: "EmploymentActivities",
                schema: "Payables");

            migrationBuilder.DropTable(
                name: "ISWActivities",
                schema: "Payables");

            migrationBuilder.DropTable(
                name: "NonISWActivities",
                schema: "Payables");

            migrationBuilder.DropTable(
                name: "Activities",
                schema: "Payables");
        }
    }
}
