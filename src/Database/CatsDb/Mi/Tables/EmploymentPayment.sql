CREATE TABLE [Mi].[EmploymentPayment] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]           DATETIME2 (7)    NOT NULL,
    [ActivityId]          UNIQUEIDENTIFIER NOT NULL,
    [ActivityApproved]    DATE             NOT NULL,
    [ParticipantId]       NVARCHAR (9)     NOT NULL,
    [ContractId]          NVARCHAR (12)    NOT NULL,
    [LocationId]          INT              NOT NULL,
    [LocationType]        NVARCHAR (25)    NOT NULL,
    [TenantId]            NVARCHAR (50)    NOT NULL,
    [EligibleForPayment]  BIT              NOT NULL,
    [IneligibilityReason] NVARCHAR (250)   NULL,
    [ActivityInput]       DATE             DEFAULT ('0001-01-01') NOT NULL,
    [CommencedDate]       DATE             DEFAULT ('0001-01-01') NOT NULL,
    [PaymentPeriod]       DATE             DEFAULT ('0001-01-01') NOT NULL
);
GO

ALTER TABLE [Mi].[EmploymentPayment]
    ADD CONSTRAINT [PK_EmploymentPayment] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [ix_ActivityPayment_ParticipantId]
    ON [Mi].[EmploymentPayment]([ParticipantId] ASC, [ContractId] ASC, [CommencedDate] ASC, [EligibleForPayment] ASC);
GO

