CREATE TABLE [dbo].[DataProtectionKeys] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [FriendlyName] NVARCHAR (MAX) NULL,
    [Xml]          NVARCHAR (MAX) NULL
);
GO

ALTER TABLE [dbo].[DataProtectionKeys]
    ADD CONSTRAINT [PK_DataProtectionKeys] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

