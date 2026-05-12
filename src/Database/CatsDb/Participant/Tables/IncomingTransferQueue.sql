CREATE TABLE [Participant].[IncomingTransferQueue] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Completed]      BIT              NOT NULL,
    [Dismissed]      BIT              NOT NULL DEFAULT 0,
    [Created]        DATETIME2 (7)    NULL,
    [CreatedBy]      NVARCHAR (36)    NULL,
    [LastModified]   DATETIME2 (7)    NULL,
    [LastModifiedBy] NVARCHAR (36)    NULL,
    [MoveOccured]    DATETIME2 (7)    NOT NULL,
    [TransferType]   INT              NOT NULL,
    [ParticipantId]  NVARCHAR (9)     NOT NULL,
    [FromContractId] NVARCHAR (12)    NULL,
    [ToContractId]   NVARCHAR (12)    NULL,
    [FromLocationId] INT              NOT NULL,
    [ToLocationId]   INT              NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_IncomingTransferQueue_FromContractId]
    ON [Participant].[IncomingTransferQueue]([FromContractId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_IncomingTransferQueue_ParticipantId]
    ON [Participant].[IncomingTransferQueue]([ParticipantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_IncomingTransferQueue_Completed]
    ON [Participant].[IncomingTransferQueue]([Completed] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_IncomingTransferQueue_ToLocationId]
    ON [Participant].[IncomingTransferQueue]([ToLocationId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_IncomingTransferQueue_ToContractId]
    ON [Participant].[IncomingTransferQueue]([ToContractId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_IncomingTransferQueue_FromLocationId]
    ON [Participant].[IncomingTransferQueue]([FromLocationId] ASC);
GO

ALTER TABLE [Participant].[IncomingTransferQueue]
    ADD CONSTRAINT [FK_IncomingTransferQueue_Location_FromLocationId] FOREIGN KEY ([FromLocationId]) REFERENCES [Configuration].[Location] ([Id]);
GO

ALTER TABLE [Participant].[IncomingTransferQueue]
    ADD CONSTRAINT [FK_IncomingTransferQueue_Contract_ToContractId] FOREIGN KEY ([ToContractId]) REFERENCES [Configuration].[Contract] ([Id]);
GO

ALTER TABLE [Participant].[IncomingTransferQueue]
    ADD CONSTRAINT [FK_IncomingTransferQueue_Contract_FromContractId] FOREIGN KEY ([FromContractId]) REFERENCES [Configuration].[Contract] ([Id]);
GO

ALTER TABLE [Participant].[IncomingTransferQueue]
    ADD CONSTRAINT [FK_IncomingTransferQueue_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]);
GO

ALTER TABLE [Participant].[IncomingTransferQueue]
    ADD CONSTRAINT [FK_IncomingTransferQueue_Location_ToLocationId] FOREIGN KEY ([ToLocationId]) REFERENCES [Configuration].[Location] ([Id]);
GO

ALTER TABLE [Participant].[IncomingTransferQueue]
    ADD CONSTRAINT [PK_IncomingTransferQueue] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

