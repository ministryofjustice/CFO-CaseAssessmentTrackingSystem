CREATE TABLE [Enrolment].[EscalationQueue] (
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [Created]         DATETIME2 (7)    NULL,
    [CreatedBy]       NVARCHAR (MAX)   NULL,
    [LastModified]    DATETIME2 (7)    NULL,
    [LastModifiedBy]  NVARCHAR (MAX)   NULL,
    [OwnerId]         NVARCHAR (36)    NULL,
    [EditorId]        NVARCHAR (36)    NULL,
    [IsAccepted]      BIT              NOT NULL,
    [IsCompleted]     BIT              NOT NULL,
    [ParticipantId]   NVARCHAR (9)     NOT NULL,
    [TenantId]        NVARCHAR (50)    NOT NULL,
    [SupportWorkerId] NVARCHAR (36)    DEFAULT (N'') NOT NULL,
    [ConsentDate]     DATE             DEFAULT ('0001-01-01') NOT NULL
);
GO

ALTER TABLE [Enrolment].[EscalationQueue]
    ADD CONSTRAINT [FK_EscalationQueue_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Configuration].[Tenant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Enrolment].[EscalationQueue]
    ADD CONSTRAINT [FK_EscalationQueue_User_SupportWorkerId] FOREIGN KEY ([SupportWorkerId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Enrolment].[EscalationQueue]
    ADD CONSTRAINT [FK_EscalationQueue_User_EditorId] FOREIGN KEY ([EditorId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Enrolment].[EscalationQueue]
    ADD CONSTRAINT [FK_EscalationQueue_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Enrolment].[EscalationQueue]
    ADD CONSTRAINT [FK_EscalationQueue_User_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id]);
GO

CREATE NONCLUSTERED INDEX [IX_EscalationQueue_ParticipantId]
    ON [Enrolment].[EscalationQueue]([ParticipantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_EscalationQueue_SupportWorkerId]
    ON [Enrolment].[EscalationQueue]([SupportWorkerId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_EscalationQueue_TenantId]
    ON [Enrolment].[EscalationQueue]([TenantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_EscalationQueue_EditorId]
    ON [Enrolment].[EscalationQueue]([EditorId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_EscalationQueue_OwnerId]
    ON [Enrolment].[EscalationQueue]([OwnerId] ASC);
GO

ALTER TABLE [Enrolment].[EscalationQueue]
    ADD CONSTRAINT [PK_EscalationQueue] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

