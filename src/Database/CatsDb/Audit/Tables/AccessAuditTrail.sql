CREATE TABLE [Audit].[AccessAuditTrail] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [RequestType]   NVARCHAR (300) NOT NULL,
    [ParticipantId] NVARCHAR (9)   NOT NULL,
    [UserId]        NVARCHAR (36)  NOT NULL,
    [AccessDate]    DATETIME2 (7)  NOT NULL
);
GO

ALTER TABLE [Audit].[AccessAuditTrail]
    ADD CONSTRAINT [PK_AccessAuditTrail] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

ALTER TABLE [Audit].[AccessAuditTrail]
    ADD CONSTRAINT [FK_AccessAuditTrail_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

CREATE NONCLUSTERED INDEX [IX_AccessAuditTrail_ParticipantId]
    ON [Audit].[AccessAuditTrail]([ParticipantId] ASC);
GO

