CREATE TABLE [Configuration].[LabelContract] (
    [LabelId]    UNIQUEIDENTIFIER NOT NULL,
    [ContractId] NVARCHAR (12)    NOT NULL
);
GO

ALTER TABLE [Configuration].[LabelContract]
    ADD CONSTRAINT [PK_LabelContract] PRIMARY KEY CLUSTERED ([LabelId] ASC, [ContractId] ASC);
GO

ALTER TABLE [Configuration].[LabelContract]
    ADD CONSTRAINT [FK_LabelContract_Label_LabelId] FOREIGN KEY ([LabelId]) REFERENCES [Configuration].[Label] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Configuration].[LabelContract]
    ADD CONSTRAINT [FK_LabelContract_Contract_ContractId] FOREIGN KEY ([ContractId]) REFERENCES [Configuration].[Contract] ([Id]) ON DELETE CASCADE;
GO

CREATE NONCLUSTERED INDEX [IX_LabelContract_ContractId]
    ON [Configuration].[LabelContract]([ContractId] ASC);
GO
