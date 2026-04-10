CREATE TABLE [Induction].[HubInduction] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [ParticipantId]  NVARCHAR (9)     NOT NULL,
    [LocationId]     INT              NOT NULL,
    [InductionDate]  DATE             NOT NULL,
    [Created]        DATETIME2 (7)    NOT NULL,
    [CreatedBy]      NVARCHAR (36)    NOT NULL,
    [LastModified]   DATETIME2 (7)    NULL,
    [LastModifiedBy] NVARCHAR (MAX)   NULL,
    [OwnerId]        NVARCHAR (36)    NOT NULL,
    [EditorId]       NVARCHAR (36)    NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_HubInduction_EditorId]
    ON [Induction].[HubInduction]([EditorId] ASC);
GO

CREATE CLUSTERED INDEX [IX_HubInduction_ParticipantId_Created]
    ON [Induction].[HubInduction]([ParticipantId] ASC, [Created] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_HubInduction_CreatedBy]
    ON [Induction].[HubInduction]([CreatedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_HubInduction_OwnerId]
    ON [Induction].[HubInduction]([OwnerId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_HubInduction_LocationId]
    ON [Induction].[HubInduction]([LocationId] ASC);
GO

ALTER TABLE [Induction].[HubInduction]
    ADD CONSTRAINT [PK_HubInduction] PRIMARY KEY NONCLUSTERED ([Id] ASC);
GO

ALTER TABLE [Induction].[HubInduction]
    ADD CONSTRAINT [FK_HubInduction_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Induction].[HubInduction]
    ADD CONSTRAINT [FK_HubInduction_User_EditorId] FOREIGN KEY ([EditorId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Induction].[HubInduction]
    ADD CONSTRAINT [FK_HubInduction_User_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Induction].[HubInduction]
    ADD CONSTRAINT [FK_HubInduction_Location_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Configuration].[Location] ([Id]);
GO

ALTER TABLE [Induction].[HubInduction]
    ADD CONSTRAINT [FK_HubInduction_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]);
GO

