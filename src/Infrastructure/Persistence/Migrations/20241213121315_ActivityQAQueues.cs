using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ActivityQAQueues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityEscalationQueue",
                schema: "Payables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    EditorId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    IsAccepted = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 36, nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityEscalationQueue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityEscalationQueue_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "Payables",
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityEscalationQueue_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActivityEscalationQueue_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "Configuration",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivityEscalationQueue_User_EditorId",
                        column: x => x.EditorId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActivityEscalationQueue_User_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ActivityQa1Queue",
                schema: "Payables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    EditorId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    IsAccepted = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 36, nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityQa1Queue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityQa1Queue_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "Payables",
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityQa1Queue_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActivityQa1Queue_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "Configuration",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivityQa1Queue_User_EditorId",
                        column: x => x.EditorId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActivityQa1Queue_User_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ActivityQa2Queue",
                schema: "Payables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsEscalated = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    EditorId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    IsAccepted = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 36, nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityQa2Queue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityQa2Queue_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "Payables",
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityQa2Queue_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActivityQa2Queue_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "Configuration",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivityQa2Queue_User_EditorId",
                        column: x => x.EditorId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActivityQa2Queue_User_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EscalationNote",
                schema: "Payables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsExternal = table.Column<bool>(type: "bit", nullable: false),
                    ActivityEscalationQueueEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CallReference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscalationNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EscalationNote_ActivityEscalationQueue_ActivityEscalationQueueEntryId",
                        column: x => x.ActivityEscalationQueueEntryId,
                        principalSchema: "Payables",
                        principalTable: "ActivityEscalationQueue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EscalationNote_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EscalationNote_User_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Qa1QueueNote",
                schema: "Payables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsExternal = table.Column<bool>(type: "bit", nullable: false),
                    ActivityQa1QueueEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CallReference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qa1QueueNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Qa1QueueNote_ActivityQa1Queue_ActivityQa1QueueEntryId",
                        column: x => x.ActivityQa1QueueEntryId,
                        principalSchema: "Payables",
                        principalTable: "ActivityQa1Queue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Qa1QueueNote_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Qa1QueueNote_User_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Qa2QueueNote",
                schema: "Payables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsExternal = table.Column<bool>(type: "bit", nullable: false),
                    ActivityQa2QueueEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CallReference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qa2QueueNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Qa2QueueNote_ActivityQa2Queue_ActivityQa2QueueEntryId",
                        column: x => x.ActivityQa2QueueEntryId,
                        principalSchema: "Payables",
                        principalTable: "ActivityQa2Queue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Qa2QueueNote_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Qa2QueueNote_User_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityEscalationQueue_ActivityId",
                schema: "Payables",
                table: "ActivityEscalationQueue",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityEscalationQueue_EditorId",
                schema: "Payables",
                table: "ActivityEscalationQueue",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityEscalationQueue_OwnerId",
                schema: "Payables",
                table: "ActivityEscalationQueue",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityEscalationQueue_ParticipantId",
                schema: "Payables",
                table: "ActivityEscalationQueue",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityEscalationQueue_TenantId",
                schema: "Payables",
                table: "ActivityEscalationQueue",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityQa1Queue_ActivityId",
                schema: "Payables",
                table: "ActivityQa1Queue",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityQa1Queue_EditorId",
                schema: "Payables",
                table: "ActivityQa1Queue",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityQa1Queue_OwnerId",
                schema: "Payables",
                table: "ActivityQa1Queue",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityQa1Queue_ParticipantId",
                schema: "Payables",
                table: "ActivityQa1Queue",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityQa1Queue_TenantId",
                schema: "Payables",
                table: "ActivityQa1Queue",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityQa2Queue_ActivityId",
                schema: "Payables",
                table: "ActivityQa2Queue",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityQa2Queue_EditorId",
                schema: "Payables",
                table: "ActivityQa2Queue",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityQa2Queue_OwnerId",
                schema: "Payables",
                table: "ActivityQa2Queue",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityQa2Queue_ParticipantId",
                schema: "Payables",
                table: "ActivityQa2Queue",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityQa2Queue_TenantId",
                schema: "Payables",
                table: "ActivityQa2Queue",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationNote_ActivityEscalationQueueEntryId",
                schema: "Payables",
                table: "EscalationNote",
                column: "ActivityEscalationQueueEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationNote_CreatedBy",
                schema: "Payables",
                table: "EscalationNote",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationNote_LastModifiedBy",
                schema: "Payables",
                table: "EscalationNote",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Qa1QueueNote_ActivityQa1QueueEntryId",
                schema: "Payables",
                table: "Qa1QueueNote",
                column: "ActivityQa1QueueEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Qa1QueueNote_CreatedBy",
                schema: "Payables",
                table: "Qa1QueueNote",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Qa1QueueNote_LastModifiedBy",
                schema: "Payables",
                table: "Qa1QueueNote",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Qa2QueueNote_ActivityQa2QueueEntryId",
                schema: "Payables",
                table: "Qa2QueueNote",
                column: "ActivityQa2QueueEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Qa2QueueNote_CreatedBy",
                schema: "Payables",
                table: "Qa2QueueNote",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Qa2QueueNote_LastModifiedBy",
                schema: "Payables",
                table: "Qa2QueueNote",
                column: "LastModifiedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EscalationNote_User_CreatedBy",
                schema: "Enrolment",
                table: "EscalationNote");

            migrationBuilder.DropForeignKey(
                name: "FK_EscalationNote_User_LastModifiedBy",
                schema: "Enrolment",
                table: "EscalationNote");

            migrationBuilder.DropForeignKey(
                name: "FK_Qa1QueueNote_User_CreatedBy",
                schema: "Enrolment",
                table: "Qa1QueueNote");

            migrationBuilder.DropForeignKey(
                name: "FK_Qa1QueueNote_User_LastModifiedBy",
                schema: "Enrolment",
                table: "Qa1QueueNote");

            migrationBuilder.DropForeignKey(
                name: "FK_Qa2QueueNote_User_CreatedBy",
                schema: "Enrolment",
                table: "Qa2QueueNote");

            migrationBuilder.DropForeignKey(
                name: "FK_Qa2QueueNote_User_LastModifiedBy",
                schema: "Enrolment",
                table: "Qa2QueueNote");

            migrationBuilder.DropTable(
                name: "EscalationNote",
                schema: "Payables");

            migrationBuilder.DropTable(
                name: "Qa1QueueNote",
                schema: "Payables");

            migrationBuilder.DropTable(
                name: "Qa2QueueNote",
                schema: "Payables");

            migrationBuilder.DropTable(
                name: "ActivityEscalationQueue",
                schema: "Payables");

            migrationBuilder.DropTable(
                name: "ActivityQa1Queue",
                schema: "Payables");

            migrationBuilder.DropTable(
                name: "ActivityQa2Queue",
                schema: "Payables");
        }
    }
}
