CREATE TABLE [Participant].[Bio] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [ParticipantId]  NVARCHAR (9)     NOT NULL,
    [BioJson]        NVARCHAR (MAX)   NULL,
    [Status]         INT              NOT NULL,
    [Created]        DATETIME2 (7)    NULL,
    [CreatedBy]      NVARCHAR (36)    NULL,
    [LastModified]   DATETIME2 (7)    NULL,
    [LastModifiedBy] NVARCHAR (36)    NULL,
    [Completed]      DATETIME2 (7)    NULL,
    [CompletedBy]    NVARCHAR (36)    NULL
);
GO

ALTER TABLE [Participant].[Bio]
    ADD CONSTRAINT [PK_Bio] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Bio_ParticipantId]
    ON [Participant].[Bio]([ParticipantId] ASC);
GO

ALTER TABLE [Participant].[Bio]
    ADD CONSTRAINT [FK_Bio_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

