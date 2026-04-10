CREATE TABLE [Activities].[ActivityQa1Queue] (
    [Id]                        UNIQUEIDENTIFIER NOT NULL,
    [Created]                   DATETIME2 (7)    NULL,
    [CreatedBy]                 NVARCHAR (MAX)   NULL,
    [LastModified]              DATETIME2 (7)    NULL,
    [LastModifiedBy]            NVARCHAR (MAX)   NULL,
    [OwnerId]                   NVARCHAR (36)    NULL,
    [EditorId]                  NVARCHAR (36)    NULL,
    [IsAccepted]                BIT              NOT NULL,
    [IsCompleted]               BIT              NOT NULL,
    [ActivityId]                UNIQUEIDENTIFIER NOT NULL,
    [ParticipantId]             NVARCHAR (9)     NULL,
    [TenantId]                  NVARCHAR (50)    NOT NULL,
    [OriginalPQASubmissionDate] DATE             DEFAULT ('0001-01-01') NOT NULL,
    [SupportWorkerId]           NVARCHAR (36)    DEFAULT (N'') NOT NULL
);
GO

ALTER TABLE [Activities].[ActivityQa1Queue]
    ADD CONSTRAINT [FK_ActivityQa1Queue_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Configuration].[Tenant] ([Id]);
GO

ALTER TABLE [Activities].[ActivityQa1Queue]
    ADD CONSTRAINT [FK_ActivityQa1Queue_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]);
GO

ALTER TABLE [Activities].[ActivityQa1Queue]
    ADD CONSTRAINT [FK_ActivityQa1Queue_User_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[ActivityQa1Queue]
    ADD CONSTRAINT [FK_ActivityQa1Queue_User_EditorId] FOREIGN KEY ([EditorId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[ActivityQa1Queue]
    ADD CONSTRAINT [FK_ActivityQa1Queue_Activity_ActivityId] FOREIGN KEY ([ActivityId]) REFERENCES [Activities].[Activity] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Activities].[ActivityQa1Queue]
    ADD CONSTRAINT [PK_ActivityQa1Queue] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityQa1Queue_ParticipantId]
    ON [Activities].[ActivityQa1Queue]([ParticipantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityQa1Queue_TenantId]
    ON [Activities].[ActivityQa1Queue]([TenantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityQa1Queue_OwnerId]
    ON [Activities].[ActivityQa1Queue]([OwnerId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityQa1Queue_ActivityId]
    ON [Activities].[ActivityQa1Queue]([ActivityId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityQa1Queue_EditorId]
    ON [Activities].[ActivityQa1Queue]([EditorId] ASC);
GO

