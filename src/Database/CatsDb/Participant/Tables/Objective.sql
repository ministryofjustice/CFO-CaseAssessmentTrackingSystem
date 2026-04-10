CREATE TABLE [Participant].[Objective] (
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [Completed]       DATETIME2 (7)    NULL,
    [CompletedStatus] INT              NULL,
    [Index]           INT              NOT NULL,
    [PathwayPlanId]   UNIQUEIDENTIFIER NOT NULL,
    [Description]     NVARCHAR (MAX)   NOT NULL,
    [Justification]   NVARCHAR (MAX)   NULL,
    [Created]         DATETIME2 (7)    NULL,
    [CreatedBy]       NVARCHAR (36)    NULL,
    [LastModified]    DATETIME2 (7)    NULL,
    [LastModifiedBy]  NVARCHAR (36)    NULL,
    [CompletedBy]     NVARCHAR (36)    NULL,
    [IsMandatory]     BIT              DEFAULT (CONVERT([bit],(0))) NOT NULL
);
GO

ALTER TABLE [Participant].[Objective]
    ADD CONSTRAINT [FK_Objective_PathwayPlan_PathwayPlanId] FOREIGN KEY ([PathwayPlanId]) REFERENCES [Participant].[PathwayPlan] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Participant].[Objective]
    ADD CONSTRAINT [FK_Objective_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Participant].[Objective]
    ADD CONSTRAINT [FK_Objective_User_CompletedBy] FOREIGN KEY ([CompletedBy]) REFERENCES [Identity].[User] ([Id]);
GO

CREATE NONCLUSTERED INDEX [IX_Objective_CreatedBy]
    ON [Participant].[Objective]([CreatedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Objective_PathwayPlanId]
    ON [Participant].[Objective]([PathwayPlanId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Objective_CompletedBy]
    ON [Participant].[Objective]([CompletedBy] ASC);
GO

ALTER TABLE [Participant].[Objective]
    ADD CONSTRAINT [PK_Objective] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

