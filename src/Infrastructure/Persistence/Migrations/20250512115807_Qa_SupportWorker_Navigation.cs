using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Qa_SupportWorker_Navigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Qa2Queue_SupportWorkerId",
                schema: "Enrolment",
                table: "Qa2Queue",
                column: "SupportWorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_Qa1Queue_SupportWorkerId",
                schema: "Enrolment",
                table: "Qa1Queue",
                column: "SupportWorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_PqaQueue_SupportWorkerId",
                schema: "Enrolment",
                table: "PqaQueue",
                column: "SupportWorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationQueue_SupportWorkerId",
                schema: "Enrolment",
                table: "EscalationQueue",
                column: "SupportWorkerId");

            migrationBuilder.AddForeignKey(
                name: "FK_EscalationQueue_User_SupportWorkerId",
                schema: "Enrolment",
                table: "EscalationQueue",
                column: "SupportWorkerId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PqaQueue_User_SupportWorkerId",
                schema: "Enrolment",
                table: "PqaQueue",
                column: "SupportWorkerId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Qa1Queue_User_SupportWorkerId",
                schema: "Enrolment",
                table: "Qa1Queue",
                column: "SupportWorkerId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Qa2Queue_User_SupportWorkerId",
                schema: "Enrolment",
                table: "Qa2Queue",
                column: "SupportWorkerId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EscalationQueue_User_SupportWorkerId",
                schema: "Enrolment",
                table: "EscalationQueue");

            migrationBuilder.DropForeignKey(
                name: "FK_PqaQueue_User_SupportWorkerId",
                schema: "Enrolment",
                table: "PqaQueue");

            migrationBuilder.DropForeignKey(
                name: "FK_Qa1Queue_User_SupportWorkerId",
                schema: "Enrolment",
                table: "Qa1Queue");

            migrationBuilder.DropForeignKey(
                name: "FK_Qa2Queue_User_SupportWorkerId",
                schema: "Enrolment",
                table: "Qa2Queue");

            migrationBuilder.DropIndex(
                name: "IX_Qa2Queue_SupportWorkerId",
                schema: "Enrolment",
                table: "Qa2Queue");

            migrationBuilder.DropIndex(
                name: "IX_Qa1Queue_SupportWorkerId",
                schema: "Enrolment",
                table: "Qa1Queue");

            migrationBuilder.DropIndex(
                name: "IX_PqaQueue_SupportWorkerId",
                schema: "Enrolment",
                table: "PqaQueue");

            migrationBuilder.DropIndex(
                name: "IX_EscalationQueue_SupportWorkerId",
                schema: "Enrolment",
                table: "EscalationQueue");
        }
    }
}
