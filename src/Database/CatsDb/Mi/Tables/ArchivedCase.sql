CREATE TABLE [Mi].[ArchivedCase] (
    [Id]                      UNIQUEIDENTIFIER NOT NULL,
    [ParticipantId]           NVARCHAR (9)     NOT NULL,
    [FirstName]               NVARCHAR (50)    NOT NULL,
    [LastName]                NVARCHAR (50)    NOT NULL,
    [EnrolmentHistoryId]      INT              NOT NULL,
    [Created]                 DATETIME2 (7)    NOT NULL,
    [CreatedBy]               NVARCHAR (36)    NOT NULL,
    [AdditionalInfo]          NVARCHAR (1000)  NULL,
    [ArchiveReason]           NVARCHAR (1000)  NULL,
    [UnarchiveAdditionalInfo] NVARCHAR (1000)  NULL,
    [UnarchiveReason]         NVARCHAR (1000)  NULL,
    [From]                    DATETIME2 (7)    NOT NULL,
    [To]                      DATETIME2 (7)    NULL,
    [ContractId]              NVARCHAR (12)    NULL,
    [LocationId]              INT              NOT NULL,
    [LocationType]            NVARCHAR (25)    NOT NULL,
    [TenantId]                NVARCHAR (50)    NOT NULL
);
GO

ALTER TABLE [Mi].[ArchivedCase]
    ADD CONSTRAINT [PK_ArchivedCase] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [ix_ArchivedCase_Participant_Enrolment]
    ON [Mi].[ArchivedCase]([ParticipantId] ASC, [EnrolmentHistoryId] ASC, [TenantId] ASC, [From] ASC);
GO

