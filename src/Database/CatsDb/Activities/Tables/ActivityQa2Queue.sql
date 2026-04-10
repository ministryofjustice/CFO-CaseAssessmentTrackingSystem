CREATE TABLE [Activities].[ActivityQa2Queue] (
    [Id]                        UNIQUEIDENTIFIER NOT NULL,
    [IsEscalated]               BIT              NOT NULL,
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

CREATE NONCLUSTERED INDEX [IX_ActivityQa2Queue_OwnerId]
    ON [Activities].[ActivityQa2Queue]([OwnerId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityQa2Queue_ActivityId]
    ON [Activities].[ActivityQa2Queue]([ActivityId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityQa2Queue_EditorId]
    ON [Activities].[ActivityQa2Queue]([EditorId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityQa2Queue_ParticipantId]
    ON [Activities].[ActivityQa2Queue]([ParticipantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityQa2Queue_TenantId]
    ON [Activities].[ActivityQa2Queue]([TenantId] ASC);
GO

ALTER TABLE [Activities].[ActivityQa2Queue]
    ADD CONSTRAINT [FK_ActivityQa2Queue_User_EditorId] FOREIGN KEY ([EditorId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[ActivityQa2Queue]
    ADD CONSTRAINT [FK_ActivityQa2Queue_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Configuration].[Tenant] ([Id]);
GO

ALTER TABLE [Activities].[ActivityQa2Queue]
    ADD CONSTRAINT [FK_ActivityQa2Queue_Activity_ActivityId] FOREIGN KEY ([ActivityId]) REFERENCES [Activities].[Activity] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Activities].[ActivityQa2Queue]
    ADD CONSTRAINT [FK_ActivityQa2Queue_User_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[ActivityQa2Queue]
    ADD CONSTRAINT [FK_ActivityQa2Queue_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]);
GO

ALTER TABLE [Activities].[ActivityQa2Queue]
    ADD CONSTRAINT [PK_ActivityQa2Queue] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

