using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ActivityFeedbackTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityFeedback",
                schema: "Activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    RecipientUserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Outcome = table.Column<int>(type: "int", nullable: false),
                    Stage = table.Column<int>(type: "int", nullable: false),
                    ActivityProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    OwnerId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    EditorId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityFeedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityFeedback_Activity_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "Activities",
                        principalTable: "Activity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivityFeedback_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivityFeedback_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "Configuration",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivityFeedback_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivityFeedback_User_EditorId",
                        column: x => x.EditorId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActivityFeedback_User_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActivityFeedback_User_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityFeedback_ActivityId",
                schema: "Activities",
                table: "ActivityFeedback",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityFeedback_CreatedBy",
                schema: "Activities",
                table: "ActivityFeedback",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityFeedback_EditorId",
                schema: "Activities",
                table: "ActivityFeedback",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityFeedback_OwnerId",
                schema: "Activities",
                table: "ActivityFeedback",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityFeedback_ParticipantId",
                schema: "Activities",
                table: "ActivityFeedback",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityFeedback_RecipientUserId_IsRead",
                schema: "Activities",
                table: "ActivityFeedback",
                columns: new[] { "RecipientUserId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityFeedback_TenantId",
                schema: "Activities",
                table: "ActivityFeedback",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityFeedback",
                schema: "Activities");
        }
    }
}
