using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddParticipantLabels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Label",
                schema: "Participant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LifetimeStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LifetimeEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Label", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Label_Label_LabelId",
                        column: x => x.LabelId,
                        principalSchema: "Configuration",
                        principalTable: "Label",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Label_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "Participant",
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Label_LabelId",
                schema: "Participant",
                table: "Label",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_Label_ParticipantId",
                schema: "Participant",
                table: "Label",
                column: "ParticipantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Label",
                schema: "Participant");
        }
    }
}
