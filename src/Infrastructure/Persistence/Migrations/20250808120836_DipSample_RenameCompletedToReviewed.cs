using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DipSample_RenameCompletedToReviewed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutcomeQualityDipSample_User_CompletedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.RenameColumn(
                name: "CompletedOn",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                newName: "ReviewedOn");

            migrationBuilder.RenameColumn(
                name: "CompletedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                newName: "ReviewedBy");

            migrationBuilder.RenameIndex(
                name: "IX_OutcomeQualityDipSample_CompletedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                newName: "IX_OutcomeQualityDipSample_ReviewedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_OutcomeQualityDipSample_User_ReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                column: "ReviewedBy",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutcomeQualityDipSample_User_ReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.RenameColumn(
                name: "ReviewedOn",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                newName: "CompletedOn");

            migrationBuilder.RenameColumn(
                name: "ReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                newName: "CompletedBy");

            migrationBuilder.RenameIndex(
                name: "IX_OutcomeQualityDipSample_ReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                newName: "IX_OutcomeQualityDipSample_CompletedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_OutcomeQualityDipSample_User_CompletedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                column: "CompletedBy",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
