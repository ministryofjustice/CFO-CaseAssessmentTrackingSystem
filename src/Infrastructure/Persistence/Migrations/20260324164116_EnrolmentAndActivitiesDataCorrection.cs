using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnrolmentAndActivitiesDataCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE pfe
                SET 
                    ProviderQaUserId = q.ProviderQaUserId,
                    CfoUserId = q.CfoUserId,
                    PqaSubmittedDate = q.PqaSubmittedDate,
                    ActionDate = q.ActionDate
                FROM 
                    [Mi].ProviderFeedbackEnrolment pfe
                    INNER JOIN 
                    (
                        SELECT 
                            q.ParticipantId,                
                            e2.CreatedBy as ProviderQaUserId,
                            qn.CreatedBy as CfoUserId,
                            e2.Created as PqaSubmittedDate,
                            qn.Created as ActionDate,
                            q.LastModified as QueueLastModified,
                            qn.EnrolmentQa2QueueEntryId as Qa2QueueEntryId,
                            qn.Id as NoteId
                        FROM [Enrolment].[Qa2Queue] q 
                        INNER JOIN [Enrolment].[Qa2QueueNote] qn on qn.EnrolmentQa2QueueEntryId = q.Id
                        OUTER APPLY (
                            SELECT TOP(1) [e1].[Created], [e1].[CreatedBy]
                            FROM [Participant].[EnrolmentHistory] AS [e1]
                            WHERE [e1].[ParticipantId] = [q].[ParticipantId] 
                            AND CAST([e1].[EnrolmentStatus] AS int) = 2 
                            AND [e1].[Created] < [qn].[Created]
                            ORDER BY [e1].[Created] DESC
                        ) AS [e2]
                        WHERE qn.IsExternal = 1
                    ) q ON 
                        pfe.QueueEntryId = q.Qa2QueueEntryId 
                        AND pfe.NoteId = q.NoteId 
                        AND pfe.SourceTable = 'Enrolment.Qa2QueueNote'
                WHERE 
                    pfe.ProviderQaUserId <> q.ProviderQaUserId 
                    OR pfe.CfoUserId <> q.CfoUserId 
                    OR pfe.PqaSubmittedDate <> q.PqaSubmittedDate
                    OR pfe.ActionDate <> q.ActionDate 
            ");

            migrationBuilder.Sql(@"
                UPDATE pfe
                SET 
                    ProviderQaUserId = q.ProviderQaUserId,
                    CfoUserId = q.CfoUserId,
                    PqaSubmittedDate = q.PqaSubmittedDate,
                    ActionDate = q.ActionDate
                FROM [Mi].ProviderFeedbackEnrolment pfe
                    INNER JOIN 
                    (
                        SELECT
                            eq.ParticipantId,                 
                            e2.CreatedBy as ProviderQaUserId,
                            en.CreatedBy as CfoUserId,
                            e2.Created as PqaSubmittedDate,
                            en.Created as ActionDate,
                            eq.LastModified as QueueLastModified,
                            eq.Id as EscalationQueueEntryId,
                            en.Id as NoteId
                        FROM [Enrolment].[EscalationQueue] eq 
                        INNER JOIN [Enrolment].[EscalationNote] en on en.EnrolmentEscalationQueueEntryId = eq.Id
                        OUTER APPLY (
                            SELECT TOP(1) [e1].[Created], [e1].[CreatedBy]
                            FROM [Participant].[EnrolmentHistory] AS [e1]
                            WHERE [e1].[ParticipantId] = [eq].[ParticipantId] 
                            AND CAST([e1].[EnrolmentStatus] AS int) = 2 
                            AND [e1].[Created] < [en].[Created]
                            ORDER BY [e1].[Created] DESC
                        ) AS [e2]
                        WHERE en.IsExternal = 1 
                    ) q ON 
                        pfe.QueueEntryId = q.EscalationQueueEntryId 
                        AND pfe.NoteId = q.NoteId 
                        AND pfe.SourceTable = 'Enrolment.EscalationNote'
                WHERE 
                    pfe.ProviderQaUserId <> q.ProviderQaUserId 
                    OR pfe.CfoUserId <> q.CfoUserId 
                    OR pfe.PqaSubmittedDate <> q.PqaSubmittedDate
                    OR pfe.ActionDate <> q.ActionDate
            ");

            migrationBuilder.Sql(@"
                UPDATE pfa
                SET 
                    ProviderQaUserId = q.ProviderQaUserId,
                    CfoUserId = q.CfoUserId,
                    PqaSubmittedDate = q.PqaSubmittedDate,
                    ActionDate = q.ActionDate
                FROM 
                    [Mi].ProviderFeedbackActivity pfa
                    INNER JOIN
                    (
                        SELECT
                            aq.ActivityId,                 
                            a3.LastModifiedBy as ProviderQaUserId,
                            qn.CreatedBy as CfoUserId,
                            a3.LastModified as PqaSubmittedDate,
                            qn.Created as ActionDate,
                            qn.[ActivityQa2QueueEntryId] as Qa2QueueEntryId,
                            qn.Id as NoteId,
                            aq.LastModified as QueueLastModified
                        FROM [Activities].[ActivityQA2Queue] aq 
                        INNER JOIN [Activities].[QA2QueueNote] qn on qn.[ActivityQA2QueueEntryId] = aq.Id
                        OUTER APPLY (
                            SELECT TOP(1) [a2].[LastModified], [a2].[LastModifiedBy]
                            FROM [Activities].[ActivityPqaQueue] AS [a2]
                            WHERE [a2].[ActivityId] = [aq].[ActivityId] 
                            AND [a2].[LastModified] < [qn].[Created]
                            ORDER BY [a2].[LastModified] DESC
                        ) AS [a3]
                        WHERE qn.IsExternal = 1
                    ) q on 
                        pfa.QueueEntryId = q.Qa2QueueEntryId 
                        AND pfa.NoteId = q.NoteId 
                        AND pfa.SourceTable = 'Activities.Qa2QueueNote'
                WHERE 
                    pfa.ProviderQaUserId <> q.ProviderQaUserId 
                    OR pfa.CfoUserId <> q.CfoUserId 
                    OR pfa.PqaSubmittedDate <> q.PqaSubmittedDate
                    OR pfa.ActionDate <> q.ActionDate
            ");

            migrationBuilder.Sql(@"
                UPDATE pfa
                SET 
                    ProviderQaUserId = q.ProviderQaUserId,
                    CfoUserId = q.CfoUserId,
                    PqaSubmittedDate = q.PqaSubmittedDate,
                    ActionDate = q.ActionDate
                FROM
                    [Mi].ProviderFeedbackActivity pfa
                    INNER JOIN 
                    (
                        SELECT      
                            aeq.ActivityId,           
                            a3.LastModifiedBy as ProviderQaUserId,
                            en.CreatedBy as CfoUserId,
                            a3.LastModified as PqaSubmittedDate,
                            en.Created as ActionDate,
                            en.ActivityEscalationQueueEntryId as EscalationQueueEntryId,
                            en.Id as NoteId,
                            aeq.LastModified as QueueLastModified
                        FROM [Activities].[ActivityEscalationQueue] aeq 
                        INNER JOIN [Activities].[EscalationNote] en on en.[ActivityEscalationQueueEntryId] = aeq.Id
                        OUTER APPLY (
                            SELECT TOP(1) [a2].[LastModified], [a2].[LastModifiedBy]
                            FROM [Activities].[ActivityPqaQueue] AS [a2]
                            WHERE [a2].[ActivityId] = [aeq].[ActivityId] 
                            AND [a2].[LastModified] < [en].[Created]
                            ORDER BY [a2].[LastModified] DESC
                        ) AS [a3]
                        WHERE en.IsExternal = 1
                    ) q ON 
                        pfa.QueueEntryId = q.EscalationQueueEntryId 
                        AND pfa.NoteId = q.NoteId 
                        AND pfa.SourceTable = 'Activities.EscalationNote'
                WHERE 
                    pfa.ProviderQaUserId <> q.ProviderQaUserId 
                    OR pfa.CfoUserId <> q.CfoUserId 
                    OR pfa.PqaSubmittedDate <> q.PqaSubmittedDate
                    OR pfa.ActionDate <> q.ActionDate
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
