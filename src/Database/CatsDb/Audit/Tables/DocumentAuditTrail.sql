CREATE TABLE [Audit].[DocumentAuditTrail] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [DocumentId]  UNIQUEIDENTIFIER NOT NULL,
    [UserId]      NVARCHAR (36)    NOT NULL,
    [RequestType] NVARCHAR (MAX)   NOT NULL,
    [OccurredOn]  DATETIME2 (7)    NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_DocumentAuditTrail_DocumentId]
    ON [Audit].[DocumentAuditTrail]([DocumentId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_DocumentAuditTrail_UserId]
    ON [Audit].[DocumentAuditTrail]([UserId] ASC);
GO

ALTER TABLE [Audit].[DocumentAuditTrail]
    ADD CONSTRAINT [PK_DocumentAuditTrail] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

ALTER TABLE [Audit].[DocumentAuditTrail]
    ADD CONSTRAINT [FK_DocumentAuditTrail_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Audit].[DocumentAuditTrail]
    ADD CONSTRAINT [FK_DocumentAuditTrail_Document_DocumentId] FOREIGN KEY ([DocumentId]) REFERENCES [Document].[Document] ([Id]) ON DELETE CASCADE;
GO

