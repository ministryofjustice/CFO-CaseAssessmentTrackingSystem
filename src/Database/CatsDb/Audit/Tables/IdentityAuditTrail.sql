CREATE TABLE [Audit].[IdentityAuditTrail] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [UserName]    NVARCHAR (100) NULL,
    [PerformedBy] NVARCHAR (100) NULL,
    [DateTime]    DATETIME2 (7)  NOT NULL,
    [ActionType]  NVARCHAR (30)  NOT NULL,
    [IpAddress]   NVARCHAR (30)  NULL
);
GO

ALTER TABLE [Audit].[IdentityAuditTrail]
    ADD CONSTRAINT [PK_IdentityAuditTrail] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [idx_IdentityAudit_UserName_DateTime]
    ON [Audit].[IdentityAuditTrail]([UserName] ASC, [DateTime] ASC);
GO

