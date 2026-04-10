CREATE TABLE [Mi].[ReassessmentPayment] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [AssessmentId]        UNIQUEIDENTIFIER NOT NULL,
    [AssessmentCompleted] DATETIME2 (7)    NOT NULL,
    [AssessmentCreated]   DATETIME2 (7)    NOT NULL,
    [ParticipantId]       NVARCHAR (9)     NOT NULL,
    [TenantId]            NVARCHAR (50)    NOT NULL,
    [EligibleForPayment]  BIT              NOT NULL,
    [IneligibilityReason] NVARCHAR (250)   NULL,
    [PaymentPeriod]       DATE             NOT NULL,
    [ContractId]          NVARCHAR (12)    NOT NULL,
    [LocationId]          INT              NOT NULL,
    [LocationType]        NVARCHAR (25)    NOT NULL,
    [SupportWorker]       NVARCHAR (36)    NOT NULL
);
GO

ALTER TABLE [Mi].[ReassessmentPayment]
    ADD CONSTRAINT [PK_ReassessmentPayment] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

