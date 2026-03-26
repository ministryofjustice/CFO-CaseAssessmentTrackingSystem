using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ActivityFeedback_AddExtraColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivityCategory",
                schema: "Activities",
                table: "ActivityFeedback",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ActivityFeedbackReason",
                schema: "Activities",
                table: "ActivityFeedback",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ActivityType",
                schema: "Activities",
                table: "ActivityFeedback",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                                 DECLARE @Created DATETIME = GETUTCDATE();
                                 DECLARE @CreatedUserId NVARCHAR(50) = '2a9b3450-1feb-4be3-ab94-24e64cd34829'; -- adjust as needed
                                 
                                 IF NOT EXISTS (SELECT 1 FROM Configuration.KeyValue WHERE Name='QaFeedbackReason' AND Value='Ineligible Claim')
                                     INSERT INTO Configuration.KeyValue (Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy)
                                     VALUES ('QaFeedbackReason','Ineligible Claim','Ineligible Claim','A QA Feedback reason',@Created,@CreatedUserId,null,null);
                                 
                                 IF NOT EXISTS (SELECT 1 FROM Configuration.KeyValue WHERE Name='QaFeedbackReason' AND Value='Incorrect / Missing Paperwork')
                                     INSERT INTO Configuration.KeyValue (Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy)
                                     VALUES ('QaFeedbackReason','Incorrect / Missing Paperwork','Incorrect / Missing Paperwork','A QA Feedback reason',@Created,@CreatedUserId,null,null);
                                 
                                 IF NOT EXISTS (SELECT 1 FROM Configuration.KeyValue WHERE Name='QaFeedbackReason' AND Value='Information incomplete')
                                     INSERT INTO Configuration.KeyValue (Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy)
                                     VALUES ('QaFeedbackReason','Information incomplete','Information incomplete','A QA Feedback reason',@Created,@CreatedUserId,null,null);
                                 
                                 IF NOT EXISTS (SELECT 1 FROM Configuration.KeyValue WHERE Name='QaFeedbackReason' AND Value='Information conflicts with CATS')
                                     INSERT INTO Configuration.KeyValue (Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy)
                                     VALUES ('QaFeedbackReason','Information conflicts with CATS','Information conflicts with CATS','A QA Feedback reason',@Created,@CreatedUserId,null,null);
                                 
                                 IF NOT EXISTS (SELECT 1 FROM Configuration.KeyValue WHERE Name='QaFeedbackReason' AND Value='Other')
                                     INSERT INTO Configuration.KeyValue (Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy)
                                     VALUES ('QaFeedbackReason','Other','Other','A QA Feedback reason',@Created,@CreatedUserId,null,null);
                                 
                                 IF NOT EXISTS (SELECT 1 FROM Configuration.KeyValue WHERE Name='QaFeedbackReason' AND Value='Positive Recognition')
                                     INSERT INTO Configuration.KeyValue (Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy)
                                     VALUES ('QaFeedbackReason','Positive Recognition','Positive Recognition','A QA Feedback reason',@Created,@CreatedUserId,null,null);
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityCategory",
                schema: "Activities",
                table: "ActivityFeedback");

            migrationBuilder.DropColumn(
                name: "ActivityFeedbackReason",
                schema: "Activities",
                table: "ActivityFeedback");

            migrationBuilder.DropColumn(
                name: "ActivityType",
                schema: "Activities",
                table: "ActivityFeedback");
        }
    }
}