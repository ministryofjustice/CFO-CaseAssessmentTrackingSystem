CREATE TABLE [Identity].[RoleClaim] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Description] NVARCHAR (200) NOT NULL,
    [Group]       NVARCHAR (50)  NOT NULL,
    [RoleId]      NVARCHAR (36)  NOT NULL,
    [ClaimType]   NVARCHAR (MAX) NOT NULL,
    [ClaimValue]  NVARCHAR (MAX) NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_RoleClaim_RoleId]
    ON [Identity].[RoleClaim]([RoleId] ASC);
GO

ALTER TABLE [Identity].[RoleClaim]
    ADD CONSTRAINT [PK_RoleClaim] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

ALTER TABLE [Identity].[RoleClaim]
    ADD CONSTRAINT [FK_RoleClaim_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Identity].[Role] ([Id]) ON DELETE CASCADE;
GO

