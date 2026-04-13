CREATE TABLE [Lookup].[ConsentStatus] (
    [Value]         INT             NOT NULL,
    [Name]          NVARCHAR (50)   NOT NULL
);
GO

ALTER TABLE [Lookup].[ConsentStatus]
    ADD CONSTRAINT [PK_ConsentStatus] PRIMARY KEY CLUSTERED ([Value] ASC)
GO

CREATE UNIQUE NONCLUSTERED INDEX [ix_ConsentStatus_Name]
    ON [Lookup].[ConsentStatus] ([Name] ASC)
GO