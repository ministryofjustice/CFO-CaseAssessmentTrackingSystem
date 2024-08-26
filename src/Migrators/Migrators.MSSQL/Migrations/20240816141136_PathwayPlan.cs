using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class PathwayPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Objective",
                schema: "Participant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Completed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedStatus = table.Column<int>(type: "int", nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    PathwayPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Justification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objective", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Objective_PathwayPlan_PathwayPlanId",
                        column: x => x.PathwayPlanId,
                        principalSchema: "Participant",
                        principalTable: "PathwayPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "ObjectiveTask",
                schema: "Participant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Due = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Completed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CompletedStatus = table.Column<int>(type: "int", nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Justification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ObjectiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectiveTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectiveTask_Objective_ObjectiveId",
                        column: x => x.ObjectiveId,
                        principalSchema: "Participant",
                        principalTable: "Objective",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectiveTask_User_CompletedBy",
                        column: x => x.CompletedBy,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Objective_PathwayPlanId",
                schema: "Participant",
                table: "Objective",
                column: "PathwayPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveTask_CompletedBy",
                schema: "Participant",
                table: "ObjectiveTask",
                column: "CompletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveTask_ObjectiveId",
                schema: "Participant",
                table: "ObjectiveTask",
                column: "ObjectiveId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectiveTask",
                schema: "Participant");

            migrationBuilder.DropTable(
                name: "PathwayPlanReviewHistory",
                schema: "Participant");

            migrationBuilder.DropTable(
                name: "Objective",
                schema: "Participant");

            migrationBuilder.DropTable(
                name: "PathwayPlan",
                schema: "Participant");
        }
    }
}
