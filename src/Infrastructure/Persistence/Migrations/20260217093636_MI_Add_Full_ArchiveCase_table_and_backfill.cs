using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MI_Add_Full_ArchiveCase_table_and_backfill : Migration
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
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EnrolmentHistoryId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    AdditionalInfo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ArchiveReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UnarchiveAdditionalInfo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UnarchiveReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    From = table.Column<DateTime>(type: "datetime2", nullable: false),
                    To = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractId = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
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
                                     FirstName,
                                     LastName,
                                     EnrolmentHistoryId,
                                     Created,
                                     CreatedBy,
                                     AdditionalInfo,
                                     ArchiveReason,
                                     UnarchiveAdditionalInfo,
                                     UnarchiveReason,
                                     [From],
                                     [To],
                                     ContractId,
                                     LocationId,
                                     LocationType,
                                     TenantId
                                 )
                                 SELECT
                                     NEWID(),
                                     p.Id,
                                     p.FirstName,
                                     p.LastName,
                                     eh.Id,
                                     eh.Created,
                                     eh.CreatedBy,
                                     eh.AdditionalInformation,
                                     eh.Reason,

                                     unarchive.AdditionalInformation,
                                     unarchive.Reason,

                                     eh.[From],
                                     eh.[To],

                                     l.ContractId,
                                     l.Id,

                                     CASE l.LocationTypeId
                                         WHEN 0 THEN 'Wing'
                                         WHEN 1 THEN 'Feeder'
                                         WHEN 2 THEN 'Outlying'
                                         WHEN 3 THEN 'Female'
                                         WHEN 4 THEN 'Community'
                                         WHEN 5 THEN 'Hub'
                                         WHEN 6 THEN 'Satellite'
                                         WHEN 7 THEN 'Unknown'
                                         WHEN 8 THEN 'Unmapped Custody'
                                         WHEN 9 THEN 'Unmapped Community'
                                         ELSE 'Unknown'
                                     END,

                                     u.TenantId

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

                                 LEFT JOIN Configuration.Location l
                                     ON l.Id = lh.LocationId

                                 -- 🔵 find first NON archived after this archived period
                                 OUTER APPLY
                                 (
                                     SELECT TOP (1)
                                         peh.Reason,
                                         peh.AdditionalInformation
                                     FROM Participant.EnrolmentHistory peh
                                     WHERE peh.ParticipantId = p.Id
                                       AND peh.[From] > eh.[From]
                                       AND (eh.[To] IS NULL OR peh.[From] <= eh.[To])
                                       AND peh.EnrolmentStatus <> 4
                                     ORDER BY peh.[From] ASC
                                 ) unarchive

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