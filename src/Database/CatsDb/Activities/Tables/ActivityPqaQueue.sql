CREATE TABLE [Activities].[ActivityPqaQueue] (
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

CREATE NONCLUSTERED INDEX [IX_ActivityPqaQueue_OwnerId]
    ON [Activities].[ActivityPqaQueue]([OwnerId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityPqaQueue_ParticipantId]
    ON [Activities].[ActivityPqaQueue]([ParticipantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityPqaQueue_ActivityId]
    ON [Activities].[ActivityPqaQueue]([ActivityId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityPqaQueue_EditorId]
    ON [Activities].[ActivityPqaQueue]([EditorId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityPqaQueue_TenantId]
    ON [Activities].[ActivityPqaQueue]([TenantId] ASC);
GO

ALTER TABLE [Activities].[ActivityPqaQueue]
    ADD CONSTRAINT [FK_ActivityPqaQueue_User_EditorId] FOREIGN KEY ([EditorId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[ActivityPqaQueue]
    ADD CONSTRAINT [FK_ActivityPqaQueue_Activity_ActivityId] FOREIGN KEY ([ActivityId]) REFERENCES [Activities].[Activity] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Activities].[ActivityPqaQueue]
    ADD CONSTRAINT [FK_ActivityPqaQueue_User_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[ActivityPqaQueue]
    ADD CONSTRAINT [FK_ActivityPqaQueue_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]);
GO

ALTER TABLE [Activities].[ActivityPqaQueue]
    ADD CONSTRAINT [FK_ActivityPqaQueue_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Configuration].[Tenant] ([Id]);
GO

ALTER TABLE [Activities].[ActivityPqaQueue]
    ADD CONSTRAINT [PK_ActivityPqaQueue] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

