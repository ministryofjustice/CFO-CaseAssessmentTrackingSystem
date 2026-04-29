CREATE TABLE [Enrolment].[EscalationNote] (
    [Id]                              INT              IDENTITY (1, 1) NOT NULL,
    [Message]                         NVARCHAR (1000)  NOT NULL,
    [CallReference]                   NVARCHAR (20)    NULL,
    [Created]                         DATETIME2 (7)    NULL,
    [CreatedBy]                       NVARCHAR (36)    NULL,
    [LastModified]                    DATETIME2 (7)    NULL,
    [LastModifiedBy]                  NVARCHAR (36)    NULL,
    [TenantId]                        NVARCHAR (50)    NOT NULL,
    [EnrolmentEscalationQueueEntryId] UNIQUEIDENTIFIER NOT NULL,
    [IsExternal]                      BIT              DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [FeedbackType]                    INT              NULL,
    [ReturnReason]                    NVARCHAR (100)   NULL
);
GO

ALTER TABLE [Enrolment].[EscalationNote]
    ADD CONSTRAINT [FK_EscalationNote_User_LastModifiedBy] FOREIGN KEY ([LastModifiedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Enrolment].[EscalationNote]
    ADD CONSTRAINT [FK_EscalationNote_EscalationQueue_EnrolmentEscalationQueueEntryId] FOREIGN KEY ([EnrolmentEscalationQueueEntryId]) REFERENCES [Enrolment].[EscalationQueue] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Enrolment].[EscalationNote]
    ADD CONSTRAINT [FK_EscalationNote_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Identity].[User] ([Id]);
GO

CREATE NONCLUSTERED INDEX [IX_EscalationNote_LastModifiedBy]
    ON [Enrolment].[EscalationNote]([LastModifiedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_EscalationNote_CreatedBy]
    ON [Enrolment].[EscalationNote]([CreatedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_EscalationNote_EnrolmentEscalationQueueEntryId]
    ON [Enrolment].[EscalationNote]([EnrolmentEscalationQueueEntryId] ASC);
GO

ALTER TABLE [Enrolment].[EscalationNote]
    ADD CONSTRAINT [PK_EscalationNote] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

