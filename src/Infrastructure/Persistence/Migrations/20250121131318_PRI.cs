using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PRI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PRI");

            migrationBuilder.CreateTable(
                name: "PRI",
                schema: "PRI",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", nullable: false),
                    ExpectedReleaseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ActualReleaseDate = table.Column<DateOnly>(type: "date", nullable: true),
                    AcceptedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpectedReleaseRegionId = table.Column<int>(type: "int", nullable: false),
                    AssignedTo = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    MeetingAttendedOn = table.Column<DateOnly>(type: "date", nullable: false),
                    MeetingAttendedInPerson = table.Column<bool>(type: "bit", nullable: false),
                    MeetingNotAttendedInPersonJustification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PRI_Location_ExpectedReleaseRegionId",
                        column: x => x.ExpectedReleaseRegionId,
                        principalSchema: "Configuration",
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PRI_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PRI_User_AssignedTo",
                        column: x => x.AssignedTo,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PRI_AssignedTo",
                schema: "PRI",
                table: "PRI",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_PRI_ExpectedReleaseRegionId",
                schema: "PRI",
                table: "PRI",
                column: "ExpectedReleaseRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_PRI_ParticipantId",
                schema: "PRI",
                table: "PRI",
                column: "ParticipantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PRI",
                schema: "PRI");
        }
    }
}
