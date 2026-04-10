CREATE TABLE [Identity].[Notification] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [Heading]          NVARCHAR (50)    NOT NULL,
    [Details]          NVARCHAR (MAX)   NOT NULL,
    [Link]             NVARCHAR (50)    NULL,
    [ReadDate]         DATETIME2 (7)    NULL,
    [Created]          DATETIME2 (7)    NOT NULL,
    [CreatedBy]        NVARCHAR (36)    NULL,
    [LastModified]     DATETIME2 (7)    NULL,
    [LastModifiedBy]   NVARCHAR (36)    NULL,
    [OwnerId]          NVARCHAR (36)    NOT NULL,
    [EditorId]         NVARCHAR (36)    NULL,
    [NotificationDate] DATETIME2 (7)    DEFAULT ('0001-01-01T00:00:00.0000000') NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_Notification_EditorId]
    ON [Identity].[Notification]([EditorId] ASC);
GO

CREATE CLUSTERED INDEX [clst_notification]
    ON [Identity].[Notification]([NotificationDate] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Notification_CreatedBy]
    ON [Identity].[Notification]([CreatedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Notification_OwnerId_NotificationDate_ReadDate]
    ON [Identity].[Notification]([OwnerId] ASC, [NotificationDate] ASC, [ReadDate] ASC);
GO

ALTER TABLE [Identity].[Notification]
    ADD CONSTRAINT [FK_Notification_User_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Identity].[Notification]
    ADD CONSTRAINT [FK_Notification_User_EditorId] FOREIGN KEY ([EditorId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Identity].[Notification]
    ADD CONSTRAINT [FK_Notification_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Identity].[Notification]
    ADD CONSTRAINT [PK_Notification] PRIMARY KEY NONCLUSTERED ([Id] ASC);
GO

