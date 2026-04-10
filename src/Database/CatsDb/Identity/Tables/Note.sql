CREATE TABLE [Identity].[Note] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [Message]        NVARCHAR (255) NOT NULL,
    [CallReference]  NVARCHAR (20)  NULL,
    [Created]        DATETIME2 (7)  NULL,
    [CreatedBy]      NVARCHAR (36)  NULL,
    [LastModified]   DATETIME2 (7)  NULL,
    [LastModifiedBy] NVARCHAR (36)  NULL,
    [UserId]         NVARCHAR (36)  NOT NULL,
    [TenantId]       NVARCHAR (50)  NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_Note_CreatedBy]
    ON [Identity].[Note]([CreatedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Note_UserId]
    ON [Identity].[Note]([UserId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Note_LastModifiedBy]
    ON [Identity].[Note]([LastModifiedBy] ASC);
GO

ALTER TABLE [Identity].[Note]
    ADD CONSTRAINT [PK_Note] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

ALTER TABLE [Identity].[Note]
    ADD CONSTRAINT [FK_Note_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Identity].[Note]
    ADD CONSTRAINT [FK_Note_User_LastModifiedBy] FOREIGN KEY ([LastModifiedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Identity].[Note]
    ADD CONSTRAINT [FK_Note_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User] ([Id]) ON DELETE CASCADE;
GO

