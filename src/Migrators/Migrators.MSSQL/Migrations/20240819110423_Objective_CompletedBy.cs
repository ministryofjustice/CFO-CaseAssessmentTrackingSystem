using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class Objective_CompletedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompletedBy",
                schema: "Participant",
                table: "Objective",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Objective_CompletedBy",
                schema: "Participant",
                table: "Objective",
                column: "CompletedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Objective_User_CompletedBy",
                schema: "Participant",
                table: "Objective",
                column: "CompletedBy",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objective_User_CompletedBy",
                schema: "Participant",
                table: "Objective");

            migrationBuilder.DropIndex(
                name: "IX_Objective_CompletedBy",
                schema: "Participant",
                table: "Objective");

            migrationBuilder.DropColumn(
                name: "CompletedBy",
                schema: "Participant",
                table: "Objective");
        }
    }
}
