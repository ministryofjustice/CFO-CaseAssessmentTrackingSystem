CREATE TABLE [PRI].[PriCode] (
    [ParticipantId]  NVARCHAR (9)  NOT NULL,
    [Code]           INT           NOT NULL,
    [Created]        DATETIME2 (7) NOT NULL,
    [CreatedBy]      NVARCHAR (36) NOT NULL,
    [LastModified]   DATETIME2 (7) NULL,
    [LastModifiedBy] NVARCHAR (36) NULL
);
GO

ALTER TABLE [PRI].[PriCode]
    ADD CONSTRAINT [PK_PriCode] PRIMARY KEY CLUSTERED ([ParticipantId] ASC);
GO

ALTER TABLE [PRI].[PriCode]
    ADD CONSTRAINT [FK_PriCode_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

