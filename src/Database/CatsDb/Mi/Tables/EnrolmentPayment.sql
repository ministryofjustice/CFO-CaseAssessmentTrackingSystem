CREATE TABLE [Mi].[EnrolmentPayment] (
    [Id]                     UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]              DATETIME2 (7)    NOT NULL,
    [ParticipantId]          NVARCHAR (9)     NOT NULL,
    [SupportWorker]          NVARCHAR (36)    NOT NULL,
    [ContractId]             NVARCHAR (12)    NOT NULL,
    [ConsentAdded]           DATE             NOT NULL,
    [ConsentSigned]          DATE             NOT NULL,
    [SubmissionToPqa]        DATE             NOT NULL,
    [SubmissionToAuthority]  DATE             NOT NULL,
    [SubmissionsToAuthority] INT              NOT NULL,
    [Approved]               DATE             NOT NULL,
    [LocationId]             INT              NOT NULL,
    [LocationType]           NVARCHAR (25)    DEFAULT (N'') NOT NULL,
    [TenantId]               NVARCHAR (50)    NOT NULL,
    [ReferralRoute]          NVARCHAR (100)   NOT NULL,
    [EligibleForPayment]     BIT              NOT NULL,
    [IneligibilityReason]    NVARCHAR (250)   NULL
);
GO

ALTER TABLE [Mi].[EnrolmentPayment]
    ADD CONSTRAINT [PK_EnrolmentPayment] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_EnrolmentPayment_ParticipantId]
    ON [Mi].[EnrolmentPayment]([ParticipantId] ASC);
GO

