CREATE TABLE [Participant].[ActiveStatusHistory] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [From]           BIT              NOT NULL,
    [To]             BIT              NOT NULL,
    [OccurredOn]     DATE             NOT NULL,
    [ParticipantId]  NVARCHAR (9)     NOT NULL,
    [Created]        DATETIME2 (7)    NULL,
    [CreatedBy]      NVARCHAR (36)    NULL,
    [LastModified]   DATETIME2 (7)    NULL,
    [LastModifiedBy] NVARCHAR (36)    NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_ActiveStatusHistory_ParticipantId]
    ON [Participant].[ActiveStatusHistory]([ParticipantId] ASC);
GO

ALTER TABLE [Participant].[ActiveStatusHistory]
    ADD CONSTRAINT [FK_ActiveStatusHistory_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Participant].[ActiveStatusHistory]
    ADD CONSTRAINT [PK_ActiveStatusHistory] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

