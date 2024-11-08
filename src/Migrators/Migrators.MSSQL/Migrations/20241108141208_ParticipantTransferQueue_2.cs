using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class ParticipantTransferQueue_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromContractId",
                schema: "Participant",
                table: "OutgoingTransferQueue",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToContractId",
                schema: "Participant",
                table: "OutgoingTransferQueue",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromContractId",
                schema: "Participant",
                table: "IncomingTransferQueue",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToContractId",
                schema: "Participant",
                table: "IncomingTransferQueue",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromContractId",
                schema: "Participant",
                table: "OutgoingTransferQueue");

            migrationBuilder.DropColumn(
                name: "ToContractId",
                schema: "Participant",
                table: "OutgoingTransferQueue");

            migrationBuilder.DropColumn(
                name: "FromContractId",
                schema: "Participant",
                table: "IncomingTransferQueue");

            migrationBuilder.DropColumn(
                name: "ToContractId",
                schema: "Participant",
                table: "IncomingTransferQueue");
        }
    }
}
