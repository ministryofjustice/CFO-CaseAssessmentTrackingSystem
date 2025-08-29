using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Mi_ParticipantEngagementModifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Mi",
                table: "ParticipantEngagement");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                schema: "Mi",
                table: "ParticipantEngagement",
                newName: "EngagedWithTenant");

            migrationBuilder.AddColumn<string>(
                name: "EngagedAtContract",
                schema: "Mi",
                table: "ParticipantEngagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EngagedAtLocation",
                schema: "Mi",
                table: "ParticipantEngagement",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EngagedWith",
                schema: "Mi",
                table: "ParticipantEngagement",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EngagedAtContract",
                schema: "Mi",
                table: "ParticipantEngagement");

            migrationBuilder.DropColumn(
                name: "EngagedAtLocation",
                schema: "Mi",
                table: "ParticipantEngagement");

            migrationBuilder.DropColumn(
                name: "EngagedWith",
                schema: "Mi",
                table: "ParticipantEngagement");

            migrationBuilder.RenameColumn(
                name: "EngagedWithTenant",
                schema: "Mi",
                table: "ParticipantEngagement",
                newName: "TenantId");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "Mi",
                table: "ParticipantEngagement",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");
        }
    }
}
