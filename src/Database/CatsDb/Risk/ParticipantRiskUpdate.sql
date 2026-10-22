CREATE TABLE [Risk].[ParticipantRiskUpdate] (
    [Id] INT    IDENTITY(1,1) NOT NULL,
    [ParticipantId] NVARCHAR(9) NOT NULL,
    [HmppsId] NVARCHAR(50) NOT NULL,
    [EventType] NVARCHAR(100) NOT NULL,
    [DateTime] DATETIME2 (7) NOT NULL
)

GO

ALTER TABLE [Risk].[ParticipantRiskUpdate]
    ADD CONSTRAINT [PK_ParticipantRiskUpdate] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

ALTER TABLE [Risk].[ParticipantRiskUpdate]
    ADD CONSTRAINT [FK_ParticipantRiskUpdate_Participant] FOREIGN KEY (ParticipantId) REFERENCES [Participant].[Participant] (Id)  ON DELETE CASCADE;

GO

CREATE NONCLUSTERED INDEX [IX_ParticipantRiskUpdate_ParticipantId]
    ON [Risk].[ParticipantRiskUpdate] ( [ParticipantId], [DateTime] );