CREATE TABLE [Participant].[Timeline] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [ParticipantId]  NVARCHAR (9)  NOT NULL,
    [EventType]      INT           NOT NULL,
    [Line1]          NVARCHAR (50) NOT NULL,
    [Line2]          NVARCHAR (50) NULL,
    [Line3]          NVARCHAR (50) NULL,
    [Created]        DATETIME2 (7) NULL,
    [CreatedBy]      NVARCHAR (36) NULL,
    [LastModified]   DATETIME2 (7) NULL,
    [LastModifiedBy] NVARCHAR (36) NULL
);
GO

ALTER TABLE [Participant].[Timeline]
    ADD CONSTRAINT [FK_Timeline_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Participant].[Timeline]
    ADD CONSTRAINT [FK_Timeline_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Participant].[Timeline]
    ADD CONSTRAINT [PK_Timeline] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Timeline_ParticipantId]
    ON [Participant].[Timeline]([ParticipantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Timeline_CreatedBy]
    ON [Participant].[Timeline]([CreatedBy] ASC);
GO

