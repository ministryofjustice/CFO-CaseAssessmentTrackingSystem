using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Addabandonedactivitytimeline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 INSERT INTO Participant.Timeline
                                 (
                                     ParticipantId,
                                     EventType,
                                     Line1,
                                     Line2,
                                     Line3,
                                     Created,
                                     CreatedBy,
                                     LastModified,
                                     LastModifiedBy
                                 )
                                 SELECT
                                     a.ParticipantId,
                                     8 AS EventType,
                                     'Activity abandoned' AS Line1,
                                     CASE a.AbandonReason
                                         WHEN 0 THEN 'No longer required'
                                         WHEN 1 THEN 'No longer engaged'
                                         WHEN 2 THEN 'Duplicate claim'
                                         WHEN 3 THEN 'Expired'
                                         WHEN 4 THEN 'Created by accident'
                                         WHEN 5 THEN 'Other'
                                         ELSE 'Unknown'
                                         END AS Line2,
                                     NULL AS Line3,
                                     a.CompletedOn AS Created,
                                     a.CompletedBy AS CreatedBy,
                                     NULL AS LastModified,
                                     NULL AS LastModifiedBy
                                 FROM Activities.Activity AS a
                                 WHERE a.Status = 4
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                    DELETE FROM Participant.Timeline
                    WHERE Line1 = 'Activity abandoned'
                """);
        }
    }
}
