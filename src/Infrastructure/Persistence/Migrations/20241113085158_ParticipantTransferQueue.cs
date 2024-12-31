using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ParticipantTransferQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncomingTransferQueue",
                schema: "Participant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoveOccured = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    FromContractId = table.Column<string>(type: "nvarchar(12)", nullable: true),
                    ToContractId = table.Column<string>(type: "nvarchar(12)", nullable: true),
                    FromLocationId = table.Column<int>(type: "int", nullable: false),
                    ToLocationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingTransferQueue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomingTransferQueue_Contract_FromContractId",
                        column: x => x.FromContractId,
                        principalSchema: "Configuration",
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncomingTransferQueue_Contract_ToContractId",
                        column: x => x.ToContractId,
                        principalSchema: "Configuration",
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncomingTransferQueue_Location_FromLocationId",
                        column: x => x.FromLocationId,
                        principalSchema: "Configuration",
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncomingTransferQueue_Location_ToLocationId",
                        column: x => x.ToLocationId,
                        principalSchema: "Configuration",
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutgoingTransferQueue",
                schema: "Participant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoveOccured = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    FromContractId = table.Column<string>(type: "nvarchar(12)", nullable: true),
                    ToContractId = table.Column<string>(type: "nvarchar(12)", nullable: true),
                    FromLocationId = table.Column<int>(type: "int", nullable: false),
                    ToLocationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingTransferQueue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutgoingTransferQueue_Contract_FromContractId",
                        column: x => x.FromContractId,
                        principalSchema: "Configuration",
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutgoingTransferQueue_Contract_ToContractId",
                        column: x => x.ToContractId,
                        principalSchema: "Configuration",
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutgoingTransferQueue_Location_FromLocationId",
                        column: x => x.FromLocationId,
                        principalSchema: "Configuration",
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutgoingTransferQueue_Location_ToLocationId",
                        column: x => x.ToLocationId,
                        principalSchema: "Configuration",
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncomingTransferQueue_Completed",
                schema: "Participant",
                table: "IncomingTransferQueue",
                column: "Completed");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingTransferQueue_FromContractId",
                schema: "Participant",
                table: "IncomingTransferQueue",
                column: "FromContractId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingTransferQueue_FromLocationId",
                schema: "Participant",
                table: "IncomingTransferQueue",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingTransferQueue_ToContractId",
                schema: "Participant",
                table: "IncomingTransferQueue",
                column: "ToContractId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingTransferQueue_ToLocationId",
                schema: "Participant",
                table: "IncomingTransferQueue",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingTransferQueue_FromContractId",
                schema: "Participant",
                table: "OutgoingTransferQueue",
                column: "FromContractId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingTransferQueue_FromLocationId",
                schema: "Participant",
                table: "OutgoingTransferQueue",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingTransferQueue_ToContractId",
                schema: "Participant",
                table: "OutgoingTransferQueue",
                column: "ToContractId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingTransferQueue_ToLocationId",
                schema: "Participant",
                table: "OutgoingTransferQueue",
                column: "ToLocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomingTransferQueue",
                schema: "Participant");

            migrationBuilder.DropTable(
                name: "OutgoingTransferQueue",
                schema: "Participant");
        }
    }
}
