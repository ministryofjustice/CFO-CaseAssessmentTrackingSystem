CREATE TABLE [Participant].[EnrolmentHistory] (
    [Id]                    INT             IDENTITY (1, 1) NOT NULL,
    [ParticipantId]         NVARCHAR (9)    NOT NULL,
    [EnrolmentStatus]       INT             NOT NULL,
    [Created]               DATETIME2 (7)   NULL,
    [CreatedBy]             NVARCHAR (36)   NULL,
    [LastModified]          DATETIME2 (7)   NULL,
    [LastModifiedBy]        NVARCHAR (36)   NULL,
    [AdditionalInformation] NVARCHAR (1000) NULL,
    [Reason]                NVARCHAR (1000) NULL,
    [From]                  DATETIME2 (7)   DEFAULT ('0001-01-01T00:00:00.0000000') NOT NULL,
    [To]                    DATETIME2 (7)   NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_EnrolmentHistory_ParticipantId]
    ON [Participant].[EnrolmentHistory]([ParticipantId] ASC);
GO

ALTER TABLE [Participant].[EnrolmentHistory]
    ADD CONSTRAINT [FK_EnrolmentHistory_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Participant].[EnrolmentHistory]
    ADD CONSTRAINT [PK_EnrolmentHistory] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

