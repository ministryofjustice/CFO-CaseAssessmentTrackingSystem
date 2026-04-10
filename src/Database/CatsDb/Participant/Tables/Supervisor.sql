CREATE TABLE [Participant].[Supervisor] (
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [Name]            NVARCHAR (128)   NULL,
    [TelephoneNumber] NVARCHAR (16)    NULL,
    [MobileNumber]    NVARCHAR (16)    NULL,
    [EmailAddress]    NVARCHAR (100)   NULL,
    [Address]         NVARCHAR (256)   NULL,
    [ParticipantId]   NVARCHAR (9)     NOT NULL,
    [Created]         DATETIME2 (7)    NULL,
    [CreatedBy]       NVARCHAR (36)    NULL,
    [LastModified]    DATETIME2 (7)    NULL,
    [LastModifiedBy]  NVARCHAR (36)    NULL
);
GO

ALTER TABLE [Participant].[Supervisor]
    ADD CONSTRAINT [FK_Supervisor_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Participant].[Supervisor]
    ADD CONSTRAINT [PK_Supervisor] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Supervisor_ParticipantId]
    ON [Participant].[Supervisor]([ParticipantId] ASC);
GO

