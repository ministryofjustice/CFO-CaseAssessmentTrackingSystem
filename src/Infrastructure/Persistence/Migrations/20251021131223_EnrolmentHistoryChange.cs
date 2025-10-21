using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnrolmentHistoryChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "From",
                schema: "Participant",
                table: "EnrolmentHistory",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "To",
                schema: "Participant",
                table: "EnrolmentHistory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql(@"EXEC('UPDATE Participant.EnrolmentHistory SET [FROM] = [Created];')");

            migrationBuilder.Sql(@"
                EXEC('WITH NextEnrollmentStart AS (
                        SELECT
                            eh.Id AS PK, 
                            LEAD(eh.[From]) OVER (
                                PARTITION BY eh.ParticipantId
                                ORDER BY eh.[From] ASC
                            ) AS NextFromDate
                        FROM
                            Participant.EnrolmentHistory AS eh
                    )
                    -- 2. Update the base table
                    UPDATE eh
                    SET eh.[To] = nes.NextFromDate
                    FROM
                        Participant.EnrolmentHistory AS eh
                    INNER JOIN
                        NextEnrollmentStart AS nes 
                        ON eh.Id = nes.PK;')
                                ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "From",
                schema: "Participant",
                table: "EnrolmentHistory");

            migrationBuilder.DropColumn(
                name: "To",
                schema: "Participant",
                table: "EnrolmentHistory");
        }
    }
}
