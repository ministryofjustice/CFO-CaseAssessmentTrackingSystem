CREATE TABLE [Mi].[ActivityFeedback] (
    [Id]                            UNIQUEIDENTIFIER    NOT NULL,
    [ActivityId]                    UNIQUEIDENTIFIER    NOT NULL,
    [ParticipantId]                 NVARCHAR (9)        NOT NULL,
    [Message]                       NVARCHAR (1000)     NOT NULL,
    [Qa1Outcome]                    INT                 NOT NULL,
    [Outcome]                       INT                 NOT NULL,
    [Stage]                         INT                 NOT NULL,
    [Qa1Date]                       DATETIME2           NOT NULL,
    [IsRead]                        BIT                 NOT NULL,
    [ReadAt]                        DATETIME2           NULL,
    [Created]                       DATETIME2           NOT NULL,
    [CreatedBy]                     NVARCHAR (36)       NOT NULL,
    [LastModified]                  DATETIME2           NULL,
    [LastModifiedBy]                NVARCHAR (36)       NULL,
    [OwnerId]                       NVARCHAR (36)       NULL,
    [EditorId]                      NVARCHAR (36)       NULL,
    [ActivityCategory]              NVARCHAR (100)      NOT NULL,
    [ActivityFeedbackReason]        NVARCHAR (50)       NOT NULL,
    [ActivityType]                  NVARCHAR (50)       NOT NULL
);

GO

ALTER TABLE [Mi].[ActivityFeedback] 
    ADD CONSTRAINT [FK_ActivityFeedback_Activity_ActivityId] FOREIGN KEY ( [ActivityId] ) REFERENCES [Activities].[Activity] ( [Id] );

GO

ALTER TABLE [Mi].[ActivityFeedback] 
    ADD CONSTRAINT [FK_ActivityFeedback_Participant_ParticipantId] FOREIGN KEY ( [ParticipantId] ) REFERENCES [Participant].[Participant] ( [Id] );

GO

ALTER TABLE [Mi].[ActivityFeedback] 
    ADD CONSTRAINT [FK_ActivityFeedback_User_CreatedBy] FOREIGN KEY ( [CreatedBy] ) REFERENCES [Identity].[User] ( [Id] );

GO

ALTER TABLE [Mi].[ActivityFeedback] 
    ADD CONSTRAINT [FK_ActivityFeedback_User_EditorId] FOREIGN KEY ( [EditorId] ) REFERENCES [Identity].[User] ( [Id] );

GO

ALTER TABLE [Mi].[ActivityFeedback] 
    ADD CONSTRAINT [FK_ActivityFeedback_User_OwnerId] FOREIGN KEY ( [OwnerId] ) REFERENCES [Identity].[User] ( [Id] );

GO

CREATE NONCLUSTERED INDEX [IX_ActivityFeedback_ActivityId]
    ON [Mi].[ActivityFeedback] ([ActivityId] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_ActivityFeedback_ActivityId_Created]
    ON [Mi].[ActivityFeedback] ([ActivityId] ASC, [Created] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_ActivityFeedback_CreatedBy]
    ON [Mi].[ActivityFeedback] ([CreatedBy] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_ActivityFeedback_EditorId]
    ON [Mi].[ActivityFeedback] ([EditorId] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_ActivityFeedback_OwnerId]
    ON [Mi].[ActivityFeedback] ([OwnerId] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_ActivityFeedback_ParticipantId]
    ON [Mi].[ActivityFeedback] ([ParticipantId] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_ActivityFeedback_OwnerId_IsRead]
    ON [Mi].[ActivityFeedback] ([OwnerId] ASC, [IsRead] ASC);

GO


ALTER TABLE [Mi].[ActivityFeedback] 
    ADD CONSTRAINT [PK_ActivityFeedback] PRIMARY KEY CLUSTERED ([Id] ASC);