CREATE TABLE [Activities].[Qa2QueueNote] (
    [Id]                      INT              IDENTITY (1, 1) NOT NULL,
    [IsExternal]              BIT              NOT NULL,
    [ActivityQa2QueueEntryId] UNIQUEIDENTIFIER NOT NULL,
    [Message]                 NVARCHAR (1000)  NOT NULL,
    [CallReference]           NVARCHAR (20)    NULL,
    [Created]                 DATETIME2 (7)    NULL,
    [CreatedBy]               NVARCHAR (36)    NULL,
    [LastModified]            DATETIME2 (7)    NULL,
    [LastModifiedBy]          NVARCHAR (36)    NULL,
    [TenantId]                NVARCHAR (50)    NOT NULL,
    [FeedbackType]            INT              NULL,
    [ReturnReason]            NVARCHAR (100)   NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_Qa2QueueNote_LastModifiedBy]
    ON [Activities].[Qa2QueueNote]([LastModifiedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Qa2QueueNote_CreatedBy]
    ON [Activities].[Qa2QueueNote]([CreatedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Qa2QueueNote_ActivityQa2QueueEntryId]
    ON [Activities].[Qa2QueueNote]([ActivityQa2QueueEntryId] ASC);
GO

ALTER TABLE [Activities].[Qa2QueueNote]
    ADD CONSTRAINT [FK_Qa2QueueNote_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[Qa2QueueNote]
    ADD CONSTRAINT [FK_Qa2QueueNote_User_LastModifiedBy] FOREIGN KEY ([LastModifiedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[Qa2QueueNote]
    ADD CONSTRAINT [FK_Qa2QueueNote_ActivityQa2Queue_ActivityQa2QueueEntryId] FOREIGN KEY ([ActivityQa2QueueEntryId]) REFERENCES [Activities].[ActivityQa2Queue] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Activities].[Qa2QueueNote]
    ADD CONSTRAINT [PK_Qa2QueueNote] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

