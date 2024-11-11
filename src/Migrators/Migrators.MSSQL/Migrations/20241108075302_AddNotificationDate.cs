using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "clst_notification",
                schema: "Identity",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_OwnerId_Created_ReadDate",
                schema: "Identity",
                table: "Notification");

            migrationBuilder.AddColumn<DateTime>(
                name: "NotificationDate",
                schema: "Identity",
                table: "Notification",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "clst_notification",
                schema: "Identity",
                table: "Notification",
                column: "NotificationDate")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_OwnerId_NotificationDate_ReadDate",
                schema: "Identity",
                table: "Notification",
                columns: new[] { "OwnerId", "NotificationDate", "ReadDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "clst_notification",
                schema: "Identity",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_OwnerId_NotificationDate_ReadDate",
                schema: "Identity",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "NotificationDate",
                schema: "Identity",
                table: "Notification");

            migrationBuilder.CreateIndex(
                name: "clst_notification",
                schema: "Identity",
                table: "Notification",
                column: "Created")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_OwnerId_Created_ReadDate",
                schema: "Identity",
                table: "Notification",
                columns: new[] { "OwnerId", "Created", "ReadDate" });
        }
    }
}
