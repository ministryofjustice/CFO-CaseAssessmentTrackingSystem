using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentIdToQualityDipCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeQualityDipSample_DocumentId",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutcomeQualityDipSample_Document_DocumentId",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                column: "DocumentId",
                principalSchema: "Document",
                principalTable: "Document",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutcomeQualityDipSample_Document_DocumentId",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropIndex(
                name: "IX_OutcomeQualityDipSample_DocumentId",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                schema: "Mi",
                table: "OutcomeQualityDipSample");
        }
    }
}
