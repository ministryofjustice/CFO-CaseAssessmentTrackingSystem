CREATE TABLE [Mi].[ProviderFeedbackEnrolment] (
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
    [ReturnReason]     NVARCHAR (100)   NULL
);
GO

ALTER TABLE [Mi].[ProviderFeedbackEnrolment]
    ADD CONSTRAINT [PK_ProviderFeedbackEnrolment] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [ix_ProviderFeedbackEnrolment_SourceTable_QueueEntryId_NoteId]
    ON [Mi].[ProviderFeedbackEnrolment]([SourceTable] ASC, [QueueEntryId] ASC, [NoteId] ASC);
GO

CREATE NONCLUSTERED INDEX [ix_ProviderFeedbackEnrolment_SourceTable_TenantId_ActionDate]
    ON [Mi].[ProviderFeedbackEnrolment]([SourceTable] ASC, [TenantId] ASC, [ActionDate] ASC);
GO

