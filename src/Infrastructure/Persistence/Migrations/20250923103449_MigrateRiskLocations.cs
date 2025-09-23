using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MigrateRiskLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                           UPDATE r
                           SET 
                                r.LocationId = lh.LocationId
                           FROM
                                Participant.Risk r
                           INNER JOIN 
                                Participant.LocationHistory lh on lh.ParticipantId = r.ParticipantId
                           AND 
                            CAST(r.Created as Date) BETWEEN CAST(lh.[From] as Date) AND CAST(ISNULL(lh.[To], '2099-12-31 00:00:00.0000000') AS DATE)
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
