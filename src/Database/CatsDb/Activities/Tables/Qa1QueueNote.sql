CREATE TABLE [Activities].[Qa1QueueNote] (
    [Id]                      INT              IDENTITY (1, 1) NOT NULL,
    [IsExternal]              BIT              NOT NULL,
    [ActivityQa1QueueEntryId] UNIQUEIDENTIFIER NOT NULL,
    [Message]                 NVARCHAR (1000)  NOT NULL,
    [CallReference]           NVARCHAR (20)    NULL,
    [Created]                 DATETIME2 (7)    NULL,
    [CreatedBy]               NVARCHAR (36)    NULL,
    [LastModified]            DATETIME2 (7)    NULL,
    [LastModifiedBy]          NVARCHAR (36)    NULL,
    [TenantId]                NVARCHAR (50)    NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_Qa1QueueNote_ActivityQa1QueueEntryId]
    ON [Activities].[Qa1QueueNote]([ActivityQa1QueueEntryId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Qa1QueueNote_CreatedBy]
    ON [Activities].[Qa1QueueNote]([CreatedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Qa1QueueNote_LastModifiedBy]
    ON [Activities].[Qa1QueueNote]([LastModifiedBy] ASC);
GO

ALTER TABLE [Activities].[Qa1QueueNote]
    ADD CONSTRAINT [FK_Qa1QueueNote_ActivityQa1Queue_ActivityQa1QueueEntryId] FOREIGN KEY ([ActivityQa1QueueEntryId]) REFERENCES [Activities].[ActivityQa1Queue] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Activities].[Qa1QueueNote]
    ADD CONSTRAINT [FK_Qa1QueueNote_User_LastModifiedBy] FOREIGN KEY ([LastModifiedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[Qa1QueueNote]
    ADD CONSTRAINT [FK_Qa1QueueNote_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[Qa1QueueNote]
    ADD CONSTRAINT [PK_Qa1QueueNote] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

