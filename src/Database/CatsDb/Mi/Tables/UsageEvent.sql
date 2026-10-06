CREATE TABLE [Mi].[UsageEvent] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Area]        NVARCHAR (50)    NOT NULL,
    [Activity]    NVARCHAR (50)   NOT NULL,
    [UserId]      NVARCHAR (36)    NULL,
    [UserName]    NVARCHAR (100)   NULL,
    [TenantId]    NVARCHAR (50)    NULL,
    [Context]     NVARCHAR (256)   NULL,
    [OccurredOn]  DATETIME2 (7)    NOT NULL
);
GO

ALTER TABLE [Mi].[UsageEvent]
    ADD CONSTRAINT [PK_UsageEvent] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_UsageEvent_OccurredOn_Area_Activity]
    ON [Mi].[UsageEvent]([OccurredOn] ASC, [Area] ASC, [Activity] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_UsageEvent_UserId]
    ON [Mi].[UsageEvent]([UserId] ASC);
