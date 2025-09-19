using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Participant_ActiveStatusHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActiveStatusHistory",
                schema: "Participant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    From = table.Column<bool>(type: "bit", nullable: false),
                    To = table.Column<bool>(type: "bit", nullable: false),
                    OccurredOn = table.Column<DateOnly>(type: "date", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveStatusHistory_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveStatusHistory_ParticipantId",
                schema: "Participant",
                table: "ActiveStatusHistory",
                column: "ParticipantId");

            migrationBuilder.Sql(@"
                INSERT INTO Participant.ActiveStatusHistory ([Id], [From], [To], [OccurredOn], [ParticipantId], [Created]) 
                SELECT NEWID(), 1, 0, DeactivatedInFeed, Id, GETUTCDATE() 
                FROM Participant.Participant 
                WHERE DeactivatedInFeed IS NOT NULL
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveStatusHistory",
                schema: "Participant");
        }
    }
}
