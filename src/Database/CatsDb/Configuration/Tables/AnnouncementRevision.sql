CREATE TABLE [Configuration].[AnnouncementRevision] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [AnnouncementId] UNIQUEIDENTIFIER NOT NULL,
    [TypeName]       NVARCHAR (100)    NOT NULL,
    [Audience]       INT              NOT NULL,
    [Title]          NVARCHAR (150)    NOT NULL,
    [PostedBy]       NVARCHAR (150)    NOT NULL,
    [SubmittedByName] NVARCHAR (256)   NOT NULL,
    [Description]    NVARCHAR (MAX)   NOT NULL,
    [SubmittedBy]    NVARCHAR (36)    NOT NULL,
    [SubmittedAt]    DATETIME2 (7)    NOT NULL,
    [ApprovedBy]     NVARCHAR (36)    NULL,
    [PublishedAt]    DATETIME2 (7)    NULL,
    [AutoApproved]   BIT              NOT NULL,
    CONSTRAINT [PK_AnnouncementRevision] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_AnnouncementRevision_Announcement] FOREIGN KEY ([AnnouncementId])
        REFERENCES [Configuration].[Announcement] ([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_AnnouncementRevision_AnnouncementId]
    ON [Configuration].[AnnouncementRevision]([AnnouncementId]);
GO
