CREATE TABLE [Mi].[InductionPayment] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]           DATETIME2 (7)    NOT NULL,
    [ParticipantId]       NVARCHAR (9)     NOT NULL,
    [SupportWorker]       NVARCHAR (36)    NOT NULL,
    [ContractId]          NVARCHAR (12)    NOT NULL,
    [Induction]           DATE             NOT NULL,
    [Approved]            DATE             DEFAULT ('0001-01-01') NOT NULL,
    [LocationId]          INT              NOT NULL,
    [LocationType]        NVARCHAR (25)    DEFAULT (N'') NOT NULL,
    [TenantId]            NVARCHAR (50)    NOT NULL,
    [EligibleForPayment]  BIT              NOT NULL,
    [IneligibilityReason] NVARCHAR (250)   NULL,
    [CommencedDate]       DATE             DEFAULT ('0001-01-01') NOT NULL,
    [InductionInput]      DATE             DEFAULT ('0001-01-01') NOT NULL,
    [PaymentPeriod]       DATE             DEFAULT ('0001-01-01') NOT NULL
);
GO

ALTER TABLE [Mi].[InductionPayment]
    ADD CONSTRAINT [PK_InductionPayment] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [ix_InductionPayment_ParticipantId]
    ON [Mi].[InductionPayment]([ParticipantId] ASC, [ContractId] ASC);
GO

