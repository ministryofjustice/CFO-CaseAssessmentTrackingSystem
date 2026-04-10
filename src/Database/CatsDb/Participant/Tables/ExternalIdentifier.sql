CREATE TABLE [Participant].[ExternalIdentifier] (
    [Value]         NVARCHAR (16) NOT NULL,
    [Type]          INT           NOT NULL,
    [ParticipantId] NVARCHAR (9)  NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_ExternalIdentifier_ParticipantId]
    ON [Participant].[ExternalIdentifier]([ParticipantId] ASC);
GO

ALTER TABLE [Participant].[ExternalIdentifier]
    ADD CONSTRAINT [FK_ExternalIdentifier_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Participant].[ExternalIdentifier]
    ADD CONSTRAINT [PK_ExternalIdentifier] PRIMARY KEY CLUSTERED ([Value] ASC, [Type] ASC, [ParticipantId] ASC);
GO

