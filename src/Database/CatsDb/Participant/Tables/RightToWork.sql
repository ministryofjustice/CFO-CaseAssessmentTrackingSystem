CREATE TABLE [Participant].[RightToWork] (
    [Id]             INT              IDENTITY (1, 1) NOT NULL,
    [ParticipantId]  NVARCHAR (9)     NOT NULL,
    [DocumentId]     UNIQUEIDENTIFIER NULL,
    [ValidFrom]      DATETIME2 (7)    NOT NULL,
    [ValidTo]        DATETIME2 (7)    NOT NULL,
    [Created]        DATETIME2 (7)    NULL,
    [CreatedBy]      NVARCHAR (36)    NULL,
    [LastModified]   DATETIME2 (7)    NULL,
    [LastModifiedBy] NVARCHAR (36)    NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_RightToWork_DocumentId]
    ON [Participant].[RightToWork]([DocumentId] ASC);
GO

ALTER TABLE [Participant].[RightToWork]
    ADD CONSTRAINT [FK_RightToWork_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Participant].[RightToWork]
    ADD CONSTRAINT [FK_RightToWork_Document_DocumentId] FOREIGN KEY ([DocumentId]) REFERENCES [Document].[Document] ([Id]);
GO

ALTER TABLE [Participant].[RightToWork]
    ADD CONSTRAINT [PK_RightToWork] PRIMARY KEY CLUSTERED ([ParticipantId] ASC, [Id] ASC);
GO

