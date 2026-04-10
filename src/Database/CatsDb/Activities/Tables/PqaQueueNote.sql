CREATE TABLE [Activities].[PqaQueueNote] (
    [Id]                      INT              IDENTITY (1, 1) NOT NULL,
    [ActivityPqaQueueEntryId] UNIQUEIDENTIFIER NOT NULL,
    [Message]                 NVARCHAR (1000)  NOT NULL,
    [CallReference]           NVARCHAR (20)    NULL,
    [Created]                 DATETIME2 (7)    NULL,
    [CreatedBy]               NVARCHAR (36)    NULL,
    [LastModified]            DATETIME2 (7)    NULL,
    [LastModifiedBy]          NVARCHAR (36)    NULL,
    [TenantId]                NVARCHAR (50)    NOT NULL
);
GO

ALTER TABLE [Activities].[PqaQueueNote]
    ADD CONSTRAINT [FK_PqaQueueNote_User_LastModifiedBy] FOREIGN KEY ([LastModifiedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[PqaQueueNote]
    ADD CONSTRAINT [FK_PqaQueueNote_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[PqaQueueNote]
    ADD CONSTRAINT [FK_PqaQueueNote_ActivityPqaQueue_ActivityPqaQueueEntryId] FOREIGN KEY ([ActivityPqaQueueEntryId]) REFERENCES [Activities].[ActivityPqaQueue] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Activities].[PqaQueueNote]
    ADD CONSTRAINT [PK_PqaQueueNote] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_PqaQueueNote_LastModifiedBy]
    ON [Activities].[PqaQueueNote]([LastModifiedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_PqaQueueNote_ActivityPqaQueueEntryId]
    ON [Activities].[PqaQueueNote]([ActivityPqaQueueEntryId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_PqaQueueNote_CreatedBy]
    ON [Activities].[PqaQueueNote]([CreatedBy] ASC);
GO

