CREATE TABLE [Configuration].[AnnouncementLink] (
    [RevisionId]  UNIQUEIDENTIFIER NOT NULL,
    [Id]          INT IDENTITY (1,1) NOT NULL,
    [Url]         NVARCHAR (2000)   NOT NULL,
    [DisplayName] NVARCHAR (150)    NULL,
    CONSTRAINT [PK_AnnouncementLink] PRIMARY KEY CLUSTERED ([RevisionId], [Id]),
    CONSTRAINT [FK_AnnouncementLink_Revision] FOREIGN KEY ([RevisionId])
        REFERENCES [Configuration].[AnnouncementRevision] ([Id]) ON DELETE CASCADE
);
GO
