CREATE TABLE [Identity].[UserClaim] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Description] NVARCHAR (200) NOT NULL,
    [UserId]      NVARCHAR (36)  NOT NULL,
    [ClaimType]   NVARCHAR (MAX) NULL,
    [ClaimValue]  NVARCHAR (MAX) NULL
);
GO

ALTER TABLE [Identity].[UserClaim]
    ADD CONSTRAINT [PK_UserClaim] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

ALTER TABLE [Identity].[UserClaim]
    ADD CONSTRAINT [FK_UserClaim_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User] ([Id]) ON DELETE CASCADE;
GO

CREATE NONCLUSTERED INDEX [IX_UserClaim_UserId]
    ON [Identity].[UserClaim]([UserId] ASC);
GO

