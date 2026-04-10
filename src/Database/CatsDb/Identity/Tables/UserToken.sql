CREATE TABLE [Identity].[UserToken] (
    [UserId]        NVARCHAR (36)  NOT NULL,
    [LoginProvider] NVARCHAR (450) NOT NULL,
    [Name]          NVARCHAR (450) NOT NULL,
    [Value]         NVARCHAR (MAX) NULL
);
GO

ALTER TABLE [Identity].[UserToken]
    ADD CONSTRAINT [PK_UserToken] PRIMARY KEY CLUSTERED ([UserId] ASC, [LoginProvider] ASC, [Name] ASC);
GO

ALTER TABLE [Identity].[UserToken]
    ADD CONSTRAINT [FK_UserToken_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User] ([Id]) ON DELETE CASCADE;
GO

