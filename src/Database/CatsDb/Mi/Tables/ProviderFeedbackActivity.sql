CREATE TABLE [Mi].[ProviderFeedbackActivity] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]        DATETIME2 (7)    NOT NULL,
    [SourceTable]      NVARCHAR (100)   NOT NULL,
    [Queue]            NVARCHAR (50)    NOT NULL,
    [QueueEntryId]     UNIQUEIDENTIFIER NOT NULL,
    [NoteId]           INT              NOT NULL,
    [TenantId]         NVARCHAR (50)    NOT NULL,
    [ContractId]       NVARCHAR (12)    NOT NULL,
    [ParticipantId]    NVARCHAR (9)     NOT NULL,
    [SupportWorkerId]  NVARCHAR (36)    NOT NULL,
    [ProviderQaUserId] NVARCHAR (36)    NOT NULL,
    [CfoUserId]        NVARCHAR (36)    NOT NULL,
    [PqaSubmittedDate] DATETIME2 (7)    NOT NULL,
    [ActionDate]       DATETIME2 (7)    NOT NULL,
    [Message]          NVARCHAR (1000)  NOT NULL,
    [FeedbackType]     INT              NULL,
    [ActivityId]       NVARCHAR (36)    DEFAULT (N'') NOT NULL,
    [ActivityType]     INT              DEFAULT ((0)) NOT NULL
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [ix_ProviderFeedbackActivity_SourceTable_QueueEntryId_NoteId]
    ON [Mi].[ProviderFeedbackActivity]([SourceTable] ASC, [QueueEntryId] ASC, [NoteId] ASC);
GO

CREATE NONCLUSTERED INDEX [ix_ProviderFeedbackActivity_SourceTable_TenantId_ActionDate]
    ON [Mi].[ProviderFeedbackActivity]([SourceTable] ASC, [TenantId] ASC, [ActionDate] ASC);
GO

ALTER TABLE [Mi].[ProviderFeedbackActivity]
    ADD CONSTRAINT [PK_ProviderFeedbackActivity] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

