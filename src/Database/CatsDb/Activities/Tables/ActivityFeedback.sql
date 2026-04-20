CREATE TABLE [Activities].[ActivityFeedback] (
    [Id]                            UNIQUEIDENTIFIER    NOT NULL,
    [ActivityId]                    UNIQUEIDENTIFIER    NOT NULL,
    [ParticipantId]                 NVARCHAR (9)        NOT NULL,
    [RecipientUserId]               NVARCHAR (36)       NOT NULL,
    [Message]                       NVARCHAR (1000)     NOT NULL,
    [Outcome]                       INT                 NOT NULL,
    [Stage]                         INT                 NOT NULL,
    [ActivityProcessedDate]         DATETIME2           NOT NULL,
    [IsRead]                        BIT                 NOT NULL,
    [ReadAt]                        DATETIME2           NULL,
    [TenantId]                      NVARCHAR (50)       NOT NULL,
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

ALTER TABLE [Activities].[ActivityFeedback] 
    ADD CONSTRAINT [FK_ActivityFeedback_Activity_ActivityId] FOREIGN KEY ( [ActivityId] ) REFERENCES [Activities].[Activity] ( [Id] );

GO

ALTER TABLE [Activities].[ActivityFeedback] 
    ADD CONSTRAINT [FK_ActivityFeedback_Participant_ParticipantId] FOREIGN KEY ( [ParticipantId] ) REFERENCES [Participant].[Participant] ( [Id] );

GO

ALTER TABLE [Activities].[ActivityFeedback] 
    ADD CONSTRAINT [FK_ActivityFeedback_Tenant_TenantId] FOREIGN KEY ( [TenantId] ) REFERENCES [Configuration].[Tenant] ( [Id] );

GO

ALTER TABLE [Activities].[ActivityFeedback] 
    ADD CONSTRAINT [FK_ActivityFeedback_User_CreatedBy] FOREIGN KEY ( [CreatedBy] ) REFERENCES [Identity].[User] ( [Id] );

GO

ALTER TABLE [Activities].[ActivityFeedback] 
    ADD CONSTRAINT [FK_ActivityFeedback_User_EditorId] FOREIGN KEY ( [EditorId] ) REFERENCES [Identity].[User] ( [Id] );

GO

ALTER TABLE [Activities].[ActivityFeedback] 
    ADD CONSTRAINT [FK_ActivityFeedback_User_OwnerId] FOREIGN KEY ( [OwnerId] ) REFERENCES [Identity].[User] ( [Id] );

GO

ALTER TABLE [Activities].[ActivityFeedback] 
    ADD CONSTRAINT [FK_ActivityFeedback_User_RecipientUserId] FOREIGN KEY ( [RecipientUserId] ) REFERENCES [Identity].[User] ( [Id] );

GO

CREATE NONCLUSTERED INDEX [IX_ActivityFeedback_ActivityId]
    ON [Activities].[ActivityFeedback] ([ActivityId] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_ActivityFeedback_ActivityId_Created]
    ON [Activities].[ActivityFeedback] ([ActivityId] ASC, [Created] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_ActivityFeedback_CreatedBy]
    ON [Activities].[ActivityFeedback] ([CreatedBy] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_ActivityFeedback_EditorId]
    ON [Activities].[ActivityFeedback] ([EditorId] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_ActivityFeedback_OwnerId]
    ON [Activities].[ActivityFeedback] ([OwnerId] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_ActivityFeedback_ParticipantId]
    ON [Activities].[ActivityFeedback] ([ParticipantId] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_ActivityFeedback_RecipientUserId_IsRead]
    ON [Activities].[ActivityFeedback] ([RecipientUserId] ASC, [IsRead] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_ActivityFeedback_TenantId]
    ON [Activities].[ActivityFeedback] ([TenantId] ASC);

GO

ALTER TABLE [Activities].[ActivityFeedback] 
    ADD CONSTRAINT [PK_ActivityFeedback] PRIMARY KEY CLUSTERED ([Id] ASC);