CREATE TABLE [Document].[Document] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Title]          NVARCHAR (MAX)   NULL,
    [Description]    NVARCHAR (MAX)   NULL,
    [Content]        NVARCHAR (4000)  NULL,
    [IsPublic]       BIT              NOT NULL,
    [URL]            NVARCHAR (MAX)   NULL,
    [DocumentType]   NVARCHAR (MAX)   NOT NULL,
    [TenantId]       NVARCHAR (50)    NULL,
    [Created]        DATETIME2 (7)    NULL,
    [CreatedBy]      NVARCHAR (36)    NULL,
    [LastModified]   DATETIME2 (7)    NULL,
    [LastModifiedBy] NVARCHAR (36)    NULL,
    [OwnerId]        NVARCHAR (36)    NULL,
    [EditorId]       NVARCHAR (36)    NULL,
    [Version]        NVARCHAR (5)     NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_Document_TenantId]
    ON [Document].[Document]([TenantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Document_CreatedBy]
    ON [Document].[Document]([CreatedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Document_LastModifiedBy]
    ON [Document].[Document]([LastModifiedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Document_Created_CreatedBy]
    ON [Document].[Document]([Created] ASC, [CreatedBy] ASC);
GO

ALTER TABLE [Document].[Document]
    ADD CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

ALTER TABLE [Document].[Document]
    ADD CONSTRAINT [FK_Document_User_LastModifiedBy] FOREIGN KEY ([LastModifiedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Document].[Document]
    ADD CONSTRAINT [FK_Document_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Document].[Document]
    ADD CONSTRAINT [FK_Document_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Configuration].[Tenant] ([Id]);
GO

