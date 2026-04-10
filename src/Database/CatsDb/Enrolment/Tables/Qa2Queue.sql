CREATE TABLE [Enrolment].[Qa2Queue] (
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
    [IsEscalated]     BIT              DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [SupportWorkerId] NVARCHAR (36)    DEFAULT (N'') NOT NULL,
    [ConsentDate]     DATE             DEFAULT ('0001-01-01') NOT NULL
);
GO

ALTER TABLE [Enrolment].[Qa2Queue]
    ADD CONSTRAINT [FK_Qa2Queue_User_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Enrolment].[Qa2Queue]
    ADD CONSTRAINT [FK_Qa2Queue_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Enrolment].[Qa2Queue]
    ADD CONSTRAINT [FK_Qa2Queue_User_SupportWorkerId] FOREIGN KEY ([SupportWorkerId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Enrolment].[Qa2Queue]
    ADD CONSTRAINT [FK_Qa2Queue_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Configuration].[Tenant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Enrolment].[Qa2Queue]
    ADD CONSTRAINT [FK_Qa2Queue_User_EditorId] FOREIGN KEY ([EditorId]) REFERENCES [Identity].[User] ([Id]);
GO

CREATE NONCLUSTERED INDEX [IX_Qa2Queue_TenantId]
    ON [Enrolment].[Qa2Queue]([TenantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Qa2Queue_SupportWorkerId]
    ON [Enrolment].[Qa2Queue]([SupportWorkerId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Qa2Queue_OwnerId]
    ON [Enrolment].[Qa2Queue]([OwnerId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Qa2Queue_ParticipantId]
    ON [Enrolment].[Qa2Queue]([ParticipantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Qa2Queue_EditorId]
    ON [Enrolment].[Qa2Queue]([EditorId] ASC);
GO

ALTER TABLE [Enrolment].[Qa2Queue]
    ADD CONSTRAINT [PK_Qa2Queue] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

