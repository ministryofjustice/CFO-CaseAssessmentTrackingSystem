using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RiskToSelf_TextualInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RiskToSelfInCommunityText",
                schema: "Participant",
                table: "Risk",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiskToSelfInCustodyText",
                schema: "Participant",
                table: "Risk",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE Participant.Risk 
                SET 
                    RiskToSelfInCommunityText = 
                        CASE 
                            WHEN RiskToSelfInCommunity IS NOT NULL THEN
                                CASE RiskToSelfInCommunity
                                    WHEN -1 THEN 'Not known'
                                    WHEN 1 THEN 'Low'
                                    WHEN 2 THEN 'Medium'
                                    WHEN 3 THEN 'High'
                                    WHEN 4 THEN 'Very high'
                                    ELSE NULL 
                                END
                            WHEN RiskToSelfInCommunityNew IS NOT NULL THEN
                                CASE RiskToSelfInCommunityNew
                                    WHEN -1 THEN 'Unknown'
                                    WHEN 0 THEN 'No'
                                    WHEN 1 THEN 'Yes'
                                    ELSE NULL 
                                END
                            ELSE NULL 
                        END,
                    RiskToSelfInCustodyText = 
                        CASE 
                            WHEN RiskToSelfInCustody IS NOT NULL THEN
                                CASE RiskToSelfInCustody
                                    WHEN -1 THEN 'Not known'
                                    WHEN 1 THEN 'Low'
                                    WHEN 2 THEN 'Medium'
                                    WHEN 3 THEN 'High'
                                    WHEN 4 THEN 'Very high'
                                    ELSE NULL 
                                END
                            WHEN RiskToSelfInCustodyNew IS NOT NULL THEN
                                CASE RiskToSelfInCustodyNew
                                    WHEN -1 THEN 'Unknown'
                                    WHEN 0 THEN 'No'
                                    WHEN 1 THEN 'Yes'
                                    ELSE NULL 
                                END
                            ELSE NULL 
                        END;");

            migrationBuilder.DropColumn(
                name: "RiskToSelfInCommunity",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "RiskToSelfInCommunityNew",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "RiskToSelfInCustody",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "RiskToSelfInCustodyNew",
                schema: "Participant",
                table: "Risk");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RiskToSelfInCommunity",
                schema: "Participant",
                table: "Risk",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RiskToSelfInCommunityNew",
                schema: "Participant",
                table: "Risk",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RiskToSelfInCustody",
                schema: "Participant",
                table: "Risk",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RiskToSelfInCustodyNew",
                schema: "Participant",
                table: "Risk",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE Participant.Risk 
                SET 
                    RiskToSelfInCommunity = 
                        CASE RiskToSelfInCommunityText
                            WHEN 'Not known' THEN -1
                            WHEN 'Low' THEN 1
                            WHEN 'Medium' THEN 2
                            WHEN 'High' THEN 3
                            WHEN 'Very high' THEN 4
                            ELSE NULL 
                        END,
                    RiskToSelfInCommunityNew = 
                        CASE RiskToSelfInCommunityText
                            WHEN 'Unknown' THEN -1
                            WHEN 'No' THEN 0
                            WHEN 'Yes' THEN 1
                            ELSE NULL 
                        END,
                    RiskToSelfInCustody = 
                        CASE RiskToSelfInCustodyText
                            WHEN 'Not known' THEN -1
                            WHEN 'Low' THEN 1
                            WHEN 'Medium' THEN 2
                            WHEN 'High' THEN 3
                            WHEN 'Very high' THEN 4
                            ELSE NULL 
                        END,
                    RiskToSelfInCustodyNew = 
                        CASE RiskToSelfInCustodyText
                            WHEN 'Unknown' THEN -1
                            WHEN 'No' THEN 0
                            WHEN 'Yes' THEN 1
                            ELSE NULL 
                        END;");

            migrationBuilder.DropColumn(
                name: "RiskToSelfInCommunityText",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "RiskToSelfInCustodyText",
                schema: "Participant",
                table: "Risk");
        }
    }
}
