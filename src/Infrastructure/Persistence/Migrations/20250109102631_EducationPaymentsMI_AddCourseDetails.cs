using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EducationPaymentsMI_AddCourseDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourseLevel",
                schema: "Mi",
                table: "EducationPayment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseTitle",
                schema: "Mi",
                table: "EducationPayment",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseLevel",
                schema: "Mi",
                table: "EducationPayment");

            migrationBuilder.DropColumn(
                name: "CourseTitle",
                schema: "Mi",
                table: "EducationPayment");
        }
    }
}
