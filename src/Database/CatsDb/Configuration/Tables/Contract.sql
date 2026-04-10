CREATE TABLE [Configuration].[Contract] (
    [Id]             NVARCHAR (12) NOT NULL,
    [LotNumber]      INT           NOT NULL,
    [LifetimeStart]  DATETIME2 (7) NOT NULL,
    [LifetimeEnd]    DATETIME2 (7) NOT NULL,
    [Description]    NVARCHAR (50) NOT NULL,
    [TenantId]       NVARCHAR (50) NULL,
    [Created]        DATETIME2 (7) NULL,
    [CreatedBy]      NVARCHAR (36) NULL,
    [LastModified]   DATETIME2 (7) NULL,
    [LastModifiedBy] NVARCHAR (36) NULL
);
GO

ALTER TABLE [Configuration].[Contract]
    ADD CONSTRAINT [PK_Contract] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Contract_LotNumber]
    ON [Configuration].[Contract]([LotNumber] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Contract_TenantId]
    ON [Configuration].[Contract]([TenantId] ASC);
GO

ALTER TABLE [Configuration].[Contract]
    ADD CONSTRAINT [FK_Contract_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Configuration].[Tenant] ([Id]);
GO

