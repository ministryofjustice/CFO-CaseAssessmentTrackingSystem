CREATE TABLE [Participant].[LocationHistory] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [ParticipantId]  NVARCHAR (9)  NOT NULL,
    [LocationId]     INT           NOT NULL,
    [From]           DATETIME2 (7) NOT NULL,
    [Created]        DATETIME2 (7) NULL,
    [CreatedBy]      NVARCHAR (36) NULL,
    [LastModified]   DATETIME2 (7) NULL,
    [LastModifiedBy] NVARCHAR (36) NULL,
    [To]             DATETIME2 (7) NULL
);
GO

ALTER TABLE [Participant].[LocationHistory]
    ADD CONSTRAINT [FK_LocationHistory_Location_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Configuration].[Location] ([Id]);
GO

ALTER TABLE [Participant].[LocationHistory]
    ADD CONSTRAINT [FK_LocationHistory_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]);
GO

CREATE NONCLUSTERED INDEX [IX_LocationHistory_ParticipantId]
    ON [Participant].[LocationHistory]([ParticipantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_LocationHistory_LocationId]
    ON [Participant].[LocationHistory]([LocationId] ASC);
GO

ALTER TABLE [Participant].[LocationHistory]
    ADD CONSTRAINT [PK_LocationHistory] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

