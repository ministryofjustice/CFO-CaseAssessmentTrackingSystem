CREATE TABLE [Configuration].[HelpLink] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Title]          NVARCHAR (150)   NOT NULL,
    [Description]    NVARCHAR (1000)  NOT NULL,
    [PageKey]        NVARCHAR (200)   NOT NULL,
    [TabName]        NVARCHAR (100)   NULL,
    [Created]        DATETIME2 (7)    NULL,
    [CreatedBy]      NVARCHAR (36)    NULL,
    [LastModified]   DATETIME2 (7)    NULL,
    [LastModifiedBy] NVARCHAR (36)    NULL
);
GO

ALTER TABLE [Configuration].[HelpLink]
    ADD CONSTRAINT [PK_HelpLink] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_HelpLink_PageKey_TabName]
    ON [Configuration].[HelpLink]([PageKey] ASC, [TabName] ASC);
GO
