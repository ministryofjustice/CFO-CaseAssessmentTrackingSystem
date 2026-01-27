using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderFeedbackEnrolmentAndActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProviderFeedbackActivity",
                schema: "Mi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SourceTable = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Queue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QueueEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoteId = table.Column<int>(type: "int", maxLength: 36, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContractId = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    SupportWorkerId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ProviderQaUserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    CfoUserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    PqaSubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoteCreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoteCreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FeedbackType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderFeedbackActivity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderFeedbackEnrolment",
                schema: "Mi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SourceTable = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Queue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QueueEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoteId = table.Column<int>(type: "int", maxLength: 36, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContractId = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    SupportWorkerId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ProviderQaUserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    CfoUserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    PqaSubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoteCreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoteCreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FeedbackType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderFeedbackEnrolment", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ProviderFeedbackActivity_SourceTable_QueueEntryId_NoteId",
                schema: "Mi",
                table: "ProviderFeedbackActivity",
                columns: new[] { "SourceTable", "QueueEntryId", "NoteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ProviderFeedbackActivity_SourceTable_TenantId_ActionDate",
                schema: "Mi",
                table: "ProviderFeedbackActivity",
                columns: new[] { "SourceTable", "TenantId", "ActionDate" });

            migrationBuilder.CreateIndex(
                name: "ix_ProviderFeedbackEnrolment_SourceTable_QueueEntryId_NoteId",
                schema: "Mi",
                table: "ProviderFeedbackEnrolment",
                columns: new[] { "SourceTable", "QueueEntryId", "NoteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ProviderFeedbackEnrolment_SourceTable_TenantId_ActionDate",
                schema: "Mi",
                table: "ProviderFeedbackEnrolment",
                columns: new[] { "SourceTable", "TenantId", "ActionDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderFeedbackActivity",
                schema: "Mi");

            migrationBuilder.DropTable(
                name: "ProviderFeedbackEnrolment",
                schema: "Mi");
        }
    }
}
