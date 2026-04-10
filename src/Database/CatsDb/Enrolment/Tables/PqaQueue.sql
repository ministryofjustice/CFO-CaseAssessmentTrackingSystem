CREATE TABLE [Enrolment].[PqaQueue] (
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

ALTER TABLE [Enrolment].[PqaQueue]
    ADD CONSTRAINT [FK_PqaQueue_User_SupportWorkerId] FOREIGN KEY ([SupportWorkerId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Enrolment].[PqaQueue]
    ADD CONSTRAINT [FK_PqaQueue_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Configuration].[Tenant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Enrolment].[PqaQueue]
    ADD CONSTRAINT [FK_PqaQueue_User_EditorId] FOREIGN KEY ([EditorId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Enrolment].[PqaQueue]
    ADD CONSTRAINT [FK_PqaQueue_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Enrolment].[PqaQueue]
    ADD CONSTRAINT [FK_PqaQueue_User_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id]);
GO

CREATE NONCLUSTERED INDEX [IX_PqaQueue_SupportWorkerId]
    ON [Enrolment].[PqaQueue]([SupportWorkerId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_PqaQueue_ParticipantId]
    ON [Enrolment].[PqaQueue]([ParticipantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_PqaQueue_TenantId]
    ON [Enrolment].[PqaQueue]([TenantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_PqaQueue_OwnerId]
    ON [Enrolment].[PqaQueue]([OwnerId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_PqaQueue_EditorId]
    ON [Enrolment].[PqaQueue]([EditorId] ASC);
GO

ALTER TABLE [Enrolment].[PqaQueue]
    ADD CONSTRAINT [PK_PqaQueue] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

