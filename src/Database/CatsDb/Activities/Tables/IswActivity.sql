CREATE TABLE [Activities].[IswActivity] (
    [Id]                         UNIQUEIDENTIFIER NOT NULL,
    [WraparoundSupportStartedOn] DATETIME2 (7)    NOT NULL,
    [HoursPerformedPre]          FLOAT (53)       NOT NULL,
    [HoursPerformedDuring]       FLOAT (53)       NOT NULL,
    [HoursPerformedPost]         FLOAT (53)       NOT NULL,
    [BaselineAchievedOn]         DATETIME2 (7)    NOT NULL,
    [DocumentId]                 UNIQUEIDENTIFIER NOT NULL
);
GO

ALTER TABLE [Activities].[IswActivity]
    ADD CONSTRAINT [PK_IswActivity] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

ALTER TABLE [Activities].[IswActivity]
    ADD CONSTRAINT [FK_IswActivity_Activity_Id] FOREIGN KEY ([Id]) REFERENCES [Activities].[Activity] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Activities].[IswActivity]
    ADD CONSTRAINT [FK_IswActivity_Document_DocumentId] FOREIGN KEY ([DocumentId]) REFERENCES [Document].[Document] ([Id]) ON DELETE CASCADE;
GO

CREATE NONCLUSTERED INDEX [IX_IswActivity_DocumentId]
    ON [Activities].[IswActivity]([DocumentId] ASC);
GO

