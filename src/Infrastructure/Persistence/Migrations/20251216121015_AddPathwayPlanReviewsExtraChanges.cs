using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPathwayPlanReviewsExtraChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_PathwayPlanReview_PathwayPlan_PathwayPlanId",
                schema: "Participant",
                table: "PathwayPlanReview",
                column: "PathwayPlanId",
                principalSchema: "Participant",
                principalTable: "PathwayPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PathwayPlanReview_PathwayPlan_PathwayPlanId",
                schema: "Participant",
                table: "PathwayPlanReview");
        }
    }
}
