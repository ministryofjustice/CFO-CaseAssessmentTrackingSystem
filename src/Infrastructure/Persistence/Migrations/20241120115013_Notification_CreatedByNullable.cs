using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Notification_CreatedByNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "Notification",
                type: "nvarchar(36)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldMaxLength: 36);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_CreatedBy",
                schema: "Identity",
                table: "Notification",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_User_CreatedBy",
                schema: "Identity",
                table: "Notification",
                column: "CreatedBy",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_User_CreatedBy",
                schema: "Identity",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_CreatedBy",
                schema: "Identity",
                table: "Notification");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "Notification",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldNullable: true);
        }
    }
}
