CREATE TABLE [Participant].[OwnershipHistory] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [ParticipantId]  NVARCHAR (9)  NOT NULL,
    [OwnerId]        NVARCHAR (36) NULL,
    [From]           DATETIME2 (7) NOT NULL,
    [Created]        DATETIME2 (7) NULL,
    [CreatedBy]      NVARCHAR (36) NULL,
    [LastModified]   DATETIME2 (7) NULL,
    [LastModifiedBy] NVARCHAR (36) NULL,
    [TenantId]       NVARCHAR (50) NULL,
    [To]             DATETIME2 (7) NULL,
    [ContractId]     NVARCHAR (12) NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_OwnershipHistory_ParticipantId]
    ON [Participant].[OwnershipHistory]([ParticipantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_OwnershipHistory_OwnerId]
    ON [Participant].[OwnershipHistory]([OwnerId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_OwnershipHistory_TenantId]
    ON [Participant].[OwnershipHistory]([TenantId] ASC);
GO

ALTER TABLE [Participant].[OwnershipHistory]
    ADD CONSTRAINT [FK_OwnershipHistory_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Configuration].[Tenant] ([Id]);
GO

ALTER TABLE [Participant].[OwnershipHistory]
    ADD CONSTRAINT [FK_OwnershipHistory_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]);
GO

ALTER TABLE [Participant].[OwnershipHistory]
    ADD CONSTRAINT [FK_OwnershipHistory_User_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Participant].[OwnershipHistory]
    ADD CONSTRAINT [PK_OwnershipHistory] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

