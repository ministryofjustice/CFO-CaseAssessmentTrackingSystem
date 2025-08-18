using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Tasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Objective_CreatedBy",
                schema: "Participant",
                table: "Objective",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Objective_User_CreatedBy",
                schema: "Participant",
                table: "Objective",
                column: "CreatedBy",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objective_User_CreatedBy",
                schema: "Participant",
                table: "Objective");

            migrationBuilder.DropIndex(
                name: "IX_Objective_CreatedBy",
                schema: "Participant",
                table: "Objective");
        }
    }
}
