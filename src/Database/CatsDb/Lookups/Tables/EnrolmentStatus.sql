CREATE TABLE [Lookup].[EnrolmentStatus] (
    [Value]         INT             NOT NULL,
    [Name]          NVARCHAR (50)   NOT NULL,
    [LogicalOrder]  INT             NOT NULL
);
GO

ALTER TABLE [Lookup].[EnrolmentStatus]
    ADD CONSTRAINT [PK_EnrolmentStatus] PRIMARY KEY CLUSTERED ([Value] ASC)
GO

CREATE UNIQUE NONCLUSTERED INDEX [ix_EnrolmentStatus_Name]
    ON [Lookup].[EnrolmentStatus] ([Name] ASC)
GO