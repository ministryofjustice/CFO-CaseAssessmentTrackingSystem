CREATE TABLE [Participant].[OutgoingTransferQueue] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [Created]          DATETIME2 (7)    NULL,
    [CreatedBy]        NVARCHAR (36)    NULL,
    [LastModified]     DATETIME2 (7)    NULL,
    [LastModifiedBy]   NVARCHAR (36)    NULL,
    [MoveOccured]      DATETIME2 (7)    NOT NULL,
    [TransferType]     INT              NOT NULL,
    [ParticipantId]    NVARCHAR (9)     NOT NULL,
    [FromContractId]   NVARCHAR (12)    NULL,
    [ToContractId]     NVARCHAR (12)    NULL,
    [FromLocationId]   INT              NOT NULL,
    [ToLocationId]     INT              NOT NULL,
    [IsReplaced]       BIT              DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [PreviousOwnerId]  NVARCHAR (36)    NULL,
    [PreviousTenantId] NVARCHAR (50)    NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_OutgoingTransferQueue_PreviousTenantId]
    ON [Participant].[OutgoingTransferQueue]([PreviousTenantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_OutgoingTransferQueue_ParticipantId]
    ON [Participant].[OutgoingTransferQueue]([ParticipantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_OutgoingTransferQueue_ToLocationId]
    ON [Participant].[OutgoingTransferQueue]([ToLocationId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_OutgoingTransferQueue_ToContractId]
    ON [Participant].[OutgoingTransferQueue]([ToContractId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_OutgoingTransferQueue_PreviousOwnerId]
    ON [Participant].[OutgoingTransferQueue]([PreviousOwnerId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_OutgoingTransferQueue_IsReplaced]
    ON [Participant].[OutgoingTransferQueue]([IsReplaced] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_OutgoingTransferQueue_FromContractId]
    ON [Participant].[OutgoingTransferQueue]([FromContractId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_OutgoingTransferQueue_FromLocationId]
    ON [Participant].[OutgoingTransferQueue]([FromLocationId] ASC);
GO

ALTER TABLE [Participant].[OutgoingTransferQueue]
    ADD CONSTRAINT [FK_OutgoingTransferQueue_Contract_FromContractId] FOREIGN KEY ([FromContractId]) REFERENCES [Configuration].[Contract] ([Id]);
GO

ALTER TABLE [Participant].[OutgoingTransferQueue]
    ADD CONSTRAINT [FK_OutgoingTransferQueue_User_PreviousOwnerId] FOREIGN KEY ([PreviousOwnerId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Participant].[OutgoingTransferQueue]
    ADD CONSTRAINT [FK_OutgoingTransferQueue_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]);
GO

ALTER TABLE [Participant].[OutgoingTransferQueue]
    ADD CONSTRAINT [FK_OutgoingTransferQueue_Location_ToLocationId] FOREIGN KEY ([ToLocationId]) REFERENCES [Configuration].[Location] ([Id]);
GO

ALTER TABLE [Participant].[OutgoingTransferQueue]
    ADD CONSTRAINT [FK_OutgoingTransferQueue_Tenant_PreviousTenantId] FOREIGN KEY ([PreviousTenantId]) REFERENCES [Configuration].[Tenant] ([Id]);
GO

ALTER TABLE [Participant].[OutgoingTransferQueue]
    ADD CONSTRAINT [FK_OutgoingTransferQueue_Contract_ToContractId] FOREIGN KEY ([ToContractId]) REFERENCES [Configuration].[Contract] ([Id]);
GO

ALTER TABLE [Participant].[OutgoingTransferQueue]
    ADD CONSTRAINT [FK_OutgoingTransferQueue_Location_FromLocationId] FOREIGN KEY ([FromLocationId]) REFERENCES [Configuration].[Location] ([Id]);
GO

ALTER TABLE [Participant].[OutgoingTransferQueue]
    ADD CONSTRAINT [PK_OutgoingTransferQueue] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

