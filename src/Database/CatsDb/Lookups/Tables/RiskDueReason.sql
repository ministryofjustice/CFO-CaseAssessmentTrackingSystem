CREATE TABLE [Lookup].[RiskDueReason] (
    [Value]         INT             NOT NULL,
    [Name]          NVARCHAR (50)   NOT NULL
);
GO

ALTER TABLE [Lookup].[RiskDueReason]
    ADD CONSTRAINT [PK_RiskDueReason] PRIMARY KEY CLUSTERED ([Value] ASC)
GO

CREATE UNIQUE NONCLUSTERED INDEX [ix_RiskDueReason_Name]
    ON [Lookup].[RiskDueReason] ([Name] ASC)
GO