CREATE TABLE [Participant].[AssessmentAnswer] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [AssessmentId]       UNIQUEIDENTIFIER NOT NULL,
    [QuestionCode]       NVARCHAR(3)   NOT NULL,
    [Answer]             NVARCHAR(80)  NOT NULL,

    CONSTRAINT [PK_AssessmentAnswer]
        PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UQ_AssessmentAnswer_BusinessKey]
ON [Participant].[AssessmentAnswer]
(
    [AssessmentId],
    [QuestionCode],
    [Answer]
);
GO

CREATE NONCLUSTERED INDEX [IX_AssessmentAnswer_AssessmentId]
ON [Participant].[AssessmentAnswer] ([AssessmentId]);
GO

ALTER TABLE [Participant].[AssessmentAnswer]
ADD CONSTRAINT [FK_AssessmentAnswer_Assessment_AssessmentId]
FOREIGN KEY ([AssessmentId])
REFERENCES [Participant].[Assessment] ([Id])
ON DELETE CASCADE;
GO