CREATE TABLE [Outbox].[OutboxMessage] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Type]           NVARCHAR (255)   NOT NULL,
    [Content]        NVARCHAR (MAX)   NOT NULL,
    [OccurredOnUtc]  DATETIME2 (7)    NOT NULL,
    [ProcessedOnUtc] DATETIME2 (7)    NULL,
    [Error]          NVARCHAR (MAX)   NULL,
    [ParentId]       UNIQUEIDENTIFIER NULL
);
GO

ALTER TABLE [Outbox].[OutboxMessage]
    ADD CONSTRAINT [PK_OutboxMessage] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_OutboxMessage_ProcessedOnUtc]
    ON [Outbox].[OutboxMessage]([ProcessedOnUtc] ASC);
GO

