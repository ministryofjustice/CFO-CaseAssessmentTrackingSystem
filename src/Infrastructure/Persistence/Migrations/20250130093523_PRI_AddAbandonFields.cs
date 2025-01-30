using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PRI_AddAbandonFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                schema: "PRI",
                table: "PRI");

            migrationBuilder.AddColumn<string>(
                name: "AbandonedBy",
                schema: "PRI",
                table: "PRI",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AbandonedOn",
                schema: "PRI",
                table: "PRI",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonAbandoned",
                schema: "PRI",
                table: "PRI",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "PRI",
                table: "PRI",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PRI_AbandonedBy",
                schema: "PRI",
                table: "PRI",
                column: "AbandonedBy");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PRI_User_AbandonedBy",
                schema: "PRI",
                table: "PRI");

            migrationBuilder.DropIndex(
                name: "IX_PRI_AbandonedBy",
                schema: "PRI",
                table: "PRI");

            migrationBuilder.DropColumn(
                name: "AbandonedBy",
                schema: "PRI",
                table: "PRI");

            migrationBuilder.DropColumn(
                name: "AbandonedOn",
                schema: "PRI",
                table: "PRI");

            migrationBuilder.DropColumn(
                name: "ReasonAbandoned",
                schema: "PRI",
                table: "PRI");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "PRI",
                table: "PRI");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                schema: "PRI",
                table: "PRI",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
