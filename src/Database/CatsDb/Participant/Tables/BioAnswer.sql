CREATE TABLE [Participant].[BioAnswer] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [BioId]              UNIQUEIDENTIFIER NOT NULL,
    [QuestionCode]       NVARCHAR(3)   NOT NULL,
    [Answer]             NVARCHAR(80)  NOT NULL,

    CONSTRAINT [PK_BioAnswer]
        PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UQ_BioAnswer_BusinessKey]
ON [Participant].[BioAnswer]
(
    [BioId],
    [QuestionCode],
    [Answer]
);
GO

CREATE NONCLUSTERED INDEX [IX_BioAnswer_BioId]
ON [Participant].[BioAnswer] ([BioId]);
GO

ALTER TABLE [Participant].[BioAnswer]
ADD CONSTRAINT [FK_BioAnswer_Bio_BioId]
FOREIGN KEY ([BioId])
REFERENCES [Participant].[Bio] ([Id])
ON DELETE CASCADE;
GO
