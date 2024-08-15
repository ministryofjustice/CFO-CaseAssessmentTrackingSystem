using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class Objectives_v8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Objective_ParticipantId",
                schema: "Participant",
                table: "Objective");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                schema: "Participant",
                table: "Objective");

            migrationBuilder.AddColumn<Guid>(
                name: "PathwayPlanId",
                schema: "Participant",
                table: "Objective",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "PathwayPlan",
                schema: "Participant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PathwayPlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PathwayPlanReviewHistory",
                schema: "Participant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PathwayPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
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
                name: "IX_Objective_PathwayPlanId",
                schema: "Participant",
                table: "Objective",
                column: "PathwayPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PathwayPlan_ParticipantId",
                schema: "Participant",
                table: "PathwayPlan",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_PathwayPlanReviewHistory_PathwayPlanId",
                schema: "Participant",
                table: "PathwayPlanReviewHistory",
                column: "PathwayPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Objective_PathwayPlan_PathwayPlanId",
                schema: "Participant",
                table: "Objective",
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
                name: "FK_Objective_PathwayPlan_PathwayPlanId",
                schema: "Participant",
                table: "Objective");

            migrationBuilder.DropTable(
                name: "PathwayPlanReviewHistory",
                schema: "Participant");

            migrationBuilder.DropTable(
                name: "PathwayPlan",
                schema: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_Objective_PathwayPlanId",
                schema: "Participant",
                table: "Objective");

            migrationBuilder.DropColumn(
                name: "PathwayPlanId",
                schema: "Participant",
                table: "Objective");

            migrationBuilder.AddColumn<string>(
                name: "ParticipantId",
                schema: "Participant",
                table: "Objective",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Objective_ParticipantId",
                schema: "Participant",
                table: "Objective",
                column: "ParticipantId");
        }
    }
}
