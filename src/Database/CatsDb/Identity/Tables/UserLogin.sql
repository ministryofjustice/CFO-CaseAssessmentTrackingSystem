CREATE TABLE [Identity].[UserLogin] (
    [LoginProvider]       NVARCHAR (450) NOT NULL,
    [ProviderKey]         NVARCHAR (450) NOT NULL,
    [ProviderDisplayName] NVARCHAR (MAX) NULL,
    [UserId]              NVARCHAR (36)  NOT NULL
);
GO

ALTER TABLE [Identity].[UserLogin]
    ADD CONSTRAINT [FK_UserLogin_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Identity].[UserLogin]
    ADD CONSTRAINT [PK_UserLogin] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_UserLogin_UserId]
    ON [Identity].[UserLogin]([UserId] ASC);
GO

