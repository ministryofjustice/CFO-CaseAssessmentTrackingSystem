CREATE TABLE [Audit].[AuditTrail] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [UserId]          NVARCHAR (36)  NULL,
    [AuditType]       NVARCHAR (MAX) NOT NULL,
    [TableName]       NVARCHAR (MAX) NULL,
    [DateTime]        DATETIME2 (7)  NOT NULL,
    [OldValues]       NVARCHAR (MAX) NULL,
    [NewValues]       NVARCHAR (MAX) NULL,
    [AffectedColumns] NVARCHAR (MAX) NULL,
    [PrimaryKey]      NVARCHAR (150) NOT NULL
);
GO

ALTER TABLE [Audit].[AuditTrail]
    ADD CONSTRAINT [FK_AuditTrail_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User] ([Id]) ON DELETE SET NULL;
GO

CREATE NONCLUSTERED INDEX [IX_AuditTrail_PrimaryKey]
    ON [Audit].[AuditTrail]([PrimaryKey] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_AuditTrail_UserId]
    ON [Audit].[AuditTrail]([UserId] ASC);
GO

ALTER TABLE [Audit].[AuditTrail]
    ADD CONSTRAINT [PK_AuditTrail] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

