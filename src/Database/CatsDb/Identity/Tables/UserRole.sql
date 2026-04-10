CREATE TABLE [Identity].[UserRole] (
    [UserId] NVARCHAR (36) NOT NULL,
    [RoleId] NVARCHAR (36) NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_UserRole_RoleId]
    ON [Identity].[UserRole]([RoleId] ASC);
GO

ALTER TABLE [Identity].[UserRole]
    ADD CONSTRAINT [FK_UserRole_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Identity].[UserRole]
    ADD CONSTRAINT [FK_UserRole_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Identity].[Role] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Identity].[UserRole]
    ADD CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC);
GO

