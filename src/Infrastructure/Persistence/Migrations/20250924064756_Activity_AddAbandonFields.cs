using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Activity_AddAbandonFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApprovedOn",
                schema: "Activities",
                table: "Activity",
                newName: "CompletedOn");

            migrationBuilder.AddColumn<string>(
                name: "AbandonJustification",
                schema: "Activities",
                table: "Activity",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AbandonReason",
                schema: "Activities",
                table: "Activity",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activity_CompletedBy",
                schema: "Activities",
                table: "Activity",
                column: "CompletedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_User_CompletedBy",
                schema: "Activities",
                table: "Activity",
                column: "CompletedBy",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activity_User_CompletedBy",
                schema: "Activities",
                table: "Activity");

            migrationBuilder.DropIndex(
                name: "IX_Activity_CompletedBy",
                schema: "Activities",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "AbandonJustification",
                schema: "Activities",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "AbandonReason",
                schema: "Activities",
                table: "Activity");

            migrationBuilder.RenameColumn(
                name: "CompletedOn",
                schema: "Activities",
                table: "Activity",
                newName: "ApprovedOn");
        }
    }
}
