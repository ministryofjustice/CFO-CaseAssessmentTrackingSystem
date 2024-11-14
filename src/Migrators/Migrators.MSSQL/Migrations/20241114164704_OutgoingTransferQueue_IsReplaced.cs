using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class OutgoingTransferQueue_IsReplaced : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReplaced",
                schema: "Participant",
                table: "OutgoingTransferQueue",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingTransferQueue_IsReplaced",
                schema: "Participant",
                table: "OutgoingTransferQueue",
                column: "IsReplaced");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutgoingTransferQueue_IsReplaced",
                schema: "Participant",
                table: "OutgoingTransferQueue");

            migrationBuilder.DropColumn(
                name: "IsReplaced",
                schema: "Participant",
                table: "OutgoingTransferQueue");
        }
    }
}
