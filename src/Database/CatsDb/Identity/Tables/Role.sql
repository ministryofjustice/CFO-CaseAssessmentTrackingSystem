CREATE TABLE [Identity].[Role] (
    [Id]               NVARCHAR (36)  NOT NULL,
    [Description]      NVARCHAR (200) NOT NULL,
    [Name]             NVARCHAR (50)  NOT NULL,
    [NormalizedName]   NVARCHAR (50)  NOT NULL,
    [ConcurrencyStamp] NVARCHAR (36)  NOT NULL,
    [RoleRank]         INT            DEFAULT ((0)) NOT NULL
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex]
    ON [Identity].[Role]([NormalizedName] ASC);
GO

ALTER TABLE [Identity].[Role]
    ADD CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

