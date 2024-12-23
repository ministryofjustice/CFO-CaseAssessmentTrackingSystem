using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class MovePayablesToActivitiesMissed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentActivities_Activity_Id",
                schema: "Activities",
                table: "EmploymentActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentActivities_Document_DocumentId",
                schema: "Activities",
                table: "EmploymentActivities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmploymentActivities",
                schema: "Activities",
                table: "EmploymentActivities");

            migrationBuilder.RenameTable(
                name: "EmploymentActivities",
                schema: "Activities",
                newName: "EmploymentActivity",
                newSchema: "Activities");

            migrationBuilder.RenameIndex(
                name: "IX_EmploymentActivities_DocumentId",
                schema: "Activities",
                table: "EmploymentActivity",
                newName: "IX_EmploymentActivity_DocumentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmploymentActivity",
                schema: "Activities",
                table: "EmploymentActivity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentActivity_Activity_Id",
                schema: "Activities",
                table: "EmploymentActivity",
                column: "Id",
                principalSchema: "Activities",
                principalTable: "Activity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentActivity_Document_DocumentId",
                schema: "Activities",
                table: "EmploymentActivity",
                column: "DocumentId",
                principalSchema: "Document",
                principalTable: "Document",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentActivity_Activity_Id",
                schema: "Activities",
                table: "EmploymentActivity");

            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentActivity_Document_DocumentId",
                schema: "Activities",
                table: "EmploymentActivity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmploymentActivity",
                schema: "Activities",
                table: "EmploymentActivity");

            migrationBuilder.RenameTable(
                name: "EmploymentActivity",
                schema: "Activities",
                newName: "EmploymentActivities",
                newSchema: "Activities");

            migrationBuilder.RenameIndex(
                name: "IX_EmploymentActivity_DocumentId",
                schema: "Activities",
                table: "EmploymentActivities",
                newName: "IX_EmploymentActivities_DocumentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmploymentActivities",
                schema: "Activities",
                table: "EmploymentActivities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentActivities_Activity_Id",
                schema: "Activities",
                table: "EmploymentActivities",
                column: "Id",
                principalSchema: "Activities",
                principalTable: "Activity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentActivities_Document_DocumentId",
                schema: "Activities",
                table: "EmploymentActivities",
                column: "DocumentId",
                principalSchema: "Document",
                principalTable: "Document",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
