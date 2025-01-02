using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MovePayablesToActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityEscalationQueue_Activities_ActivityId",
                schema: "Payables",
                table: "ActivityEscalationQueue");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityPqaQueue_Activities_ActivityId",
                schema: "Payables",
                table: "ActivityPqaQueue");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityQa1Queue_Activities_ActivityId",
                schema: "Payables",
                table: "ActivityQa1Queue");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityQa2Queue_Activities_ActivityId",
                schema: "Payables",
                table: "ActivityQa2Queue");

            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentActivities_Activities_Id",
                schema: "Payables",
                table: "EmploymentActivities");

            migrationBuilder.DropTable(
                name: "EducationTrainingActivities",
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

            migrationBuilder.EnsureSchema(
                name: "Activities");

            migrationBuilder.RenameTable(
                name: "Qa2QueueNote",
                schema: "Payables",
                newName: "Qa2QueueNote",
                newSchema: "Activities");

            migrationBuilder.RenameTable(
                name: "Qa1QueueNote",
                schema: "Payables",
                newName: "Qa1QueueNote",
                newSchema: "Activities");

            migrationBuilder.RenameTable(
                name: "PqaQueueNote",
                schema: "Payables",
                newName: "PqaQueueNote",
                newSchema: "Activities");

            migrationBuilder.RenameTable(
                name: "EscalationNote",
                schema: "Payables",
                newName: "EscalationNote",
                newSchema: "Activities");

            migrationBuilder.RenameTable(
                name: "EmploymentActivities",
                schema: "Payables",
                newName: "EmploymentActivities",
                newSchema: "Activities");

            migrationBuilder.RenameTable(
                name: "ActivityQa2Queue",
                schema: "Payables",
                newName: "ActivityQa2Queue",
                newSchema: "Activities");

            migrationBuilder.RenameTable(
                name: "ActivityQa1Queue",
                schema: "Payables",
                newName: "ActivityQa1Queue",
                newSchema: "Activities");

            migrationBuilder.RenameTable(
                name: "ActivityPqaQueue",
                schema: "Payables",
                newName: "ActivityPqaQueue",
                newSchema: "Activities");

            migrationBuilder.RenameTable(
                name: "ActivityEscalationQueue",
                schema: "Payables",
                newName: "ActivityEscalationQueue",
                newSchema: "Activities");

            migrationBuilder.CreateTable(
                name: "Activity",
                schema: "Activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Definition = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObjectiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TookPlaceAtLocationId = table.Column<int>(type: "int", nullable: false),
                    TookPlaceAtContractId = table.Column<string>(type: "nvarchar(12)", nullable: false),
                    ParticipantCurrentLocationId = table.Column<int>(type: "int", nullable: false),
                    ParticipantCurrentContractId = table.Column<string>(type: "nvarchar(12)", nullable: true),
                    ParticipantStatus = table.Column<int>(type: "int", nullable: false),
                    AdditionalInformation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CommencedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    EditorId = table.Column<string>(type: "nvarchar(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activity_Contract_ParticipantCurrentContractId",
                        column: x => x.ParticipantCurrentContractId,
                        principalSchema: "Configuration",
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activity_Contract_TookPlaceAtContractId",
                        column: x => x.TookPlaceAtContractId,
                        principalSchema: "Configuration",
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activity_Location_ParticipantCurrentLocationId",
                        column: x => x.ParticipantCurrentLocationId,
                        principalSchema: "Configuration",
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activity_Location_TookPlaceAtLocationId",
                        column: x => x.TookPlaceAtLocationId,
                        principalSchema: "Configuration",
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activity_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activity_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "Configuration",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activity_User_EditorId",
                        column: x => x.EditorId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Activity_User_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EducationTrainingActivity",
                schema: "Activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CourseLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseCommencedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CourseCompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CourseCompletionStatus = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationTrainingActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationTrainingActivity_Activity_Id",
                        column: x => x.Id,
                        principalSchema: "Activities",
                        principalTable: "Activity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EducationTrainingActivity_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "Document",
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IswActivity",
                schema: "Activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WraparoundSupportStartedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoursPerformedPre = table.Column<double>(type: "float", nullable: false),
                    HoursPerformedDuring = table.Column<double>(type: "float", nullable: false),
                    HoursPerformedPost = table.Column<double>(type: "float", nullable: false),
                    BaselineAchievedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IswActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IswActivity_Activity_Id",
                        column: x => x.Id,
                        principalSchema: "Activities",
                        principalTable: "Activity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IswActivity_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "Document",
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NonIsqActivity",
                schema: "Activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonIsqActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NonIsqActivity_Activity_Id",
                        column: x => x.Id,
                        principalSchema: "Activities",
                        principalTable: "Activity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activity_EditorId",
                schema: "Activities",
                table: "Activity",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_OwnerId",
                schema: "Activities",
                table: "Activity",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_ParticipantCurrentContractId",
                schema: "Activities",
                table: "Activity",
                column: "ParticipantCurrentContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_ParticipantCurrentLocationId",
                schema: "Activities",
                table: "Activity",
                column: "ParticipantCurrentLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_ParticipantId",
                schema: "Activities",
                table: "Activity",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_TenantId",
                schema: "Activities",
                table: "Activity",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_TookPlaceAtContractId",
                schema: "Activities",
                table: "Activity",
                column: "TookPlaceAtContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_TookPlaceAtLocationId",
                schema: "Activities",
                table: "Activity",
                column: "TookPlaceAtLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationTrainingActivity_DocumentId",
                schema: "Activities",
                table: "EducationTrainingActivity",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_IswActivity_DocumentId",
                schema: "Activities",
                table: "IswActivity",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityEscalationQueue_Activity_ActivityId",
                schema: "Activities",
                table: "ActivityEscalationQueue",
                column: "ActivityId",
                principalSchema: "Activities",
                principalTable: "Activity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityPqaQueue_Activity_ActivityId",
                schema: "Activities",
                table: "ActivityPqaQueue",
                column: "ActivityId",
                principalSchema: "Activities",
                principalTable: "Activity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityQa1Queue_Activity_ActivityId",
                schema: "Activities",
                table: "ActivityQa1Queue",
                column: "ActivityId",
                principalSchema: "Activities",
                principalTable: "Activity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityQa2Queue_Activity_ActivityId",
                schema: "Activities",
                table: "ActivityQa2Queue",
                column: "ActivityId",
                principalSchema: "Activities",
                principalTable: "Activity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentActivities_Activity_Id",
                schema: "Activities",
                table: "EmploymentActivities",
                column: "Id",
                principalSchema: "Activities",
                principalTable: "Activity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityEscalationQueue_Activity_ActivityId",
                schema: "Activities",
                table: "ActivityEscalationQueue");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityPqaQueue_Activity_ActivityId",
                schema: "Activities",
                table: "ActivityPqaQueue");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityQa1Queue_Activity_ActivityId",
                schema: "Activities",
                table: "ActivityQa1Queue");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityQa2Queue_Activity_ActivityId",
                schema: "Activities",
                table: "ActivityQa2Queue");

            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentActivities_Activity_Id",
                schema: "Activities",
                table: "EmploymentActivities");

            migrationBuilder.DropTable(
                name: "EducationTrainingActivity",
                schema: "Activities");

            migrationBuilder.DropTable(
                name: "IswActivity",
                schema: "Activities");

            migrationBuilder.DropTable(
                name: "NonIsqActivity",
                schema: "Activities");

            migrationBuilder.DropTable(
                name: "Activity",
                schema: "Activities");

            migrationBuilder.EnsureSchema(
                name: "Payables");

            migrationBuilder.RenameTable(
                name: "Qa2QueueNote",
                schema: "Activities",
                newName: "Qa2QueueNote",
                newSchema: "Payables");

            migrationBuilder.RenameTable(
                name: "Qa1QueueNote",
                schema: "Activities",
                newName: "Qa1QueueNote",
                newSchema: "Payables");

            migrationBuilder.RenameTable(
                name: "PqaQueueNote",
                schema: "Activities",
                newName: "PqaQueueNote",
                newSchema: "Payables");

            migrationBuilder.RenameTable(
                name: "EscalationNote",
                schema: "Activities",
                newName: "EscalationNote",
                newSchema: "Payables");

            migrationBuilder.RenameTable(
                name: "EmploymentActivities",
                schema: "Activities",
                newName: "EmploymentActivities",
                newSchema: "Payables");

            migrationBuilder.RenameTable(
                name: "ActivityQa2Queue",
                schema: "Activities",
                newName: "ActivityQa2Queue",
                newSchema: "Payables");

            migrationBuilder.RenameTable(
                name: "ActivityQa1Queue",
                schema: "Activities",
                newName: "ActivityQa1Queue",
                newSchema: "Payables");

            migrationBuilder.RenameTable(
                name: "ActivityPqaQueue",
                schema: "Activities",
                newName: "ActivityPqaQueue",
                newSchema: "Payables");

            migrationBuilder.RenameTable(
                name: "ActivityEscalationQueue",
                schema: "Activities",
                newName: "ActivityEscalationQueue",
                newSchema: "Payables");

            migrationBuilder.CreateTable(
                name: "Activities",
                schema: "Payables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EditorId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    OwnerId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    ParticipantCurrentContractId = table.Column<string>(type: "nvarchar(12)", nullable: true),
                    ParticipantCurrentLocationId = table.Column<int>(type: "int", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", nullable: false),
                    TookPlaceAtContractId = table.Column<string>(type: "nvarchar(12)", nullable: false),
                    TookPlaceAtLocationId = table.Column<int>(type: "int", nullable: false),
                    AdditionalInformation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<int>(type: "int", nullable: false),
                    CommencedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Definition = table.Column<int>(type: "int", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ObjectiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantStatus = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Activities_User_EditorId",
                        column: x => x.EditorId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Activities_User_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EducationTrainingActivities",
                schema: "Payables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseCommencedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CourseCompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CourseCompletionStatus = table.Column<int>(type: "int", nullable: false),
                    CourseLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "ISWActivities",
                schema: "Payables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaselineAchievedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoursPerformedDuring = table.Column<double>(type: "float", nullable: false),
                    HoursPerformedPost = table.Column<double>(type: "float", nullable: false),
                    HoursPerformedPre = table.Column<double>(type: "float", nullable: false),
                    WraparoundSupportStartedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "IX_Activities_EditorId",
                schema: "Payables",
                table: "Activities",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_OwnerId",
                schema: "Payables",
                table: "Activities",
                column: "OwnerId");

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
                name: "IX_ISWActivities_DocumentId",
                schema: "Payables",
                table: "ISWActivities",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityEscalationQueue_Activities_ActivityId",
                schema: "Payables",
                table: "ActivityEscalationQueue",
                column: "ActivityId",
                principalSchema: "Payables",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityPqaQueue_Activities_ActivityId",
                schema: "Payables",
                table: "ActivityPqaQueue",
                column: "ActivityId",
                principalSchema: "Payables",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityQa1Queue_Activities_ActivityId",
                schema: "Payables",
                table: "ActivityQa1Queue",
                column: "ActivityId",
                principalSchema: "Payables",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityQa2Queue_Activities_ActivityId",
                schema: "Payables",
                table: "ActivityQa2Queue",
                column: "ActivityId",
                principalSchema: "Payables",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentActivities_Activities_Id",
                schema: "Payables",
                table: "EmploymentActivities",
                column: "Id",
                principalSchema: "Payables",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
