using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class WingInductionPhase_AddIdForDuplicatePhase1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WingInductionPhase",
                schema: "Induction",
                table: "WingInductionPhase");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "Induction",
                table: "WingInductionPhase",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_WingInductionPhase",
                schema: "Induction",
                table: "WingInductionPhase",
                columns: new[] { "WingInductionId", "Number", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WingInductionPhase",
                schema: "Induction",
                table: "WingInductionPhase");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "Induction",
                table: "WingInductionPhase");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WingInductionPhase",
                schema: "Induction",
                table: "WingInductionPhase",
                columns: new[] { "WingInductionId", "Number" });
        }
    }
}
