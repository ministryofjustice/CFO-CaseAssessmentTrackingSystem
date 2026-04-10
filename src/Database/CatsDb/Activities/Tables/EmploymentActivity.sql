CREATE TABLE [Activities].[EmploymentActivity] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [EmploymentType]      NVARCHAR (100)   NOT NULL,
    [EmployerName]        NVARCHAR (64)    NOT NULL,
    [JobTitle]            NVARCHAR (MAX)   NOT NULL,
    [JobTitleCode]        NVARCHAR (MAX)   NOT NULL,
    [Salary]              FLOAT (53)       NULL,
    [EmploymentCommenced] DATETIME2 (7)    NOT NULL,
    [DocumentId]          UNIQUEIDENTIFIER NOT NULL,
    [SalaryFrequency]     NVARCHAR (100)   NULL
);
GO

ALTER TABLE [Activities].[EmploymentActivity]
    ADD CONSTRAINT [FK_EmploymentActivity_Activity_Id] FOREIGN KEY ([Id]) REFERENCES [Activities].[Activity] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Activities].[EmploymentActivity]
    ADD CONSTRAINT [FK_EmploymentActivity_Document_DocumentId] FOREIGN KEY ([DocumentId]) REFERENCES [Document].[Document] ([Id]) ON DELETE CASCADE;
GO

CREATE NONCLUSTERED INDEX [IX_EmploymentActivity_DocumentId]
    ON [Activities].[EmploymentActivity]([DocumentId] ASC);
GO

ALTER TABLE [Activities].[EmploymentActivity]
    ADD CONSTRAINT [PK_EmploymentActivity] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

