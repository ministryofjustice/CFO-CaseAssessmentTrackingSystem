CREATE TABLE [Activities].[NonIsqActivity] (
    [Id] UNIQUEIDENTIFIER NOT NULL
);
GO

ALTER TABLE [Activities].[NonIsqActivity]
    ADD CONSTRAINT [FK_NonIsqActivity_Activity_Id] FOREIGN KEY ([Id]) REFERENCES [Activities].[Activity] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Activities].[NonIsqActivity]
    ADD CONSTRAINT [PK_NonIsqActivity] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

