using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ArchivedCase_Add_Unarchived_AddInfo_and_Reason_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ArchiveReason",
                schema: "Mi",
                table: "ArchivedCase",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnarchiveAdditionalInfo",
                schema: "Mi",
                table: "ArchivedCase",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnarchiveReason",
                schema: "Mi",
                table: "ArchivedCase",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
            //
            migrationBuilder.Sql(@"
                    UPDATE ac
                    SET ac.UnarchiveReason = peh.Reason,
                        ac.UnarchiveAdditionalInfo = peh.AdditionalInformation
                    FROM Mi.ArchivedCase ac
                    OUTER APPLY (
                        SELECT TOP (1) peh.Reason, peh.AdditionalInformation
                        FROM Participant.EnrolmentHistory peh
                        WHERE peh.ParticipantId = ac.ParticipantId
                          AND peh.[From] > ac.[From]
                          AND peh.[From] <= ac.[To]
                          AND peh.EnrolmentStatus <> 4
                        ORDER BY peh.[From] ASC
                    ) peh
                    WHERE ac.[To] IS NOT NULL
                      AND ac.UnarchiveReason IS NULL;
                       ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnarchiveAdditionalInfo",
                schema: "Mi",
                table: "ArchivedCase");

            migrationBuilder.DropColumn(
                name: "UnarchiveReason",
                schema: "Mi",
                table: "ArchivedCase");

            migrationBuilder.AlterColumn<string>(
                name: "ArchiveReason",
                schema: "Mi",
                table: "ArchivedCase",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
