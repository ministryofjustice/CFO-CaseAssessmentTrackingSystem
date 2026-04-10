CREATE TABLE [Participant].[PathwayPlanReview] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [PathwayPlanId]  UNIQUEIDENTIFIER NOT NULL,
    [LocationId]     INT              NOT NULL,
    [ReviewDate]     DATETIME2 (7)    NOT NULL,
    [Review]         NVARCHAR (1000)  NULL,
    [ReviewReason]   INT              NOT NULL,
    [ParticipantId]  NVARCHAR (9)     NOT NULL,
    [Created]        DATETIME2 (7)    NULL,
    [CreatedBy]      NVARCHAR (36)    NULL,
    [LastModified]   DATETIME2 (7)    NULL,
    [LastModifiedBy] NVARCHAR (36)    NULL
);
GO

ALTER TABLE [Participant].[PathwayPlanReview]
    ADD CONSTRAINT [FK_PathwayPlanReview_PathwayPlan_PathwayPlanId] FOREIGN KEY ([PathwayPlanId]) REFERENCES [Participant].[PathwayPlan] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Participant].[PathwayPlanReview]
    ADD CONSTRAINT [PK_PathwayPlanReview] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_PathwayPlanReview_ParticipantId]
    ON [Participant].[PathwayPlanReview]([ParticipantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_PathwayPlanReview_PathwayPlanId]
    ON [Participant].[PathwayPlanReview]([PathwayPlanId] ASC);
GO

