CREATE TABLE [Mi].[SupportAndReferralPayment] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [PriId]               UNIQUEIDENTIFIER NOT NULL,
    [SupportType]         NVARCHAR (20)    NOT NULL,
    [CreatedOn]           DATETIME2 (7)    NOT NULL,
    [Approved]            DATE             NOT NULL,
    [ParticipantId]       NVARCHAR (9)     NOT NULL,
    [SupportWorker]       NVARCHAR (MAX)   NOT NULL,
    [ContractId]          NVARCHAR (12)    NOT NULL,
    [LocationId]          INT              NOT NULL,
    [LocationType]        NVARCHAR (25)    NOT NULL,
    [TenantId]            NVARCHAR (50)    NOT NULL,
    [EligibleForPayment]  BIT              NOT NULL,
    [IneligibilityReason] NVARCHAR (250)   NULL,
    [ActivityInput]       DATE             DEFAULT ('0001-01-01') NOT NULL,
    [PaymentPeriod]       DATE             DEFAULT ('0001-01-01') NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [ix_ActivityPayment_ParticipantId]
    ON [Mi].[SupportAndReferralPayment]([ParticipantId] ASC, [ContractId] ASC, [SupportType] ASC, [Approved] ASC);
GO

ALTER TABLE [Mi].[SupportAndReferralPayment]
    ADD CONSTRAINT [PK_SupportAndReferralPayment] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

