CREATE TABLE [Enrolment].[Qa1QueueNote] (
    [Id]                       INT              IDENTITY (1, 1) NOT NULL,
    [Message]                  NVARCHAR (1000)  NOT NULL,
    [CallReference]            NVARCHAR (20)    NULL,
    [Created]                  DATETIME2 (7)    NULL,
    [CreatedBy]                NVARCHAR (36)    NULL,
    [LastModified]             DATETIME2 (7)    NULL,
    [LastModifiedBy]           NVARCHAR (36)    NULL,
    [TenantId]                 NVARCHAR (50)    NOT NULL,
    [EnrolmentQa1QueueEntryId] UNIQUEIDENTIFIER NOT NULL,
    [IsExternal]               BIT              DEFAULT (CONVERT([bit],(0))) NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_Qa1QueueNote_LastModifiedBy]
    ON [Enrolment].[Qa1QueueNote]([LastModifiedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Qa1QueueNote_CreatedBy]
    ON [Enrolment].[Qa1QueueNote]([CreatedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Qa1QueueNote_EnrolmentQa1QueueEntryId]
    ON [Enrolment].[Qa1QueueNote]([EnrolmentQa1QueueEntryId] ASC);
GO

ALTER TABLE [Enrolment].[Qa1QueueNote]
    ADD CONSTRAINT [PK_Qa1QueueNote] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

ALTER TABLE [Enrolment].[Qa1QueueNote]
    ADD CONSTRAINT [FK_Qa1QueueNote_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Enrolment].[Qa1QueueNote]
    ADD CONSTRAINT [FK_Qa1QueueNote_Qa1Queue_EnrolmentQa1QueueEntryId] FOREIGN KEY ([EnrolmentQa1QueueEntryId]) REFERENCES [Enrolment].[Qa1Queue] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Enrolment].[Qa1QueueNote]
    ADD CONSTRAINT [FK_Qa1QueueNote_User_LastModifiedBy] FOREIGN KEY ([LastModifiedBy]) REFERENCES [Identity].[User] ([Id]);
GO

