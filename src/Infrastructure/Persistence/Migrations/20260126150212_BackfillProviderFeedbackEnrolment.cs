using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BackfillProviderFeedbackEnrolment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            // Backfill Enrolment Returned from QA2 Queue
            migrationBuilder.Sql(@"
                INSERT INTO [Mi].[ProviderFeedbackEnrolment] (
                    [Id], [CreatedOn], [SourceTable], [Queue], [QueueEntryId], [NoteId],
                    [TenantId], [ContractId], [ParticipantId], [SupportWorkerId],
                    [ProviderQaUserId], [CfoUserId], [PqaSubmittedDate], [ActionDate],
                    [Message], [FeedbackType]
                )
                SELECT 
                    NEWID() AS [Id],
                    GETUTCDATE() AS [CreatedOn],
                    'Enrolment.Qa2QueueNote' AS [SourceTable],
                    N'QA2' AS [Queue],
                    [qn].[EnrolmentQa2QueueEntryId] AS [QueueEntryId],
                    [qn].[Id] AS [NoteId],
                    [q].[TenantId],
                    (
                        SELECT TOP(1) [c].[Id]
                        FROM [Configuration].[Contract] AS [c]
                        LEFT JOIN [Configuration].[Tenant] AS [t] ON [c].[TenantId] = [t].[Id]
                        WHERE [t].[Id] IS NOT NULL AND LEFT([q].[TenantId], LEN([t].[Id])) = [t].[Id]
                        ORDER BY CAST(LEN([t].[Id]) AS int) DESC
                    ) AS [ContractId],
                    [q].[ParticipantId],
                    [q].[SupportWorkerId],
                    [e2].[CreatedBy] AS [ProviderQaUserId],
                    [q].[OwnerId] AS [CfoUserId],
                    [e2].[Created] AS [PqaSubmittedDate],
                    [q].[Created] AS [ActionDate],
                    REPLACE(REPLACE([qn].[Message], nchar(13), N' '), nchar(10), N' ') AS [Message],
                    [qn].[FeedbackType]
                FROM [Enrolment].[Qa2Queue] AS [q]
                INNER JOIN [Enrolment].[Qa2QueueNote] AS [qn] ON [q].[Id] = [qn].[EnrolmentQa2QueueEntryId]
                OUTER APPLY (
                    SELECT TOP(1) [e1].[Created], [e1].[CreatedBy]
                    FROM [Participant].[EnrolmentHistory] AS [e1]
                    WHERE [e1].[ParticipantId] = [q].[ParticipantId] 
                    AND CAST([e1].[EnrolmentStatus] AS int) = 2 
                    AND [e1].[Created] < [q].[Created]
                    ORDER BY [e1].[Created] DESC
                ) AS [e2]
                WHERE [q].[IsCompleted] = CAST(1 AS bit) 
                AND [q].[IsAccepted] = CAST(0 AS bit)
                AND CAST(LEN([q].[TenantId]) AS int) > 6
                AND NOT EXISTS (
                    SELECT 1 
                    FROM [Mi].[ProviderFeedbackEnrolment] AS [pfe]
                    WHERE [pfe].[QueueEntryId] = [qn].[EnrolmentQa2QueueEntryId]
                        AND [pfe].[NoteId] = [qn].[Id]
                        AND [pfe].[SourceTable] = 'Enrolment.Qa2QueueNote'
                );
            ");

            // Backfill Enrolment Returned from Escalation Queue
            migrationBuilder.Sql(@"
                INSERT INTO [Mi].[ProviderFeedbackEnrolment] (
                    [Id], [CreatedOn], [SourceTable], [Queue], [QueueEntryId], [NoteId],
                    [TenantId], [ContractId], [ParticipantId], [SupportWorkerId],
                    [ProviderQaUserId], [CfoUserId], [PqaSubmittedDate], [ActionDate],
                    [Message], [FeedbackType]
                )
                SELECT 
                    NEWID() AS [Id],
                    GETUTCDATE() AS [CreatedOn],
                    'Enrolment.EscalationNote' AS [SourceTable],
                    N'Escalation' AS [Queue],
                    [en].[EnrolmentEscalationQueueEntryId] AS [QueueEntryId],
                    [en].[Id] AS [NoteId],
                    [e].[TenantId],
                    (
                        SELECT TOP(1) [c].[Id]
                        FROM [Configuration].[Contract] AS [c]
                        LEFT JOIN [Configuration].[Tenant] AS [t] ON [c].[TenantId] = [t].[Id]
                        WHERE [t].[Id] IS NOT NULL AND LEFT([e].[TenantId], LEN([t].[Id])) = [t].[Id]
                        ORDER BY CAST(LEN([t].[Id]) AS int) DESC
                    ) AS [ContractId],
                    [e].[ParticipantId],
                    [e].[SupportWorkerId],
                    [e2].[CreatedBy] AS [ProviderQaUserId],
                    [e].[CreatedBy] AS [CfoUserId],
                    [e2].[Created] AS [PqaSubmittedDate],
                    [e].[Created] AS [ActionDate],
                    REPLACE(REPLACE([en].[Message], nchar(13), N' '), nchar(10), N' ') AS [Message],
                    [en].[FeedbackType]
                FROM [Enrolment].[EscalationQueue] AS [e]
                INNER JOIN [Enrolment].[EscalationNote] AS [en] ON [e].[Id] = [en].[EnrolmentEscalationQueueEntryId]
                OUTER APPLY (
                    SELECT TOP(1) [e1].[Created], [e1].[CreatedBy]
                    FROM [Participant].[EnrolmentHistory] AS [e1]
                    WHERE [e1].[ParticipantId] = [e].[ParticipantId] 
                    AND CAST([e1].[EnrolmentStatus] AS int) = 2 
                    AND [e1].[Created] < [e].[Created]
                    ORDER BY [e1].[Created] DESC
                ) AS [e2]
                WHERE [e].[IsCompleted] = CAST(1 AS bit) 
                AND [e].[IsAccepted] = CAST(0 AS bit)
                AND CAST(LEN([e].[TenantId]) AS int) > 6
                AND NOT EXISTS (
                    SELECT 1 
                    FROM [Mi].[ProviderFeedbackEnrolment] AS [pfe]
                    WHERE [pfe].[QueueEntryId] = [en].[EnrolmentEscalationQueueEntryId]
                        AND [pfe].[NoteId] = [en].[Id]
                        AND [pfe].[SourceTable] = 'Enrolment.EscalationNote'
                );
            ");

            // Backfill Enrolments Advisory from QA2 Queue
            migrationBuilder.Sql(@"
                INSERT INTO [Mi].[ProviderFeedbackEnrolment] (
                [Id], [CreatedOn], [SourceTable], [Queue], [QueueEntryId], [NoteId],
                [TenantId], [ContractId], [ParticipantId], [SupportWorkerId],
                [ProviderQaUserId], [CfoUserId], [PqaSubmittedDate], [ActionDate],
                [Message], [FeedbackType]
                )
                SELECT 
                    NEWID() AS [Id],
                    GETUTCDATE() AS [CreatedOn],
                    'Enrolment.Qa2QueueNote' AS [SourceTable],
                    N'QA2' AS [Queue],
                    [qn].[EnrolmentQa2QueueEntryId] AS [QueueEntryId],
                    [qn].[Id] AS [NoteId],
                    [q].[TenantId],
                    (
                        SELECT TOP(1) [c].[Id]
                        FROM [Configuration].[Contract] AS [c]
                        LEFT JOIN [Configuration].[Tenant] AS [t] ON [c].[TenantId] = [t].[Id]
                        WHERE [t].[Id] IS NOT NULL AND LEFT([q].[TenantId], LEN([t].[Id])) = [t].[Id]
                        ORDER BY CAST(LEN([t].[Id]) AS int) DESC
                    ) AS [ContractId],
                    [q].[ParticipantId],
                    [q].[SupportWorkerId],
                    [e2].[CreatedBy] AS [ProviderQaUserId],
                    [q].[OwnerId] AS [CfoUserId],
                    [e2].[Created] AS [PqaSubmittedDate],
                    [q].[Created] AS [ActionDate],
                    REPLACE(REPLACE([qn].[Message], nchar(13), N' '), nchar(10), N' ') AS [Message],
                    [qn].[FeedbackType]
                FROM [Enrolment].[Qa2Queue] AS [q]
                INNER JOIN [Enrolment].[Qa2QueueNote] AS [qn] ON [q].[Id] = [qn].[EnrolmentQa2QueueEntryId]
                OUTER APPLY (
                    SELECT TOP(1) [e1].[Created], [e1].[CreatedBy]
                    FROM [Participant].[EnrolmentHistory] AS [e1]
                    WHERE [e1].[ParticipantId] = [q].[ParticipantId] 
                    AND CAST([e1].[EnrolmentStatus] AS int) = 2 
                    AND [e1].[Created] < [q].[Created]
                    ORDER BY [e1].[Created] DESC
                ) AS [e2]
                WHERE ([qn].[Message] LIKE N'Advisory%' OR [qn].[Message] LIKE N'Accepted by Exception%')
                AND [q].[IsAccepted] = CAST(1 AS bit) 
                AND [q].[IsCompleted] = CAST(1 AS bit)
                AND CAST(LEN([q].[TenantId]) AS int) > 6
                AND NOT EXISTS (
                    SELECT 1 
                    FROM [Mi].[ProviderFeedbackEnrolment] AS [pfe]
                    WHERE [pfe].[QueueEntryId] = [qn].[EnrolmentQa2QueueEntryId]
                        AND [pfe].[NoteId] = [qn].[Id]
                        AND [pfe].[SourceTable] = 'Enrolment.Qa2QueueNote'
                );
            ");

            // Backfill Enrolments Advisory from Escalation Queue
            migrationBuilder.Sql(@"
                INSERT INTO [Mi].[ProviderFeedbackEnrolment] (
                    [Id], [CreatedOn], [SourceTable], [Queue], [QueueEntryId], [NoteId],
                    [TenantId], [ContractId], [ParticipantId], [SupportWorkerId],
                    [ProviderQaUserId], [CfoUserId], [PqaSubmittedDate], [ActionDate],
                    [Message], [FeedbackType]
                )
                SELECT 
                    NEWID() AS [Id],
                    GETUTCDATE() AS [CreatedOn],
                    'Enrolment.EscalationNote' AS [SourceTable],
                    N'Escalation' AS [Queue],
                    [en].[EnrolmentEscalationQueueEntryId] AS [QueueEntryId],
                    [en].[Id] AS [NoteId],
                    [e].[TenantId],
                    (
                        SELECT TOP(1) [c].[Id]
                        FROM [Configuration].[Contract] AS [c]
                        LEFT JOIN [Configuration].[Tenant] AS [t] ON [c].[TenantId] = [t].[Id]
                        WHERE [t].[Id] IS NOT NULL AND LEFT([e].[TenantId], LEN([t].[Id])) = [t].[Id]
                        ORDER BY CAST(LEN([t].[Id]) AS int) DESC
                    ) AS [ContractId],
                    [e].[ParticipantId],
                    [e].[SupportWorkerId],
                    [e2].[CreatedBy] AS [ProviderQaUserId],
                    [e].[CreatedBy] AS [CfoUserId],
                    [e2].[Created] AS [PqaSubmittedDate],
                    [e].[Created] AS [ActionDate],
                    REPLACE(REPLACE([en].[Message], nchar(13), N' '), nchar(10), N' ') AS [Message],
                    [en].[FeedbackType]
                FROM [Enrolment].[EscalationQueue] AS [e]
                INNER JOIN [Enrolment].[EscalationNote] AS [en] ON [e].[Id] = [en].[EnrolmentEscalationQueueEntryId]
                OUTER APPLY (
                    SELECT TOP(1) [e1].[Created], [e1].[CreatedBy]
                    FROM [Participant].[EnrolmentHistory] AS [e1]
                    WHERE [e1].[ParticipantId] = [e].[ParticipantId] 
                    AND CAST([e1].[EnrolmentStatus] AS int) = 2 
                    AND [e1].[Created] < [e].[Created]
                    ORDER BY [e1].[Created] DESC
                ) AS [e2]
                WHERE ([en].[Message] LIKE N'Advisory%' OR [en].[Message] LIKE N'Accepted by Exception%')
                AND [e].[IsAccepted] = CAST(1 AS bit) 
                AND [e].[IsCompleted] = CAST(1 AS bit)
                AND CAST(LEN([e].[TenantId]) AS int) > 6
                AND NOT EXISTS (
                    SELECT 1 
                    FROM [Mi].[ProviderFeedbackEnrolment] AS [pfe]
                    WHERE [pfe].[QueueEntryId] = [en].[EnrolmentEscalationQueueEntryId]
                        AND [pfe].[NoteId] = [en].[Id]
                        AND [pfe].[SourceTable] = 'Enrolment.EscalationNote'
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Clear all backfilled data
            migrationBuilder.Sql(@"
                DELETE FROM [MI].[ProviderFeedbackEnrolment];
            ");
        }
    }
}
