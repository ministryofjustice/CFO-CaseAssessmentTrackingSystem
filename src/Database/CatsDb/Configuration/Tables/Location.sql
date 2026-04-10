CREATE TABLE [Configuration].[Location] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [Name]              NVARCHAR (200) NOT NULL,
    [ContractId]        NVARCHAR (12)  NULL,
    [LifetimeStart]     DATETIME2 (7)  NOT NULL,
    [LifetimeEnd]       DATETIME2 (7)  NOT NULL,
    [ParentLocationId]  INT            NULL,
    [GenderProvisionId] INT            NOT NULL,
    [LocationTypeId]    INT            NOT NULL,
    [Created]           DATETIME2 (7)  NULL,
    [CreatedBy]         NVARCHAR (36)  NULL,
    [LastModified]      DATETIME2 (7)  NULL,
    [LastModifiedBy]    NVARCHAR (36)  NULL
);
GO

ALTER TABLE [Configuration].[Location]
    ADD CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Location_ContractId]
    ON [Configuration].[Location]([ContractId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Location_ParentLocationId]
    ON [Configuration].[Location]([ParentLocationId] ASC);
GO

ALTER TABLE [Configuration].[Location]
    ADD CONSTRAINT [FK_Location_Location_ParentLocationId] FOREIGN KEY ([ParentLocationId]) REFERENCES [Configuration].[Location] ([Id]);
GO

ALTER TABLE [Configuration].[Location]
    ADD CONSTRAINT [FK_Location_Contract_ContractId] FOREIGN KEY ([ContractId]) REFERENCES [Configuration].[Contract] ([Id]);
GO

