using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DipSampleScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScoreAvg",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.AddColumn<int>(
                name: "CpmCompliant",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CpmPercentage",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CpmScore",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CsoCompliant",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CsoPercentage",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CsoScore",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinalCompliant",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinalPercentage",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinalScore",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Size",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CpmCompliant",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropColumn(
                name: "CpmPercentage",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropColumn(
                name: "CpmScore",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropColumn(
                name: "CsoCompliant",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropColumn(
                name: "CsoPercentage",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropColumn(
                name: "CsoScore",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropColumn(
                name: "FinalCompliant",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropColumn(
                name: "FinalPercentage",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropColumn(
                name: "FinalScore",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropColumn(
                name: "LastModified",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.DropColumn(
                name: "Size",
                schema: "Mi",
                table: "OutcomeQualityDipSample");

            migrationBuilder.AddColumn<double>(
                name: "ScoreAvg",
                schema: "Mi",
                table: "OutcomeQualityDipSample",
                type: "float",
                nullable: true);
        }
    }
}
