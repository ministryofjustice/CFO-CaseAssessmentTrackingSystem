CREATE TABLE [Configuration].[AnnouncementType] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Name]           NVARCHAR (100)   NOT NULL,
    [Created]        DATETIME2 (7)    NULL,
    [CreatedBy]      NVARCHAR (36)    NULL,
    [LastModified]   DATETIME2 (7)    NULL,
    [LastModifiedBy] NVARCHAR (36)    NULL,
    CONSTRAINT [PK_AnnouncementType] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [UQ_AnnouncementType_Name] UNIQUE ([Name])
);
GO
