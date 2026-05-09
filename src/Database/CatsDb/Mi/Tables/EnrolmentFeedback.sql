CREATE TABLE [Mi].[EnrolmentFeedback] (
    [Id]                            UNIQUEIDENTIFIER    NOT NULL,
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
    [EnrolmentFeedbackReason]       NVARCHAR (50)      NOT NULL
);

GO

ALTER TABLE [Mi].[EnrolmentFeedback] 
    ADD CONSTRAINT [FK_EnrolmentFeedback_Participant_ParticipantId] FOREIGN KEY ( [ParticipantId] ) REFERENCES [Participant].[Participant] ( [Id] );

GO

ALTER TABLE [Mi].[EnrolmentFeedback] 
    ADD CONSTRAINT [FK_EnrolmentFeedback_User_CreatedBy] FOREIGN KEY ( [CreatedBy] ) REFERENCES [Identity].[User] ( [Id] );

GO

ALTER TABLE [Mi].[EnrolmentFeedback] 
    ADD CONSTRAINT [FK_EnrolmentFeedback_User_EditorId] FOREIGN KEY ( [EditorId] ) REFERENCES [Identity].[User] ( [Id] );

GO

ALTER TABLE [Mi].[EnrolmentFeedback] 
    ADD CONSTRAINT [FK_EnrolmentFeedback_User_OwnerId] FOREIGN KEY ( [OwnerId] ) REFERENCES [Identity].[User] ( [Id] );

GO

CREATE NONCLUSTERED INDEX [IX_EnrolmentFeedback_CreatedBy]
    ON [Mi].[EnrolmentFeedback] ([CreatedBy] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_EnrolmentFeedback_EditorId]
    ON [Mi].[EnrolmentFeedback] ([EditorId] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_EnrolmentFeedback_OwnerId]
    ON [Mi].[EnrolmentFeedback] ([OwnerId] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_EnrolmentFeedback_ParticipantId]
    ON [Mi].[EnrolmentFeedback] ([ParticipantId] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_EnrolmentFeedback_OwnerId_IsRead]
    ON [Mi].[EnrolmentFeedback] ([OwnerId] ASC, [IsRead] ASC);

GO

ALTER TABLE [Mi].[EnrolmentFeedback] 
    ADD CONSTRAINT [PK_EnrolmentFeedback] PRIMARY KEY CLUSTERED ([Id] ASC);

GO