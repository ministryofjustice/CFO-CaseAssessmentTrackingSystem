using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPathwayPlanReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PathwayPlanReview",
                schema: "Participant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PathwayPlanId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 36, nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Review = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewReason = table.Column<int>(type: "int", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PathwayPlanReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PathwayPlanReview_Location_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "Configuration",
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PathwayPlanReview_LocationId",
                schema: "Participant",
                table: "PathwayPlanReview",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PathwayPlanReview_ParticipantId",
                schema: "Participant",
                table: "PathwayPlanReview",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_PathwayPlanReview_PathwayPlanId",
                schema: "Participant",
                table: "PathwayPlanReview",
                column: "PathwayPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PathwayPlanReview",
                schema: "Participant");
        }
    }
}
