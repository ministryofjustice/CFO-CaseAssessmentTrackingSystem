CREATE TABLE [Mi].[ParticipantEngagement] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [ParticipantId]     NVARCHAR (9)     NOT NULL,
    [Description]       NVARCHAR (256)   NOT NULL,
    [Category]          NVARCHAR (64)    NOT NULL,
    [EngagedOn]         DATE             NOT NULL,
    [EngagedWithTenant] NVARCHAR (50)    NOT NULL,
    [CreatedOn]         DATETIME2 (7)    NOT NULL,
    [EngagedAtContract] NVARCHAR (50)    DEFAULT (N'') NOT NULL,
    [EngagedAtLocation] NVARCHAR (200)   DEFAULT (N'') NOT NULL,
    [EngagedWith]       NVARCHAR (100)   DEFAULT (N'') NOT NULL
);
GO

ALTER TABLE [Mi].[ParticipantEngagement]
    ADD CONSTRAINT [PK_ParticipantEngagement] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ParticipantEngagement_EngagedOn_ParticipantId]
    ON [Mi].[ParticipantEngagement]([EngagedOn] ASC, [ParticipantId] ASC);
GO

