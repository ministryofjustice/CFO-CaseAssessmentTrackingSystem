CREATE TABLE [Configuration].[Label] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Name]           NVARCHAR (40)    NOT NULL,
    [Description]    NVARCHAR (200)   NOT NULL,
    [Colour]         INT              NOT NULL,
    [Variant]        INT              NOT NULL,
    [Created]        DATETIME2 (7)    NULL,
    [CreatedBy]      NVARCHAR (36)    NULL,
    [LastModified]   DATETIME2 (7)    NULL,
    [LastModifiedBy] NVARCHAR (36)    NULL,
    [AppIcon]        INT              DEFAULT ((0)) NOT NULL,
    [Scope]          INT              DEFAULT ((0)) NOT NULL
);
GO

ALTER TABLE [Configuration].[Label]
    ADD CONSTRAINT [PK_Label] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

