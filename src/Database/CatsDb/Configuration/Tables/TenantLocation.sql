CREATE TABLE [Configuration].[TenantLocation] (
    [LocationId] INT           NOT NULL,
    [TenantId]   NVARCHAR (50) NOT NULL
);
GO

ALTER TABLE [Configuration].[TenantLocation]
    ADD CONSTRAINT [FK_TenantLocation_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Configuration].[Tenant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Configuration].[TenantLocation]
    ADD CONSTRAINT [FK_TenantLocation_Location_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Configuration].[Location] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Configuration].[TenantLocation]
    ADD CONSTRAINT [PK_TenantLocation] PRIMARY KEY CLUSTERED ([LocationId] ASC, [TenantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_TenantLocation_TenantId]
    ON [Configuration].[TenantLocation]([TenantId] ASC);
GO

