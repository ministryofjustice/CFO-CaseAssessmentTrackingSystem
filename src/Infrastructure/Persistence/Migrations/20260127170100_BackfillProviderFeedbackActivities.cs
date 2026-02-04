using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BackfillProviderFeedbackActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Activity Returned - QA2 Queue
            migrationBuilder.Sql(@"
                INSERT INTO [Mi].[ProviderFeedbackActivity] (
                    [Id], [CreatedOn], [SourceTable], [Queue], [QueueEntryId], [NoteId],
                    [ActivityId], [ActivityType], [TenantId], [ContractId], [ParticipantId],
                    [SupportWorkerId], [ProviderQaUserId], [CfoUserId], [PqaSubmittedDate],
                    [ActionDate], [Message], [FeedbackType]
                )
                SELECT 
                    NEWID() AS [Id],
                    GETUTCDATE() AS [CreatedOn],
                    N'Activities.Qa2QueueNote' AS [SourceTable],
                    N'QA2' AS [Queue],
                    [qn].[ActivityQa2QueueEntryId] AS [QueueEntryId],
                    [qn].[Id] AS [NoteId],
                    [aq].[ActivityId],
                    (SELECT [a1].[Type] FROM [Activities].[Activity] AS [a1] WHERE [a1].[Id] = [aq].[ActivityId]) AS [ActivityType],
                    [aq].[TenantId],
                    (
                        SELECT TOP(1) [c].[Id]
                        FROM [Configuration].[Contract] AS [c]
                        LEFT JOIN [Configuration].[Tenant] AS [t] ON [c].[TenantId] = [t].[Id]
                        WHERE [t].[Id] IS NOT NULL AND LEFT([aq].[TenantId], LEN([t].[Id])) = [t].[Id]
                        ORDER BY CAST(LEN([t].[Id]) AS int) DESC
                    ) AS [ContractId],
                    [aq].[ParticipantId],
                    [aq].[SupportWorkerId],
                    [a3].[LastModifiedBy] AS [ProviderQaUserId],
                    [aq].[OwnerId] AS [CfoUserId],
                    [a3].[LastModified] AS [PqaSubmittedDate],
                    [aq].[Created] AS [ActionDate],
                    REPLACE(REPLACE([qn].[Message], nchar(13), N' '), nchar(10), N' ') AS [Message],
                    [qn].[FeedbackType]
                FROM [Activities].[ActivityQa2Queue] AS [aq]
                INNER JOIN [Activities].[Qa2QueueNote] AS [qn] ON [aq].[Id] = [qn].[ActivityQa2QueueEntryId]
                OUTER APPLY (
                    SELECT TOP(1) [a2].[LastModified], [a2].[LastModifiedBy]
                    FROM [Activities].[ActivityPqaQueue] AS [a2]
                    WHERE [a2].[ActivityId] = [aq].[ActivityId] 
                    AND [a2].[LastModified] < [aq].[Created]
                    ORDER BY [a2].[LastModified] DESC
                ) AS [a3]
                WHERE [aq].[IsCompleted] = CAST(1 AS bit) 
                AND [aq].[IsAccepted] = CAST(0 AS bit)
                AND CAST(LEN([aq].[TenantId]) AS int) > 6
                AND NOT EXISTS (
                    SELECT 1 
                    FROM [Mi].[ProviderFeedbackActivity] AS [pfa]
                    WHERE [pfa].[QueueEntryId] = [qn].[ActivityQa2QueueEntryId]
                        AND [pfa].[NoteId] = [qn].[Id]
                        AND [pfa].[SourceTable] = N'Activities.Qa2QueueNote'
                );
            ");

            // Activity Returned - Escalation Queue
            migrationBuilder.Sql(@"
                INSERT INTO [Mi].[ProviderFeedbackActivity] (
                    [Id], [CreatedOn], [SourceTable], [Queue], [QueueEntryId], [NoteId],
                    [ActivityId], [ActivityType], [TenantId], [ContractId], [ParticipantId],
                    [SupportWorkerId], [ProviderQaUserId], [CfoUserId], [PqaSubmittedDate],
                    [ActionDate], [Message], [FeedbackType]
                )
                SELECT 
                    NEWID() AS [Id],
                    GETUTCDATE() AS [CreatedOn],
                    N'Activities.EscalationNote' AS [SourceTable],
                    N'Escalation' AS [Queue],
                    [en].[ActivityEscalationQueueEntryId] AS [QueueEntryId],
                    [en].[Id] AS [NoteId],
                    [aeq].[ActivityId],
                    (SELECT [a1].[Type] FROM [Activities].[Activity] AS [a1] WHERE [a1].[Id] = [aeq].[ActivityId]) AS [ActivityType],
                    [aeq].[TenantId],
                    (
                        SELECT TOP(1) [c].[Id]
                        FROM [Configuration].[Contract] AS [c]
                        LEFT JOIN [Configuration].[Tenant] AS [t] ON [c].[TenantId] = [t].[Id]
                        WHERE [t].[Id] IS NOT NULL AND LEFT([aeq].[TenantId], LEN([t].[Id])) = [t].[Id]
                        ORDER BY CAST(LEN([t].[Id]) AS int) DESC
                    ) AS [ContractId],
                    [aeq].[ParticipantId],
                    [aeq].[SupportWorkerId],
                    [a3].[LastModifiedBy] AS [ProviderQaUserId],
                    [aeq].[CreatedBy] AS [CfoUserId],
                    [a3].[LastModified] AS [PqaSubmittedDate],
                    [aeq].[Created] AS [ActionDate],
                    REPLACE(REPLACE([en].[Message], nchar(13), N' '), nchar(10), N' ') AS [Message],
                    [en].[FeedbackType]
                FROM [Activities].[ActivityEscalationQueue] AS [aeq]
                INNER JOIN [Activities].[EscalationNote] AS [en] ON [aeq].[Id] = [en].[ActivityEscalationQueueEntryId]
                OUTER APPLY (
                    SELECT TOP(1) [a2].[LastModified], [a2].[LastModifiedBy]
                    FROM [Activities].[ActivityPqaQueue] AS [a2]
                    WHERE [a2].[ActivityId] = [aeq].[ActivityId] 
                    AND [a2].[LastModified] < [aeq].[Created]
                    ORDER BY [a2].[LastModified] DESC
                ) AS [a3]
                WHERE [aeq].[IsCompleted] = CAST(1 AS bit) 
                AND [aeq].[IsAccepted] = CAST(0 AS bit)
                AND CAST(LEN([aeq].[TenantId]) AS int) > 6
                AND NOT EXISTS (
                    SELECT 1 
                    FROM [Mi].[ProviderFeedbackActivity] AS [pfa]
                    WHERE [pfa].[QueueEntryId] = [en].[ActivityEscalationQueueEntryId]
                        AND [pfa].[NoteId] = [en].[Id]
                        AND [pfa].[SourceTable] = N'Activities.EscalationNote'
                );
            ");

            // Activity Advisory - QA2 Queue
            migrationBuilder.Sql(@"
                INSERT INTO [Mi].[ProviderFeedbackActivity] (
                    [Id], [CreatedOn], [SourceTable], [Queue], [QueueEntryId], [NoteId],
                    [ActivityId], [ActivityType], [TenantId], [ContractId], [ParticipantId],
                    [SupportWorkerId], [ProviderQaUserId], [CfoUserId], [PqaSubmittedDate],
                    [ActionDate], [Message], [FeedbackType]
                )
                SELECT 
                    NEWID() AS [Id],
                    GETUTCDATE() AS [CreatedOn],
                    N'Activities.Qa2QueueNote' AS [SourceTable],
                    N'QA2' AS [Queue],
                    [qn].[ActivityQa2QueueEntryId] AS [QueueEntryId],
                    [qn].[Id] AS [NoteId],
                    [aq].[ActivityId],
                    (SELECT [a1].[Type] FROM [Activities].[Activity] AS [a1] WHERE [a1].[Id] = [aq].[ActivityId]) AS [ActivityType],
                    [aq].[TenantId],
                    (
                        SELECT TOP(1) [c].[Id]
                        FROM [Configuration].[Contract] AS [c]
                        LEFT JOIN [Configuration].[Tenant] AS [t] ON [c].[TenantId] = [t].[Id]
                        WHERE [t].[Id] IS NOT NULL AND LEFT([aq].[TenantId], LEN([t].[Id])) = [t].[Id]
                        ORDER BY CAST(LEN([t].[Id]) AS int) DESC
                    ) AS [ContractId],
                    [aq].[ParticipantId],
                    [aq].[SupportWorkerId],
                    [a3].[LastModifiedBy] AS [ProviderQaUserId],
                    [aq].[OwnerId] AS [CfoUserId],
                    [a3].[LastModified] AS [PqaSubmittedDate],
                    [aq].[Created] AS [ActionDate],
                    REPLACE(REPLACE([qn].[Message], nchar(13), N' '), nchar(10), N' ') AS [Message],
                    [qn].[FeedbackType]
                FROM [Activities].[ActivityQa2Queue] AS [aq]
                INNER JOIN [Activities].[Qa2QueueNote] AS [qn] ON [aq].[Id] = [qn].[ActivityQa2QueueEntryId]
                OUTER APPLY (
                    SELECT TOP(1) [a2].[LastModified], [a2].[LastModifiedBy]
                    FROM [Activities].[ActivityPqaQueue] AS [a2]
                    WHERE [a2].[ActivityId] = [aq].[ActivityId] 
                    AND [a2].[LastModified] < [aq].[Created]
                    ORDER BY [a2].[LastModified] DESC
                ) AS [a3]
                WHERE ([qn].[Message] LIKE N'Advisory%' OR [qn].[Message] LIKE N'Accepted by Exception%')
                AND [aq].[IsAccepted] = CAST(1 AS bit) 
                AND [aq].[IsCompleted] = CAST(1 AS bit)
                AND CAST(LEN([aq].[TenantId]) AS int) > 6
                AND NOT EXISTS (
                    SELECT 1 
                    FROM [Mi].[ProviderFeedbackActivity] AS [pfa]
                    WHERE [pfa].[QueueEntryId] = [qn].[ActivityQa2QueueEntryId]
                        AND [pfa].[NoteId] = [qn].[Id]
                        AND [pfa].[SourceTable] = N'Activities.Qa2QueueNote'
                );
            ");

            // Activity Advisory - Escalation Queue
            migrationBuilder.Sql(@"
                INSERT INTO [Mi].[ProviderFeedbackActivity] (
                    [Id], [CreatedOn], [SourceTable], [Queue], [QueueEntryId], [NoteId],
                    [ActivityId], [ActivityType], [TenantId], [ContractId], [ParticipantId],
                    [SupportWorkerId], [ProviderQaUserId], [CfoUserId], [PqaSubmittedDate],
                    [ActionDate], [Message], [FeedbackType]
                )
                SELECT 
                    NEWID() AS [Id],
                    GETUTCDATE() AS [CreatedOn],
                    N'Activities.EscalationNote' AS [SourceTable],
                    N'Escalation' AS [Queue],
                    [en].[ActivityEscalationQueueEntryId] AS [QueueEntryId],
                    [en].[Id] AS [NoteId],
                    [aeq].[ActivityId],
                    (SELECT [a1].[Type] FROM [Activities].[Activity] AS [a1] WHERE [a1].[Id] = [aeq].[ActivityId]) AS [ActivityType],
                    [aeq].[TenantId],
                    (
                        SELECT TOP(1) [c].[Id]
                        FROM [Configuration].[Contract] AS [c]
                        LEFT JOIN [Configuration].[Tenant] AS [t] ON [c].[TenantId] = [t].[Id]
                        WHERE [t].[Id] IS NOT NULL AND LEFT([aeq].[TenantId], LEN([t].[Id])) = [t].[Id]
                        ORDER BY CAST(LEN([t].[Id]) AS int) DESC
                    ) AS [ContractId],
                    [aeq].[ParticipantId],
                    [aeq].[SupportWorkerId],
                    [a3].[LastModifiedBy] AS [ProviderQaUserId],
                    [aeq].[CreatedBy] AS [CfoUserId],
                    [a3].[LastModified] AS [PqaSubmittedDate],
                    [aeq].[Created] AS [ActionDate],
                    REPLACE(REPLACE([en].[Message], nchar(13), N' '), nchar(10), N' ') AS [Message],
                    [en].[FeedbackType]
                FROM [Activities].[ActivityEscalationQueue] AS [aeq]
                INNER JOIN [Activities].[EscalationNote] AS [en] ON [aeq].[Id] = [en].[ActivityEscalationQueueEntryId]
                OUTER APPLY (
                    SELECT TOP(1) [a2].[LastModified], [a2].[LastModifiedBy]
                    FROM [Activities].[ActivityPqaQueue] AS [a2]
                    WHERE [a2].[ActivityId] = [aeq].[ActivityId] 
                    AND [a2].[LastModified] < [aeq].[Created]
                    ORDER BY [a2].[LastModified] DESC
                ) AS [a3]
                WHERE ([en].[Message] LIKE N'Advisory%' OR [en].[Message] LIKE N'Accepted by Exception%')
                AND [aeq].[IsAccepted] = CAST(1 AS bit) 
                AND [aeq].[IsCompleted] = CAST(1 AS bit)
                AND CAST(LEN([aeq].[TenantId]) AS int) > 6
                AND NOT EXISTS (
                    SELECT 1 
                    FROM [Mi].[ProviderFeedbackActivity] AS [pfa]
                    WHERE [pfa].[QueueEntryId] = [en].[ActivityEscalationQueueEntryId]
                        AND [pfa].[NoteId] = [en].[Id]
                        AND [pfa].[SourceTable] = N'Activities.EscalationNote'
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Clear all backfilled data
            migrationBuilder.Sql(@"
                DELETE FROM [MI].[ProviderFeedbackActivity];
            ");
        }
    }
}
