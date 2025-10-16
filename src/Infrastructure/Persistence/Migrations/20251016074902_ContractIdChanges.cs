using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ContractIdChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContractId",
                schema: "Configuration",
                table: "Tenant",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContractId",
                schema: "Participant",
                table: "OwnershipHistory",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.Sql(@"
                                    EXEC('
                                        UPDATE [T]
                                        SET [T].[ContractId] = c.Id
                                        FROM [Configuration].[Tenant] as [T]
                                        INNER JOIN [Configuration].[Contract] as [C]
                                        ON LEFT([T].[Id], 8) = [C].TenantId
                                    ')");

            migrationBuilder.Sql(@"
                                    EXEC('
                                    UPDATE oh 
                                    SET ContractId = t.ContractId
                                    FROM Participant.OwnershipHistory as [oh]
                                    INNER JOIN Configuration.Tenant as t on oh.TenantId = t.Id
                                    ')");

            migrationBuilder.Sql(@"
                                EXEC('
                                UPDATE oh
                                SET oh.ContractId = lh.ContractId
                                FROM Participant.OwnershipHistory oh
                                OUTER APPLY (
                                    SELECT TOP 1 
                                        lh.LocationId,
                                        lh.[FROM],
                                        lh.[TO],
                                        l.ContractId
                                    FROM 
                                        Participant.LocationHistory lh
                                    INNER JOIN 
                                        [Configuration].[Location] as l on lh.LocationId = l.Id
                                    WHERE 
                                        lh.ParticipantId = oh.ParticipantId
                                        AND lh.[FROM] <= oh.[FROM]
                                    ORDER BY 
                                        lh.[FROM] DESC
                                ) lh
                                WHERE oh.ContractId is null
                                ')");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractId",
                schema: "Configuration",
                table: "Tenant");

            migrationBuilder.DropColumn(
                name: "ContractId",
                schema: "Participant",
                table: "OwnershipHistory");
        }
    }
}
