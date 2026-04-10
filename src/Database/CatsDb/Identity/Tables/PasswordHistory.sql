CREATE TABLE [Identity].[PasswordHistory] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [UserId]       NVARCHAR (36)  NOT NULL,
    [PasswordHash] NVARCHAR (MAX) NOT NULL,
    [CreatedAt]    DATETIME2 (7)  NOT NULL
);
GO

ALTER TABLE [Identity].[PasswordHistory]
    ADD CONSTRAINT [PK_PasswordHistory] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

