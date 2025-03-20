using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class WingInductionPhase_AddAbandonReasonAndJustification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AbandonJustification",
                schema: "Induction",
                table: "WingInductionPhase",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AbandonReason",
                schema: "Induction",
                table: "WingInductionPhase",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompletedBy",
                schema: "Induction",
                table: "WingInductionPhase",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "Induction",
                table: "WingInductionPhase",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WingInductionPhase_CompletedBy",
                schema: "Induction",
                table: "WingInductionPhase",
                column: "CompletedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_WingInductionPhase_User_CompletedBy",
                schema: "Induction",
                table: "WingInductionPhase",
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
                name: "FK_WingInductionPhase_User_CompletedBy",
                schema: "Induction",
                table: "WingInductionPhase");

            migrationBuilder.DropIndex(
                name: "IX_WingInductionPhase_CompletedBy",
                schema: "Induction",
                table: "WingInductionPhase");

            migrationBuilder.DropColumn(
                name: "AbandonJustification",
                schema: "Induction",
                table: "WingInductionPhase");

            migrationBuilder.DropColumn(
                name: "AbandonReason",
                schema: "Induction",
                table: "WingInductionPhase");

            migrationBuilder.DropColumn(
                name: "CompletedBy",
                schema: "Induction",
                table: "WingInductionPhase");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Induction",
                table: "WingInductionPhase");
        }
    }
}
