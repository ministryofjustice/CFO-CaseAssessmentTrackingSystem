using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class ActivityPqaQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityPqaQueue",
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
                    table.PrimaryKey("PK_ActivityPqaQueue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityPqaQueue_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "Payables",
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityPqaQueue_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActivityPqaQueue_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "Configuration",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivityPqaQueue_User_EditorId",
                        column: x => x.EditorId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActivityPqaQueue_User_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PqaQueueNote",
                schema: "Payables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityPqaQueueEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_PqaQueueNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PqaQueueNote_ActivityPqaQueue_ActivityPqaQueueEntryId",
                        column: x => x.ActivityPqaQueueEntryId,
                        principalSchema: "Payables",
                        principalTable: "ActivityPqaQueue",
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

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPqaQueue_ActivityId",
                schema: "Payables",
                table: "ActivityPqaQueue",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPqaQueue_EditorId",
                schema: "Payables",
                table: "ActivityPqaQueue",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPqaQueue_OwnerId",
                schema: "Payables",
                table: "ActivityPqaQueue",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPqaQueue_ParticipantId",
                schema: "Payables",
                table: "ActivityPqaQueue",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPqaQueue_TenantId",
                schema: "Payables",
                table: "ActivityPqaQueue",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PqaQueueNote_ActivityPqaQueueEntryId",
                schema: "Payables",
                table: "PqaQueueNote",
                column: "ActivityPqaQueueEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_PqaQueueNote_CreatedBy",
                schema: "Payables",
                table: "PqaQueueNote",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PqaQueueNote_LastModifiedBy",
                schema: "Payables",
                table: "PqaQueueNote",
                column: "LastModifiedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PqaQueueNote_User_CreatedBy",
                schema: "Enrolment",
                table: "PqaQueueNote");

            migrationBuilder.DropForeignKey(
                name: "FK_PqaQueueNote_User_LastModifiedBy",
                schema: "Enrolment",
                table: "PqaQueueNote");

            migrationBuilder.DropTable(
                name: "PqaQueueNote",
                schema: "Payables");

            migrationBuilder.DropTable(
                name: "ActivityPqaQueue",
                schema: "Payables");
        }
    }
}
