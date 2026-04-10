CREATE TABLE [Participant].[Consent] (
    [Id]             INT              IDENTITY (1, 1) NOT NULL,
    [ParticipantId]  NVARCHAR (9)     NOT NULL,
    [DocumentId]     UNIQUEIDENTIFIER NOT NULL,
    [ValidFrom]      DATETIME2 (7)    NOT NULL,
    [ValidTo]        DATETIME2 (7)    NOT NULL,
    [Created]        DATETIME2 (7)    NULL,
    [CreatedBy]      NVARCHAR (36)    NULL,
    [LastModified]   DATETIME2 (7)    NULL,
    [LastModifiedBy] NVARCHAR (36)    NULL
);
GO

ALTER TABLE [Participant].[Consent]
    ADD CONSTRAINT [FK_Consent_Document_DocumentId] FOREIGN KEY ([DocumentId]) REFERENCES [Document].[Document] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Participant].[Consent]
    ADD CONSTRAINT [FK_Consent_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Participant].[Consent]
    ADD CONSTRAINT [PK_Consent] PRIMARY KEY CLUSTERED ([ParticipantId] ASC, [Id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Consent_DocumentId]
    ON [Participant].[Consent]([DocumentId] ASC);
GO

