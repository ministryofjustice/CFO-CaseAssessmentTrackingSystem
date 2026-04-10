CREATE TABLE [Participant].[Label] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [LabelId]        UNIQUEIDENTIFIER NOT NULL,
    [LifetimeStart]  DATETIME2 (7)    NOT NULL,
    [LifetimeEnd]    DATETIME2 (7)    NOT NULL,
    [ParticipantId]  NVARCHAR (9)     NOT NULL,
    [Created]        DATETIME2 (7)    NOT NULL,
    [CreatedBy]      NVARCHAR (36)    NOT NULL,
    [LastModified]   DATETIME2 (7)    NULL,
    [LastModifiedBy] NVARCHAR (36)    NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_Label_LabelId]
    ON [Participant].[Label]([LabelId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Label_ParticipantId]
    ON [Participant].[Label]([ParticipantId] ASC);
GO

ALTER TABLE [Participant].[Label]
    ADD CONSTRAINT [FK_Label_Label_LabelId] FOREIGN KEY ([LabelId]) REFERENCES [Configuration].[Label] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Participant].[Label]
    ADD CONSTRAINT [FK_Label_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Participant].[Label]
    ADD CONSTRAINT [PK_Label] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

