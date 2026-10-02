CREATE TABLE [Configuration].[HelpLinkUrl] (
    [HelpLinkId]  UNIQUEIDENTIFIER NOT NULL,
    [Id]          INT              NOT NULL IDENTITY (1, 1),
    [Url]         NVARCHAR (2000)  NOT NULL,
    [DisplayName] NVARCHAR (100)   NULL
);
GO

ALTER TABLE [Configuration].[HelpLinkUrl]
    ADD CONSTRAINT [PK_HelpLinkUrl] PRIMARY KEY CLUSTERED ([HelpLinkId] ASC, [Id] ASC);
GO

ALTER TABLE [Configuration].[HelpLinkUrl]
    ADD CONSTRAINT [FK_HelpLinkUrl_HelpLink_HelpLinkId] FOREIGN KEY ([HelpLinkId]) REFERENCES [Configuration].[HelpLink] ([Id]) ON DELETE CASCADE;
GO
