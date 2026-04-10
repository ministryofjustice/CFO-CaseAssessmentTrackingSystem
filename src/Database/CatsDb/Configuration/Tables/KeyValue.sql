CREATE TABLE [Configuration].[KeyValue] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (50)  NOT NULL,
    [Value]          NVARCHAR (100) NOT NULL,
    [Text]           NVARCHAR (100) NOT NULL,
    [Description]    NVARCHAR (100) NULL,
    [Created]        DATETIME2 (7)  NULL,
    [CreatedBy]      NVARCHAR (36)  NULL,
    [LastModified]   DATETIME2 (7)  NULL,
    [LastModifiedBy] NVARCHAR (36)  NULL
);
GO

ALTER TABLE [Configuration].[KeyValue]
    ADD CONSTRAINT [PK_KeyValue] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

