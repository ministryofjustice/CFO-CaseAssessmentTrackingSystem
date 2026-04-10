CREATE TABLE [Participant].[PathwayPlan] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [ParticipantId]  NVARCHAR (9)     NOT NULL,
    [Created]        DATETIME2 (7)    NULL,
    [CreatedBy]      NVARCHAR (36)    NULL,
    [LastModified]   DATETIME2 (7)    NULL,
    [LastModifiedBy] NVARCHAR (36)    NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_PathwayPlan_ParticipantId]
    ON [Participant].[PathwayPlan]([ParticipantId] ASC);
GO

ALTER TABLE [Participant].[PathwayPlan]
    ADD CONSTRAINT [PK_PathwayPlan] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

