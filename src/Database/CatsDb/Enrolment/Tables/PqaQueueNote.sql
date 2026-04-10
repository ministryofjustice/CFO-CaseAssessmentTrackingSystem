CREATE TABLE [Enrolment].[PqaQueueNote] (
    [Id]                       INT              IDENTITY (1, 1) NOT NULL,
    [Message]                  NVARCHAR (1000)  NOT NULL,
    [CallReference]            NVARCHAR (20)    NULL,
    [Created]                  DATETIME2 (7)    NULL,
    [CreatedBy]                NVARCHAR (36)    NULL,
    [LastModified]             DATETIME2 (7)    NULL,
    [LastModifiedBy]           NVARCHAR (36)    NULL,
    [TenantId]                 NVARCHAR (50)    NOT NULL,
    [EnrolmentPqaQueueEntryId] UNIQUEIDENTIFIER NOT NULL
);
GO

ALTER TABLE [Enrolment].[PqaQueueNote]
    ADD CONSTRAINT [FK_PqaQueueNote_User_LastModifiedBy] FOREIGN KEY ([LastModifiedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Enrolment].[PqaQueueNote]
    ADD CONSTRAINT [FK_PqaQueueNote_PqaQueue_EnrolmentPqaQueueEntryId] FOREIGN KEY ([EnrolmentPqaQueueEntryId]) REFERENCES [Enrolment].[PqaQueue] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Enrolment].[PqaQueueNote]
    ADD CONSTRAINT [FK_PqaQueueNote_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Enrolment].[PqaQueueNote]
    ADD CONSTRAINT [PK_PqaQueueNote] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_PqaQueueNote_EnrolmentPqaQueueEntryId]
    ON [Enrolment].[PqaQueueNote]([EnrolmentPqaQueueEntryId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_PqaQueueNote_LastModifiedBy]
    ON [Enrolment].[PqaQueueNote]([LastModifiedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_PqaQueueNote_CreatedBy]
    ON [Enrolment].[PqaQueueNote]([CreatedBy] ASC);
GO

