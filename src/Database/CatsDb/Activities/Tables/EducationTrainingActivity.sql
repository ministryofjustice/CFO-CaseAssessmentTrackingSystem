CREATE TABLE [Activities].[EducationTrainingActivity] (
    [Id]                     UNIQUEIDENTIFIER NOT NULL,
    [CourseTitle]            NVARCHAR (200)   NOT NULL,
    [CourseUrl]              NVARCHAR (2000)  NULL,
    [CourseLevel]            NVARCHAR (100)   NOT NULL,
    [CourseCommencedOn]      DATETIME2 (7)    NOT NULL,
    [CourseCompletedOn]      DATETIME2 (7)    NULL,
    [CourseCompletionStatus] INT              NOT NULL,
    [DocumentId]             UNIQUEIDENTIFIER NOT NULL
);
GO

ALTER TABLE [Activities].[EducationTrainingActivity]
    ADD CONSTRAINT [FK_EducationTrainingActivity_Document_DocumentId] FOREIGN KEY ([DocumentId]) REFERENCES [Document].[Document] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Activities].[EducationTrainingActivity]
    ADD CONSTRAINT [FK_EducationTrainingActivity_Activity_Id] FOREIGN KEY ([Id]) REFERENCES [Activities].[Activity] ([Id]) ON DELETE CASCADE;
GO

CREATE NONCLUSTERED INDEX [IX_EducationTrainingActivity_DocumentId]
    ON [Activities].[EducationTrainingActivity]([DocumentId] ASC);
GO

ALTER TABLE [Activities].[EducationTrainingActivity]
    ADD CONSTRAINT [PK_EducationTrainingActivity] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

