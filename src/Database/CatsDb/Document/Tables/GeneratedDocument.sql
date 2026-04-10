CREATE TABLE [Document].[GeneratedDocument] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [SearchCriteriaUsed] NVARCHAR (MAX)   NULL,
    [Status]             NVARCHAR (MAX)   NOT NULL,
    [ExpiresOn]          DATETIME2 (7)    NOT NULL,
    [Template]           NVARCHAR (256)   NOT NULL
);
GO

ALTER TABLE [Document].[GeneratedDocument]
    ADD CONSTRAINT [FK_GeneratedDocument_Document_Id] FOREIGN KEY ([Id]) REFERENCES [Document].[Document] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Document].[GeneratedDocument]
    ADD CONSTRAINT [PK_GeneratedDocument] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_GeneratedDocument_ExpiresOn]
    ON [Document].[GeneratedDocument]([ExpiresOn] ASC);
GO

