using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class Activity_BaseAuditableAndParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EditorId",
                schema: "Payables",
                table: "Activities",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                schema: "Payables",
                table: "Activities",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_EditorId",
                schema: "Payables",
                table: "Activities",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_OwnerId",
                schema: "Payables",
                table: "Activities",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_User_EditorId",
                schema: "Payables",
                table: "Activities",
                column: "EditorId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_User_OwnerId",
                schema: "Payables",
                table: "Activities",
                column: "OwnerId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_User_EditorId",
                schema: "Payables",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_User_OwnerId",
                schema: "Payables",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_EditorId",
                schema: "Payables",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_OwnerId",
                schema: "Payables",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "EditorId",
                schema: "Payables",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                schema: "Payables",
                table: "Activities");
        }
    }
}
