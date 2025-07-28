using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DipSampleParticipantChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutcomeQualityDipSampleParticipant_User_ReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutcomeQualityDipSampleParticipant",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropIndex(
                name: "IX_OutcomeQualityDipSampleParticipant_IsCompliant",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropColumn(
                name: "IsCompliant",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.RenameColumn(
                name: "ReviewedOn",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                newName: "LastModified");

            migrationBuilder.RenameColumn(
                name: "ReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                newName: "FinalReviewedBy");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                newName: "CsoComments");

            migrationBuilder.RenameIndex(
                name: "IX_OutcomeQualityDipSampleParticipant_ReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                newName: "IX_OutcomeQualityDipSampleParticipant_FinalReviewedBy");

            migrationBuilder.AlterColumn<int>(
                name: "TemplatesAlignWithREG",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TTGDemonstratesGoodPRIProcess",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SupportsJourneyAndAlignsWithDoS",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ShowsTaskProgression",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HasClearParticipantJourney",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ActivitiesLinkToTasks",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "CpmComments",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CpmIsCompliant",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CpmReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CpmReviewedOn",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CsoIsCompliant",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CsoReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CsoReviewedOn",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinalComments",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinalIsCompliant",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinalReviewedOn",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutcomeQualityDipSampleParticipant",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeQualityDipSampleParticipant_CpmReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                column: "CpmReviewedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeQualityDipSampleParticipant_CsoReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                column: "CsoReviewedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeQualityDipSampleParticipant_DipSampleId_ParticipantId",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                columns: new[] { "DipSampleId", "ParticipantId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OutcomeQualityDipSampleParticipant_User_CpmReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                column: "CpmReviewedBy",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OutcomeQualityDipSampleParticipant_User_CsoReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                column: "CsoReviewedBy",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OutcomeQualityDipSampleParticipant_User_FinalReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                column: "FinalReviewedBy",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutcomeQualityDipSampleParticipant_User_CpmReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_OutcomeQualityDipSampleParticipant_User_CsoReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_OutcomeQualityDipSampleParticipant_User_FinalReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutcomeQualityDipSampleParticipant",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropIndex(
                name: "IX_OutcomeQualityDipSampleParticipant_CpmReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropIndex(
                name: "IX_OutcomeQualityDipSampleParticipant_CsoReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropIndex(
                name: "IX_OutcomeQualityDipSampleParticipant_DipSampleId_ParticipantId",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropColumn(
                name: "CpmComments",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropColumn(
                name: "CpmIsCompliant",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropColumn(
                name: "CpmReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropColumn(
                name: "CpmReviewedOn",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropColumn(
                name: "CsoIsCompliant",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropColumn(
                name: "CsoReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropColumn(
                name: "CsoReviewedOn",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropColumn(
                name: "FinalComments",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropColumn(
                name: "FinalIsCompliant",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropColumn(
                name: "FinalReviewedOn",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant");

            migrationBuilder.RenameColumn(
                name: "LastModified",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                newName: "ReviewedOn");

            migrationBuilder.RenameColumn(
                name: "FinalReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                newName: "ReviewedBy");

            migrationBuilder.RenameColumn(
                name: "CsoComments",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                newName: "Remarks");

            migrationBuilder.RenameIndex(
                name: "IX_OutcomeQualityDipSampleParticipant_FinalReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                newName: "IX_OutcomeQualityDipSampleParticipant_ReviewedBy");

            migrationBuilder.AlterColumn<bool>(
                name: "TemplatesAlignWithREG",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "bit",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "TTGDemonstratesGoodPRIProcess",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "bit",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "SupportsJourneyAndAlignsWithDoS",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "bit",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "ShowsTaskProgression",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "bit",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "HasClearParticipantJourney",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "bit",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "ActivitiesLinkToTasks",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "bit",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompliant",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                type: "bit",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutcomeQualityDipSampleParticipant",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                columns: new[] { "DipSampleId", "ParticipantId" });

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeQualityDipSampleParticipant_IsCompliant",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                column: "IsCompliant");

            migrationBuilder.AddForeignKey(
                name: "FK_OutcomeQualityDipSampleParticipant_User_ReviewedBy",
                schema: "Mi",
                table: "OutcomeQualityDipSampleParticipant",
                column: "ReviewedBy",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
