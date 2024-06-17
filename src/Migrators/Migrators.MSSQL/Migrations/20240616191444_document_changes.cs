using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class document_changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Document");

            migrationBuilder.RenameColumn(
                name: "DeletedBy",
                table: "Document",
                newName: "URL");

            migrationBuilder.AddColumn<string>(
                name: "EditorId",
                table: "Participant",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "Document",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Document",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Document",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Document",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentType",
                table: "Document",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EditorId",
                table: "Document",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Document",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Document",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Document",
                type: "nvarchar(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Document",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participant_EditorId",
                table: "Participant",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_Document_CreatedBy",
                table: "Document",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Document_LastModifiedBy",
                table: "Document",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Document_TenantId",
                table: "Document",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Document_ApplicationUser_CreatedBy",
                table: "Document",
                column: "CreatedBy",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Document_ApplicationUser_LastModifiedBy",
                table: "Document",
                column: "LastModifiedBy",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Document_Tenant_TenantId",
                table: "Document",
                column: "TenantId",
                principalTable: "Tenant",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_ApplicationUser_EditorId",
                table: "Participant",
                column: "EditorId",
                principalTable: "ApplicationUser",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Document_ApplicationUser_CreatedBy",
                table: "Document");

            migrationBuilder.DropForeignKey(
                name: "FK_Document_ApplicationUser_LastModifiedBy",
                table: "Document");

            migrationBuilder.DropForeignKey(
                name: "FK_Document_Tenant_TenantId",
                table: "Document");

            migrationBuilder.DropForeignKey(
                name: "FK_Participant_ApplicationUser_EditorId",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_Participant_EditorId",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_Document_CreatedBy",
                table: "Document");

            migrationBuilder.DropIndex(
                name: "IX_Document_LastModifiedBy",
                table: "Document");

            migrationBuilder.DropIndex(
                name: "IX_Document_TenantId",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "EditorId",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "DocumentType",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "EditorId",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Document");

            migrationBuilder.RenameColumn(
                name: "URL",
                table: "Document",
                newName: "DeletedBy");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "Document",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Document",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Document",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Deleted",
                table: "Document",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Document",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }
    }
}
