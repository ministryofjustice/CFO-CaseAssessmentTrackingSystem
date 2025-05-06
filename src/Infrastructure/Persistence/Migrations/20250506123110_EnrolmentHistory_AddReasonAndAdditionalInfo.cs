using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnrolmentHistory_AddReasonAndAdditionalInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalInformation",
                schema: "Participant",
                table: "EnrolmentHistory",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                schema: "Participant",
                table: "EnrolmentHistory",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalInformation",
                schema: "Participant",
                table: "EnrolmentHistory");

            migrationBuilder.DropColumn(
                name: "Reason",
                schema: "Participant",
                table: "EnrolmentHistory");
        }
    }
}
