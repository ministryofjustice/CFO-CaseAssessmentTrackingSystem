using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MI_Add_ArchiveCase_table_and_backfill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchivedCase",
                schema: "Mi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    EnrolmentHistoryId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    AdditionalInfo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ArchiveReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    From = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContractId = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    LocationType = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedCase", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ArchivedCase_Participant_Enrolment",
                schema: "Mi",
                table: "ArchivedCase",
                columns: new[] { "ParticipantId", "EnrolmentHistoryId", "TenantId", "From" });
            
              migrationBuilder.Sql("""
                INSERT INTO Mi.ArchivedCase
                (
                    Id,
                    ParticipantId,
                    EnrolmentHistoryId,
                    Created,
                    CreatedBy,
                    AdditionalInfo,
                    ArchiveReason,
                    [From],
                    ContractId,
                    LocationId,
                    LocationType,
                    TenantId
                )
                SELECT
                    NEWID()                               AS Id,
                    p.Id                                  AS ParticipantId,
                    eh.Id                                 AS EnrolmentHistoryId,
                    eh.Created                            AS Created,
                    eh.CreatedBy                          AS CreatedBy,
                    eh.AdditionalInformation              AS AdditionalInfo,
                    eh.Reason                             AS ArchiveReason,
                    eh.[From]                             AS [From],
                    l.ContractId                          AS ContractId,
                    l.Id                                  AS LocationId,
                    l.LocationTypeId                      AS LocationType,
                    u.TenantId                            AS TenantId
                FROM Participant.Participant p
                JOIN Participant.EnrolmentHistory eh
                    ON p.Id = eh.ParticipantId
                JOIN [Identity].[User] u
                    ON eh.CreatedBy = u.Id
                OUTER APPLY
                (
                    SELECT TOP (1) lh.*
                    FROM Participant.LocationHistory lh
                    WHERE lh.ParticipantId = p.Id
                      AND lh.[From] <= eh.[From]
                      AND (lh.[To] IS NULL OR eh.[From] <= lh.[To])
                    ORDER BY lh.[From] DESC
                ) lh
                JOIN Configuration.Location l
                    ON l.Id = lh.LocationId
                WHERE eh.EnrolmentStatus = 4
                  AND NOT EXISTS
                  (
                      SELECT 1
                      FROM Mi.ArchivedCase ac
                      WHERE ac.EnrolmentHistoryId = eh.Id
                  );
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivedCase",
                schema: "Mi");
        }
    }
}
