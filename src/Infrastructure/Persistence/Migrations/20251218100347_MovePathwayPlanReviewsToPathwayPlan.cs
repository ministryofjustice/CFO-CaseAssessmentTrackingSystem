using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MovePathwayPlanReviewsToPathwayPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 INSERT INTO Participant.PathwayPlanReview
                                 (
                                  Id,
                                     PathwayPlanId,
                                     LocationId,
                                     ReviewDate,
                                     Review,
                                     ReviewReason,
                                     ParticipantId,
                                     Created,
                                     CreatedBy,
                                     LastModified,
                                     LastModifiedBy
                                 )
                                 SELECT
                                     newid(),
                                     pprh.PathwayPlanId,
                                     loc.LocationId,
                                     pprh.Created AS ReviewDate,
                                     'Original Review' AS Review,
                                     0 AS ReviewReason,
                                     pp.ParticipantId,
                                     pprh.Created AS Created,
                                     pprh.CreatedBy,
                                     null,
                                     null
                                 FROM Participant.PathwayPlanReviewHistory pprh
                                          INNER JOIN Participant.PathwayPlan pp
                                                     ON pp.Id = pprh.PathwayPlanId
                                          OUTER APPLY
                                      (
                                          SELECT TOP (1)
                                              lh.LocationId
                                          FROM Participant.LocationHistory lh
                                          WHERE lh.ParticipantId = pp.ParticipantId
                                            AND lh.[From] <= pprh.Created
                                          ORDER BY lh.[From] DESC
                                      ) loc;
                                 """);
        
            migrationBuilder.DropForeignKey(
                name: "FK_PathwayPlanReview_Location_LocationId",
                schema: "Participant",
                table: "PathwayPlanReview");

            migrationBuilder.DropTable(
                name: "PathwayPlanReviewHistory",
                schema: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_PathwayPlanReview_LocationId",
                schema: "Participant",
                table: "PathwayPlanReview");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PathwayPlanReviewHistory",
                schema: "Participant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    PathwayPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PathwayPlanReviewHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PathwayPlanReviewHistory_PathwayPlan_PathwayPlanId",
                        column: x => x.PathwayPlanId,
                        principalSchema: "Participant",
                        principalTable: "PathwayPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PathwayPlanReview_LocationId",
                schema: "Participant",
                table: "PathwayPlanReview",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PathwayPlanReviewHistory_PathwayPlanId",
                schema: "Participant",
                table: "PathwayPlanReviewHistory",
                column: "PathwayPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_PathwayPlanReview_Location_LocationId",
                schema: "Participant",
                table: "PathwayPlanReview",
                column: "LocationId",
                principalSchema: "Configuration",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
