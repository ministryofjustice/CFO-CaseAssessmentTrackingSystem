CREATE TABLE [Participant].[PersonalDetails] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [PreferredNames]    NVARCHAR (128)   NULL,
    [PreferredPronouns] NVARCHAR (16)    NULL,
    [ParticipantId]     NVARCHAR (9)     NOT NULL,
    [PreferredTitle]    NVARCHAR (6)     NULL,
    [AdditionalNotes]   NVARCHAR (256)   NULL,
    [NINo]              NVARCHAR (9)     NULL
);
GO

ALTER TABLE [Participant].[PersonalDetails]
    ADD CONSTRAINT [PK_PersonalDetails] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

ALTER TABLE [Participant].[PersonalDetails]
    ADD CONSTRAINT [FK_PersonalDetails_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_PersonalDetails_ParticipantId]
    ON [Participant].[PersonalDetails]([ParticipantId] ASC);
GO

