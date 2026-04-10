CREATE TABLE [Configuration].[TenantDomain] (
    [Domain]         NVARCHAR (255) NOT NULL,
    [TenantId]       NVARCHAR (50)  NOT NULL,
    [Created]        DATETIME2 (7)  NULL,
    [CreatedBy]      NVARCHAR (36)  NULL,
    [LastModified]   DATETIME2 (7)  NULL,
    [LastModifiedBy] NVARCHAR (36)  NULL
);
GO

ALTER TABLE [Configuration].[TenantDomain]
    ADD CONSTRAINT [PK_TenantDomain] PRIMARY KEY CLUSTERED ([TenantId] ASC, [Domain] ASC);
GO

ALTER TABLE [Configuration].[TenantDomain]
    ADD CONSTRAINT [FK_TenantDomain_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Configuration].[Tenant] ([Id]) ON DELETE CASCADE;
GO

