CREATE TABLE [Induction].[WingInduction] (
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

ALTER TABLE [Induction].[WingInduction]
    ADD CONSTRAINT [FK_WingInduction_Location_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Configuration].[Location] ([Id]);
GO

ALTER TABLE [Induction].[WingInduction]
    ADD CONSTRAINT [FK_WingInduction_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Induction].[WingInduction]
    ADD CONSTRAINT [FK_WingInduction_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]);
GO

ALTER TABLE [Induction].[WingInduction]
    ADD CONSTRAINT [FK_WingInduction_User_EditorId] FOREIGN KEY ([EditorId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Induction].[WingInduction]
    ADD CONSTRAINT [FK_WingInduction_User_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id]);
GO

CREATE NONCLUSTERED INDEX [IX_WingInduction_LocationId]
    ON [Induction].[WingInduction]([LocationId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_WingInduction_EditorId]
    ON [Induction].[WingInduction]([EditorId] ASC);
GO

CREATE CLUSTERED INDEX [IX_WingInduction_ParticipantId_Created]
    ON [Induction].[WingInduction]([ParticipantId] ASC, [Created] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_WingInduction_CreatedBy]
    ON [Induction].[WingInduction]([CreatedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_WingInduction_OwnerId]
    ON [Induction].[WingInduction]([OwnerId] ASC);
GO

ALTER TABLE [Induction].[WingInduction]
    ADD CONSTRAINT [PK_WingInduction] PRIMARY KEY NONCLUSTERED ([Id] ASC);
GO

