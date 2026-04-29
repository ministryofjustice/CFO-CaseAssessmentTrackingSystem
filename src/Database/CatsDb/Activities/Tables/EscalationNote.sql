CREATE TABLE [Activities].[EscalationNote] (
    [Id]                             INT              IDENTITY (1, 1) NOT NULL,
    [IsExternal]                     BIT              NOT NULL,
    [ActivityEscalationQueueEntryId] UNIQUEIDENTIFIER NOT NULL,
    [Message]                        NVARCHAR (1000)  NOT NULL,
    [CallReference]                  NVARCHAR (20)    NULL,
    [Created]                        DATETIME2 (7)    NULL,
    [CreatedBy]                      NVARCHAR (36)    NULL,
    [LastModified]                   DATETIME2 (7)    NULL,
    [LastModifiedBy]                 NVARCHAR (36)    NULL,
    [TenantId]                       NVARCHAR (50)    NOT NULL,
    [FeedbackType]                   INT              NULL,
    [ReturnReason]                   NVARCHAR (100)   NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_EscalationNote_LastModifiedBy]
    ON [Activities].[EscalationNote]([LastModifiedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_EscalationNote_CreatedBy]
    ON [Activities].[EscalationNote]([CreatedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_EscalationNote_ActivityEscalationQueueEntryId]
    ON [Activities].[EscalationNote]([ActivityEscalationQueueEntryId] ASC);
GO

ALTER TABLE [Activities].[EscalationNote]
    ADD CONSTRAINT [FK_EscalationNote_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[EscalationNote]
    ADD CONSTRAINT [FK_EscalationNote_ActivityEscalationQueue_ActivityEscalationQueueEntryId] FOREIGN KEY ([ActivityEscalationQueueEntryId]) REFERENCES [Activities].[ActivityEscalationQueue] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Activities].[EscalationNote]
    ADD CONSTRAINT [FK_EscalationNote_User_LastModifiedBy] FOREIGN KEY ([LastModifiedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[EscalationNote]
    ADD CONSTRAINT [PK_EscalationNote] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

