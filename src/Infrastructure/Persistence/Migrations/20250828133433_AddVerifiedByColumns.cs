using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVerifiedByColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerifiedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedOn",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeQualityDipSample_VerifiedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                column: "VerifiedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_OutcomeQualityDipSample_User_VerifiedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                column: "VerifiedBy",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutcomeQualityDipSample_User_VerifiedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropIndex(
                name: "IX_OutcomeQualityDipSample_VerifiedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropColumn(
                name: "VerifiedOn",
                schema: "Mi",
                table: "OutcomeQualityDipSample");
        }
    }
}
