CREATE TABLE [Participant].[ObjectiveTask] (
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [Due]             DATETIME2 (7)    NOT NULL,
    [Completed]       DATETIME2 (7)    NULL,
    [CompletedBy]     NVARCHAR (36)    NULL,
    [CompletedStatus] INT              NULL,
    [Index]           INT              NOT NULL,
    [Justification]   NVARCHAR (MAX)   NULL,
    [ObjectiveId]     UNIQUEIDENTIFIER NOT NULL,
    [Description]     NVARCHAR (MAX)   NOT NULL,
    [Created]         DATETIME2 (7)    NULL,
    [CreatedBy]       NVARCHAR (36)    NULL,
    [LastModified]    DATETIME2 (7)    NULL,
    [LastModifiedBy]  NVARCHAR (36)    NULL,
    [IsMandatory]     BIT              DEFAULT (CONVERT([bit],(0))) NOT NULL
);
GO

ALTER TABLE [Participant].[ObjectiveTask]
    ADD CONSTRAINT [FK_ObjectiveTask_User_CompletedBy] FOREIGN KEY ([CompletedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Participant].[ObjectiveTask]
    ADD CONSTRAINT [FK_ObjectiveTask_Objective_ObjectiveId] FOREIGN KEY ([ObjectiveId]) REFERENCES [Participant].[Objective] ([Id]) ON DELETE CASCADE;
GO

CREATE NONCLUSTERED INDEX [IX_ObjectiveTask_CompletedBy]
    ON [Participant].[ObjectiveTask]([CompletedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ObjectiveTask_ObjectiveId]
    ON [Participant].[ObjectiveTask]([ObjectiveId] ASC);
GO

ALTER TABLE [Participant].[ObjectiveTask]
    ADD CONSTRAINT [PK_ObjectiveTask] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

