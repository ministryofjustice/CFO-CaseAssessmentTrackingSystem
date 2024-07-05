using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class IAuditable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "TenantDomain",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TenantDomain",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "TenantDomain",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "TenantDomain",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ApplicationUser",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "ApplicationUser",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "TenantDomain");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TenantDomain");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "TenantDomain");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "TenantDomain");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ApplicationUser");
        }
    }
}
