using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class UserRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Provider",
                table: "ApplicationUser",
                newName: "ProviderId");

            migrationBuilder.AddColumn<string>(
                name: "MemorableDate",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MemorablePlace",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemorableDate",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "MemorablePlace",
                table: "ApplicationUser");

            migrationBuilder.RenameColumn(
                name: "ProviderId",
                table: "ApplicationUser",
                newName: "Provider");
        }
    }
}
