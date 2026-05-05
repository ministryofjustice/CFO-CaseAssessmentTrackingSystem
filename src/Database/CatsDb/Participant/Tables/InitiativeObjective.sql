CREATE TABLE [Participant].[InitiativeObjective] (
    [ObjectiveId]   UNIQUEIDENTIFIER NOT NULL,
    [InitiativeId]  UNIQUEIDENTIFIER NOT NULL,
    [ParticipantId] NVARCHAR(9)      NOT NULL,
    CONSTRAINT [PK_InitiativeObjective] PRIMARY KEY CLUSTERED ([ObjectiveId] ASC),
    CONSTRAINT [FK_InitiativeObjective_Objective]   FOREIGN KEY ([ObjectiveId])   REFERENCES [Participant].[Objective] ([Id])             ON DELETE CASCADE,
    CONSTRAINT [FK_InitiativeObjective_Initiative]  FOREIGN KEY ([InitiativeId])  REFERENCES [Configuration].[Initiative] ([Id]),
    CONSTRAINT [FK_InitiativeObjective_Participant] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_InitiativeObjective_InitiativeId]
    ON [Participant].[InitiativeObjective]([InitiativeId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_InitiativeObjective_ParticipantId]
    ON [Participant].[InitiativeObjective]([ParticipantId] ASC);
GO
