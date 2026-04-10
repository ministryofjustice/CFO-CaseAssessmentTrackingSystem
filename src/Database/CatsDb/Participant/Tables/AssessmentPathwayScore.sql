CREATE TABLE [Participant].[AssessmentPathwayScore] (
    [Pathway]      NVARCHAR (50)    NOT NULL,
    [AssessmentId] UNIQUEIDENTIFIER NOT NULL,
    [Score]        FLOAT (53)       NOT NULL
);
GO

ALTER TABLE [Participant].[AssessmentPathwayScore]
    ADD CONSTRAINT [PK_AssessmentPathwayScore] PRIMARY KEY CLUSTERED ([AssessmentId] ASC, [Pathway] ASC);
GO

ALTER TABLE [Participant].[AssessmentPathwayScore]
    ADD CONSTRAINT [FK_AssessmentPathwayScore_Assessment_AssessmentId] FOREIGN KEY ([AssessmentId]) REFERENCES [Participant].[Assessment] ([Id]) ON DELETE CASCADE;
GO

