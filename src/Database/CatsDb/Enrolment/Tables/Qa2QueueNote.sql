CREATE TABLE [Enrolment].[Qa2QueueNote] (
    [Id]                       INT              IDENTITY (1, 1) NOT NULL,
    [Message]                  NVARCHAR (1000)  NOT NULL,
    [CallReference]            NVARCHAR (20)    NULL,
    [Created]                  DATETIME2 (7)    NULL,
    [CreatedBy]                NVARCHAR (36)    NULL,
    [LastModified]             DATETIME2 (7)    NULL,
    [LastModifiedBy]           NVARCHAR (36)    NULL,
    [TenantId]                 NVARCHAR (50)    NOT NULL,
    [EnrolmentQa2QueueEntryId] UNIQUEIDENTIFIER NOT NULL,
    [IsExternal]               BIT              DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [FeedbackType]             INT              NULL,
    [ReturnReason]             NVARCHAR (100)   NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_Qa2QueueNote_EnrolmentQa2QueueEntryId]
    ON [Enrolment].[Qa2QueueNote]([EnrolmentQa2QueueEntryId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Qa2QueueNote_LastModifiedBy]
    ON [Enrolment].[Qa2QueueNote]([LastModifiedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Qa2QueueNote_CreatedBy]
    ON [Enrolment].[Qa2QueueNote]([CreatedBy] ASC);
GO

ALTER TABLE [Enrolment].[Qa2QueueNote]
    ADD CONSTRAINT [FK_Qa2QueueNote_Qa2Queue_EnrolmentQa2QueueEntryId] FOREIGN KEY ([EnrolmentQa2QueueEntryId]) REFERENCES [Enrolment].[Qa2Queue] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Enrolment].[Qa2QueueNote]
    ADD CONSTRAINT [FK_Qa2QueueNote_User_LastModifiedBy] FOREIGN KEY ([LastModifiedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Enrolment].[Qa2QueueNote]
    ADD CONSTRAINT [FK_Qa2QueueNote_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Enrolment].[Qa2QueueNote]
    ADD CONSTRAINT [PK_Qa2QueueNote] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

