CREATE TABLE [Participant].[InitiativeObjective] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [ObjectiveId]      UNIQUEIDENTIFIER NOT NULL,
    [InitiativeId]     UNIQUEIDENTIFIER NOT NULL,
    [ParticipantId]    NVARCHAR(9)      NOT NULL,
    [TenantId]         NVARCHAR(50)     NOT NULL,
    [Created]          DATETIME2        NULL,
    [CreatedBy]        NVARCHAR(36)     NULL,
    [LastModified]     DATETIME2        NULL,
    [LastModifiedBy]   NVARCHAR(36)     NULL,
    CONSTRAINT [PK_InitiativeObjective]            PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_InitiativeObjective_ObjectiveId] UNIQUE ([ObjectiveId]),
    CONSTRAINT [FK_InitiativeObjective_Objective]   FOREIGN KEY ([ObjectiveId])   REFERENCES [Participant].[Objective] ([Id])  ON DELETE CASCADE,
    CONSTRAINT [FK_InitiativeObjective_Initiative]  FOREIGN KEY ([InitiativeId])  REFERENCES [Configuration].[Initiative] ([Id]),
    CONSTRAINT [FK_InitiativeObjective_Participant] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]),
    CONSTRAINT [FK_InitiativeObjective_Tenant]      FOREIGN KEY ([TenantId])      REFERENCES [Configuration].[Tenant] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_InitiativeObjective_InitiativeId]
    ON [Participant].[InitiativeObjective]([InitiativeId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_InitiativeObjective_ParticipantId]
    ON [Participant].[InitiativeObjective]([ParticipantId] ASC);
GO
