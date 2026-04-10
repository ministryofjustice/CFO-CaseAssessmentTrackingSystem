CREATE TABLE [Enrolment].[Qa1Queue] (
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

ALTER TABLE [Enrolment].[Qa1Queue]
    ADD CONSTRAINT [FK_Qa1Queue_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Configuration].[Tenant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Enrolment].[Qa1Queue]
    ADD CONSTRAINT [FK_Qa1Queue_User_EditorId] FOREIGN KEY ([EditorId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Enrolment].[Qa1Queue]
    ADD CONSTRAINT [FK_Qa1Queue_User_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Enrolment].[Qa1Queue]
    ADD CONSTRAINT [FK_Qa1Queue_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Enrolment].[Qa1Queue]
    ADD CONSTRAINT [FK_Qa1Queue_User_SupportWorkerId] FOREIGN KEY ([SupportWorkerId]) REFERENCES [Identity].[User] ([Id]);
GO

CREATE NONCLUSTERED INDEX [IX_Qa1Queue_EditorId]
    ON [Enrolment].[Qa1Queue]([EditorId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Qa1Queue_ParticipantId]
    ON [Enrolment].[Qa1Queue]([ParticipantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Qa1Queue_OwnerId]
    ON [Enrolment].[Qa1Queue]([OwnerId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Qa1Queue_TenantId]
    ON [Enrolment].[Qa1Queue]([TenantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Qa1Queue_SupportWorkerId]
    ON [Enrolment].[Qa1Queue]([SupportWorkerId] ASC);
GO

ALTER TABLE [Enrolment].[Qa1Queue]
    ADD CONSTRAINT [PK_Qa1Queue] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

