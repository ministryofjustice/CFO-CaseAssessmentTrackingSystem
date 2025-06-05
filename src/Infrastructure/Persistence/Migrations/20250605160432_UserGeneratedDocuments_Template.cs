using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserGeneratedDocuments_Template : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SearchCriteriaUsed",
                schema: "Document",
                table: "GeneratedDocument",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Template",
                schema: "Document",
                table: "GeneratedDocument",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedDocument_ExpiresOn",
                schema: "Document",
                table: "GeneratedDocument",
                column: "ExpiresOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GeneratedDocument_ExpiresOn",
                schema: "Document",
                table: "GeneratedDocument");

            migrationBuilder.DropColumn(
                name: "SearchCriteriaUsed",
                schema: "Document",
                table: "GeneratedDocument");

            migrationBuilder.DropColumn(
                name: "Template",
                schema: "Document",
                table: "GeneratedDocument");
        }
    }
}
