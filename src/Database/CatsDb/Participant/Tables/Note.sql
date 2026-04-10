CREATE TABLE [Participant].[Note] (
    [Id]             INT             IDENTITY (1, 1) NOT NULL,
    [Message]        NVARCHAR (1000) NOT NULL,
    [CallReference]  NVARCHAR (20)   NULL,
    [Created]        DATETIME2 (7)   NULL,
    [CreatedBy]      NVARCHAR (36)   NULL,
    [LastModified]   DATETIME2 (7)   NULL,
    [LastModifiedBy] NVARCHAR (36)   NULL,
    [ParticipantId]  NVARCHAR (9)    NOT NULL,
    [TenantId]       NVARCHAR (50)   NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_Note_CreatedBy]
    ON [Participant].[Note]([CreatedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Note_LastModifiedBy]
    ON [Participant].[Note]([LastModifiedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Note_ParticipantId]
    ON [Participant].[Note]([ParticipantId] ASC);
GO

ALTER TABLE [Participant].[Note]
    ADD CONSTRAINT [FK_Note_User_LastModifiedBy] FOREIGN KEY ([LastModifiedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Participant].[Note]
    ADD CONSTRAINT [FK_Note_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Participant].[Note]
    ADD CONSTRAINT [FK_Note_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Participant].[Note]
    ADD CONSTRAINT [PK_Note] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

