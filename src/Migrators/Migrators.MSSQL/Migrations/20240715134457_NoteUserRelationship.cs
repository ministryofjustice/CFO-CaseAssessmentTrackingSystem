using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class NoteUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ParticipantNote",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ApplicationUserNote",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantNote_CreatedBy",
                table: "ParticipantNote",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserNote_CreatedBy",
                table: "ApplicationUserNote",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserNote_ApplicationUser_CreatedBy",
                table: "ApplicationUserNote",
                column: "CreatedBy",
                principalTable: "ApplicationUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantNote_ApplicationUser_CreatedBy",
                table: "ParticipantNote",
                column: "CreatedBy",
                principalTable: "ApplicationUser",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserNote_ApplicationUser_CreatedBy",
                table: "ApplicationUserNote");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantNote_ApplicationUser_CreatedBy",
                table: "ParticipantNote");

            migrationBuilder.DropIndex(
                name: "IX_ParticipantNote_CreatedBy",
                table: "ParticipantNote");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserNote_CreatedBy",
                table: "ApplicationUserNote");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ParticipantNote",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ApplicationUserNote",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
