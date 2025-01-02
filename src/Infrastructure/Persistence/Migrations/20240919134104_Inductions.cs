using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Inductions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Induction");

            migrationBuilder.CreateTable(
                name: "HubInduction",
                schema: "Induction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    InductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    EditorId = table.Column<string>(type: "nvarchar(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HubInduction", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_HubInduction_Location_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "Configuration",
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HubInduction_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HubInduction_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HubInduction_User_EditorId",
                        column: x => x.EditorId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HubInduction_User_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WingInduction",
                schema: "Induction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    InductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    EditorId = table.Column<string>(type: "nvarchar(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WingInduction", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_WingInduction_Location_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "Configuration",
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WingInduction_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WingInduction_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WingInduction_User_EditorId",
                        column: x => x.EditorId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WingInduction_User_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WingInductionPhase",
                schema: "Induction",
                columns: table => new
                {
                    Number = table.Column<int>(type: "int", nullable: false),
                    WingInductionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WingInductionPhase", x => new { x.WingInductionId, x.Number });
                    table.ForeignKey(
                        name: "FK_WingInductionPhase_WingInduction_WingInductionId",
                        column: x => x.WingInductionId,
                        principalSchema: "Induction",
                        principalTable: "WingInduction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HubInduction_CreatedBy",
                schema: "Induction",
                table: "HubInduction",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_HubInduction_EditorId",
                schema: "Induction",
                table: "HubInduction",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_HubInduction_LocationId",
                schema: "Induction",
                table: "HubInduction",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_HubInduction_OwnerId",
                schema: "Induction",
                table: "HubInduction",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_HubInduction_ParticipantId_Created",
                schema: "Induction",
                table: "HubInduction",
                columns: new[] { "ParticipantId", "Created" })
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_WingInduction_CreatedBy",
                schema: "Induction",
                table: "WingInduction",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WingInduction_EditorId",
                schema: "Induction",
                table: "WingInduction",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_WingInduction_LocationId",
                schema: "Induction",
                table: "WingInduction",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_WingInduction_OwnerId",
                schema: "Induction",
                table: "WingInduction",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_WingInduction_ParticipantId_Created",
                schema: "Induction",
                table: "WingInduction",
                columns: new[] { "ParticipantId", "Created" })
                .Annotation("SqlServer:Clustered", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HubInduction",
                schema: "Induction");

            migrationBuilder.DropTable(
                name: "WingInductionPhase",
                schema: "Induction");

            migrationBuilder.DropTable(
                name: "WingInduction",
                schema: "Induction");
        }
    }
}
