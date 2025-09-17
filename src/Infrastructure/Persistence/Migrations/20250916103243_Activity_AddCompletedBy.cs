using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Activity_AddCompletedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompletedBy",
                schema: "Activities",
                table: "Activity",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedBy",
                schema: "Activities",
                table: "Activity");
        }
    }
}
