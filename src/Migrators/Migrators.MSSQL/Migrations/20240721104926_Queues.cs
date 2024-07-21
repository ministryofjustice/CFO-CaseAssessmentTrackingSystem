using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class Queues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Enrolment");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Participant",
                table: "Note",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Identity",
                table: "Note",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EscalationQueue",
                schema: "Enrolment",
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
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscalationQueue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EscalationQueue_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EscalationQueue_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "Configuration",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EscalationQueue_User_EditorId",
                        column: x => x.EditorId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EscalationQueue_User_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PqaQueue",
                schema: "Enrolment",
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
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PqaQueue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PqaQueue_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PqaQueue_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "Configuration",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PqaQueue_User_EditorId",
                        column: x => x.EditorId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PqaQueue_User_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Qa1Queue",
                schema: "Enrolment",
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
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qa1Queue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Qa1Queue_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Qa1Queue_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "Configuration",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Qa1Queue_User_EditorId",
                        column: x => x.EditorId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Qa1Queue_User_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Qa2Queue",
                schema: "Enrolment",
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
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qa2Queue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Qa2Queue_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Qa2Queue_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "Configuration",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Qa2Queue_User_EditorId",
                        column: x => x.EditorId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Qa2Queue_User_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EscalationNote",
                schema: "Enrolment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CallReference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnrolmentEscalationQueueEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscalationNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EscalationNote_EscalationQueue_EnrolmentEscalationQueueEntryId",
                        column: x => x.EnrolmentEscalationQueueEntryId,
                        principalSchema: "Enrolment",
                        principalTable: "EscalationQueue",
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
                name: "PqaQueueNote",
                schema: "Enrolment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CallReference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnrolmentPqaQueueEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PqaQueueNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PqaQueueNote_PqaQueue_EnrolmentPqaQueueEntryId",
                        column: x => x.EnrolmentPqaQueueEntryId,
                        principalSchema: "Enrolment",
                        principalTable: "PqaQueue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PqaQueueNote_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PqaQueueNote_User_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Qa1QueueNote",
                schema: "Enrolment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CallReference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnrolmentQa1QueueEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qa1QueueNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Qa1QueueNote_Qa1Queue_EnrolmentQa1QueueEntryId",
                        column: x => x.EnrolmentQa1QueueEntryId,
                        principalSchema: "Enrolment",
                        principalTable: "Qa1Queue",
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
                schema: "Enrolment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CallReference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnrolmentQa2QueueEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qa2QueueNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Qa2QueueNote_Qa2Queue_EnrolmentQa2QueueEntryId",
                        column: x => x.EnrolmentQa2QueueEntryId,
                        principalSchema: "Enrolment",
                        principalTable: "Qa2Queue",
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
                name: "IX_EscalationNote_CreatedBy",
                schema: "Enrolment",
                table: "EscalationNote",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationNote_EnrolmentEscalationQueueEntryId",
                schema: "Enrolment",
                table: "EscalationNote",
                column: "EnrolmentEscalationQueueEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationNote_LastModifiedBy",
                schema: "Enrolment",
                table: "EscalationNote",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationQueue_EditorId",
                schema: "Enrolment",
                table: "EscalationQueue",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationQueue_OwnerId",
                schema: "Enrolment",
                table: "EscalationQueue",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationQueue_ParticipantId",
                schema: "Enrolment",
                table: "EscalationQueue",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationQueue_TenantId",
                schema: "Enrolment",
                table: "EscalationQueue",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PqaQueue_EditorId",
                schema: "Enrolment",
                table: "PqaQueue",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_PqaQueue_OwnerId",
                schema: "Enrolment",
                table: "PqaQueue",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PqaQueue_ParticipantId",
                schema: "Enrolment",
                table: "PqaQueue",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_PqaQueue_TenantId",
                schema: "Enrolment",
                table: "PqaQueue",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PqaQueueNote_CreatedBy",
                schema: "Enrolment",
                table: "PqaQueueNote",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PqaQueueNote_EnrolmentPqaQueueEntryId",
                schema: "Enrolment",
                table: "PqaQueueNote",
                column: "EnrolmentPqaQueueEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_PqaQueueNote_LastModifiedBy",
                schema: "Enrolment",
                table: "PqaQueueNote",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Qa1Queue_EditorId",
                schema: "Enrolment",
                table: "Qa1Queue",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_Qa1Queue_OwnerId",
                schema: "Enrolment",
                table: "Qa1Queue",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Qa1Queue_ParticipantId",
                schema: "Enrolment",
                table: "Qa1Queue",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Qa1Queue_TenantId",
                schema: "Enrolment",
                table: "Qa1Queue",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Qa1QueueNote_CreatedBy",
                schema: "Enrolment",
                table: "Qa1QueueNote",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Qa1QueueNote_EnrolmentQa1QueueEntryId",
                schema: "Enrolment",
                table: "Qa1QueueNote",
                column: "EnrolmentQa1QueueEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Qa1QueueNote_LastModifiedBy",
                schema: "Enrolment",
                table: "Qa1QueueNote",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Qa2Queue_EditorId",
                schema: "Enrolment",
                table: "Qa2Queue",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_Qa2Queue_OwnerId",
                schema: "Enrolment",
                table: "Qa2Queue",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Qa2Queue_ParticipantId",
                schema: "Enrolment",
                table: "Qa2Queue",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Qa2Queue_TenantId",
                schema: "Enrolment",
                table: "Qa2Queue",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Qa2QueueNote_CreatedBy",
                schema: "Enrolment",
                table: "Qa2QueueNote",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Qa2QueueNote_EnrolmentQa2QueueEntryId",
                schema: "Enrolment",
                table: "Qa2QueueNote",
                column: "EnrolmentQa2QueueEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Qa2QueueNote_LastModifiedBy",
                schema: "Enrolment",
                table: "Qa2QueueNote",
                column: "LastModifiedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EscalationNote",
                schema: "Enrolment");

            migrationBuilder.DropTable(
                name: "PqaQueueNote",
                schema: "Enrolment");

            migrationBuilder.DropTable(
                name: "Qa1QueueNote",
                schema: "Enrolment");

            migrationBuilder.DropTable(
                name: "Qa2QueueNote",
                schema: "Enrolment");

            migrationBuilder.DropTable(
                name: "EscalationQueue",
                schema: "Enrolment");

            migrationBuilder.DropTable(
                name: "PqaQueue",
                schema: "Enrolment");

            migrationBuilder.DropTable(
                name: "Qa1Queue",
                schema: "Enrolment");

            migrationBuilder.DropTable(
                name: "Qa2Queue",
                schema: "Enrolment");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Participant",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Identity",
                table: "Note");
        }
    }
}
