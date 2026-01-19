using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFeddbackTypeToQa2QueueNoteAndEscalationQueueNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FeedbackType",
                schema: "Activities",
                table: "Qa2QueueNote",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FeedbackType",
                schema: "Activities",
                table: "EscalationNote",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeedbackType",
                schema: "Activities",
                table: "Qa2QueueNote");

            migrationBuilder.DropColumn(
                name: "FeedbackType",
                schema: "Activities",
                table: "EscalationNote");
        }
    }
}
