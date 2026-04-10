CREATE TABLE [Activities].[ActivityEscalationQueue] (
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

ALTER TABLE [Activities].[ActivityEscalationQueue]
    ADD CONSTRAINT [FK_ActivityEscalationQueue_User_EditorId] FOREIGN KEY ([EditorId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[ActivityEscalationQueue]
    ADD CONSTRAINT [FK_ActivityEscalationQueue_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Configuration].[Tenant] ([Id]);
GO

ALTER TABLE [Activities].[ActivityEscalationQueue]
    ADD CONSTRAINT [FK_ActivityEscalationQueue_Activity_ActivityId] FOREIGN KEY ([ActivityId]) REFERENCES [Activities].[Activity] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Activities].[ActivityEscalationQueue]
    ADD CONSTRAINT [FK_ActivityEscalationQueue_User_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Activities].[ActivityEscalationQueue]
    ADD CONSTRAINT [FK_ActivityEscalationQueue_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityEscalationQueue_ActivityId]
    ON [Activities].[ActivityEscalationQueue]([ActivityId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityEscalationQueue_EditorId]
    ON [Activities].[ActivityEscalationQueue]([EditorId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityEscalationQueue_TenantId]
    ON [Activities].[ActivityEscalationQueue]([TenantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityEscalationQueue_OwnerId]
    ON [Activities].[ActivityEscalationQueue]([OwnerId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ActivityEscalationQueue_ParticipantId]
    ON [Activities].[ActivityEscalationQueue]([ParticipantId] ASC);
GO

ALTER TABLE [Activities].[ActivityEscalationQueue]
    ADD CONSTRAINT [PK_ActivityEscalationQueue] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

