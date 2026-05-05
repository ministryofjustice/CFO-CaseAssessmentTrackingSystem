CREATE TABLE [Configuration].[InnovationFund] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Code]           NVARCHAR (8)   NOT NULL,
    [Description]    NVARCHAR (256) NOT NULL,
    [ContractId]     NVARCHAR (12)  NOT NULL,
    [LifetimeStart]  DATETIME2 (7)  NOT NULL,
    [LifetimeEnd]    DATETIME2 (7)  NOT NULL,
    [Created]        DATETIME2 (7)  NULL,
    [CreatedBy]      NVARCHAR (36)  NULL,
    [LastModified]   DATETIME2 (7)  NULL,
    [LastModifiedBy] NVARCHAR (36)  NULL
);
GO

ALTER TABLE [Configuration].[InnovationFund]
    ADD CONSTRAINT [PK_InnovationFund] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_InnovationFund_Code]
    ON [Configuration].[InnovationFund]([Code] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_InnovationFund_ContractId]
    ON [Configuration].[InnovationFund]([ContractId] ASC);
GO

ALTER TABLE [Configuration].[InnovationFund]
    ADD CONSTRAINT [FK_InnovationFund_Contract_ContractId] FOREIGN KEY ([ContractId]) REFERENCES [Configuration].[Contract] ([Id]);
GO
