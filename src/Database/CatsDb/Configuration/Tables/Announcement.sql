CREATE TABLE [Configuration].[Announcement] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [CreatorId]           NVARCHAR (36)    NOT NULL,
    [Version]             UNIQUEIDENTIFIER NOT NULL,
    [PendingRevisionId]   UNIQUEIDENTIFIER NULL,
    [PublishedRevisionId] UNIQUEIDENTIFIER NULL,
    [DeletedAt]           DATETIME2 (7)    NULL,
    [DeletedBy]           NVARCHAR (36)    NULL,
    [Created]             DATETIME2 (7)    NULL,
    [CreatedBy]           NVARCHAR (36)    NULL,
    [LastModified]        DATETIME2 (7)    NULL,
    [LastModifiedBy]      NVARCHAR (36)    NULL,
    CONSTRAINT [PK_Announcement] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_Announcement_DeletedAt]
    ON [Configuration].[Announcement]([DeletedAt]);
GO
