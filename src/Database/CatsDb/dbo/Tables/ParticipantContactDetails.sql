CREATE TABLE [dbo].[ParticipantContactDetails] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [ParticipantId] NVARCHAR (9)     NOT NULL,
    [Description]   NVARCHAR (100)   NOT NULL,
    [Primary]       BIT              NOT NULL,
    [Address]       NVARCHAR (256)   NULL,
    [PostCode]      NVARCHAR (8)     NULL,
    [UPRN]          NVARCHAR (12)    NULL,
    [MobileNumber]  NVARCHAR (16)    NULL,
    [EmailAddress]  NVARCHAR (256)   NULL
);
GO

ALTER TABLE [dbo].[ParticipantContactDetails]
    ADD CONSTRAINT [FK_ParticipantContactDetails_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

CREATE NONCLUSTERED INDEX [IX_ParticipantContactDetails_ParticipantId]
    ON [dbo].[ParticipantContactDetails]([ParticipantId] ASC);
GO

ALTER TABLE [dbo].[ParticipantContactDetails]
    ADD CONSTRAINT [PK_ParticipantContactDetails] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

