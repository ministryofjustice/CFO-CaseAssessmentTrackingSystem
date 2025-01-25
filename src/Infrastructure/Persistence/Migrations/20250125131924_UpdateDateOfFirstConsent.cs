using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDateOfFirstConsent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update p set p.DateOfFirstConsent = Consent.ValidFrom
                                   FROM
                                   (select ParticipantId, Min(Created) as [EarliestApproval] from participant.EnrolmentHistory
                                    where enrolmentstatus = 3
                                    group by ParticipantId
                                   ) as [Approved]
                                   CROSS APPLY 
                                   (
	                                  SELECT TOP(1) * FROM 
	                                   Participant.Consent
	                                  WHERE consent.ParticipantId = [Approved].ParticipantId
		                              AND Consent.Created < [Approved].EarliestApproval
	                                  ORDER BY Created Desc
                                   ) as Consent
                                   INNER JOIN 
                                    participant.participant as p 
                                    on consent.ParticipantId = p.Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update [Participant].[Participant] SET DateOfFirstConsent=NULL;");
        }
    }
}
