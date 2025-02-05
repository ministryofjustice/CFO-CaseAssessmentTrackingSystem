using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PRI_ModifyToJustCompletedAndStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PRI_User_AbandonedBy",
                schema: "PRI",
                table: "PRI");

            migrationBuilder.DropColumn(
                name: "AbandonedOn",
                schema: "PRI",
                table: "PRI");

            migrationBuilder.RenameColumn(
                name: "AbandonedBy",
                schema: "PRI",
                table: "PRI",
                newName: "CompletedBy");

            migrationBuilder.RenameIndex(
                name: "IX_PRI_AbandonedBy",
                schema: "PRI",
                table: "PRI",
                newName: "IX_PRI_CompletedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_PRI_User_CompletedBy",
                schema: "PRI",
                table: "PRI",
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
                name: "FK_PRI_User_CompletedBy",
                schema: "PRI",
                table: "PRI");

            migrationBuilder.RenameColumn(
                name: "CompletedBy",
                schema: "PRI",
                table: "PRI",
                newName: "AbandonedBy");

            migrationBuilder.RenameIndex(
                name: "IX_PRI_CompletedBy",
                schema: "PRI",
                table: "PRI",
                newName: "IX_PRI_AbandonedBy");

            migrationBuilder.AddColumn<DateTime>(
                name: "AbandonedOn",
                schema: "PRI",
                table: "PRI",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PRI_User_AbandonedBy",
                schema: "PRI",
                table: "PRI",
                column: "AbandonedBy",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
