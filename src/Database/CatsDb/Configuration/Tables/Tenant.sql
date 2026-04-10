CREATE TABLE [Configuration].[Tenant] (
    [Id]             NVARCHAR (50)  NOT NULL,
    [Name]           NVARCHAR (50)  NOT NULL,
    [Description]    NVARCHAR (150) NOT NULL,
    [Created]        DATETIME2 (7)  NULL,
    [CreatedBy]      NVARCHAR (36)  NULL,
    [LastModified]   DATETIME2 (7)  NULL,
    [LastModifiedBy] NVARCHAR (36)  NULL,
    [ContractId]     NVARCHAR (12)  NULL
);
GO

ALTER TABLE [Configuration].[Tenant]
    ADD CONSTRAINT [PK_Tenant] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

