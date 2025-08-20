using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Mi_ParticipantEngagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParticipantEngagement",
                schema: "Mi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    EngagedOn = table.Column<DateOnly>(type: "date", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantEngagement", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantEngagement_EngagedOn_ParticipantId",
                schema: "Mi",
                table: "ParticipantEngagement",
                columns: new[] { "EngagedOn", "ParticipantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParticipantEngagement",
                schema: "Mi");
        }
    }
}
