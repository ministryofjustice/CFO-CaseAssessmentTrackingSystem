CREATE TABLE [Participant].[Assessment] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [ParticipantId]  NVARCHAR (9)     NOT NULL,
    [AssessmentJson] NVARCHAR (MAX)   NULL,
    [TenantId]       NVARCHAR (50)    NULL,
    [Created]        DATETIME2 (7)    NULL,
    [CreatedBy]      NVARCHAR (36)    NULL,
    [LastModified]   DATETIME2 (7)    NULL,
    [LastModifiedBy] NVARCHAR (36)    NULL,
    [OwnerId]        NVARCHAR (36)    NULL,
    [EditorId]       NVARCHAR (36)    NULL,
    [Completed]      DATETIME2 (7)    NULL,
    [CompletedBy]    NVARCHAR (36)    NULL,
    [LocationId]     INT              DEFAULT ((0)) NOT NULL
);
GO

ALTER TABLE [Participant].[Assessment]
    ADD CONSTRAINT [PK_Assessment] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Assessment_TenantId]
    ON [Participant].[Assessment]([TenantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Assessment_OwnerId]
    ON [Participant].[Assessment]([OwnerId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Assessment_ParticipantId]
    ON [Participant].[Assessment]([ParticipantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Assessment_EditorId]
    ON [Participant].[Assessment]([EditorId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Assessment_LocationId]
    ON [Participant].[Assessment]([LocationId] ASC);
GO

ALTER TABLE [Participant].[Assessment]
    ADD CONSTRAINT [FK_Assessment_User_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Participant].[Assessment]
    ADD CONSTRAINT [FK_Assessment_User_EditorId] FOREIGN KEY ([EditorId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Participant].[Assessment]
    ADD CONSTRAINT [FK_Assessment_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Participant].[Assessment]
    ADD CONSTRAINT [FK_Assessment_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Configuration].[Tenant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Participant].[Assessment]
    ADD CONSTRAINT [FK_Assessment_Location_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Configuration].[Location] ([Id]);
GO

