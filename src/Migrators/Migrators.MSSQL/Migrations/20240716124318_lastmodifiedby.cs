﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class lastmodifiedby : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_User_ModifiedByUserId",
                schema: "Identity",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_ModifiedByUserId",
                schema: "Identity",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                schema: "Identity",
                table: "Note");

            migrationBuilder.CreateIndex(
                name: "IX_Note_LastModifiedBy",
                schema: "Identity",
                table: "Note",
                column: "LastModifiedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Note_User_LastModifiedBy",
                schema: "Identity",
                table: "Note",
                column: "LastModifiedBy",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_User_LastModifiedBy",
                schema: "Identity",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_LastModifiedBy",
                schema: "Identity",
                table: "Note");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByUserId",
                schema: "Identity",
                table: "Note",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Note_ModifiedByUserId",
                schema: "Identity",
                table: "Note",
                column: "ModifiedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Note_User_ModifiedByUserId",
                schema: "Identity",
                table: "Note",
                column: "ModifiedByUserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
