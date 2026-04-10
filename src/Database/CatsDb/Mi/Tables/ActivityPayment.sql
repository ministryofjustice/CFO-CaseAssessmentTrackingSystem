CREATE TABLE [Mi].[ActivityPayment] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [ActivityId]          UNIQUEIDENTIFIER NOT NULL,
    [ActivityCategory]    NVARCHAR (100)   NOT NULL,
    [ActivityType]        NVARCHAR (50)    NOT NULL,
    [CreatedOn]           DATETIME2 (7)    NOT NULL,
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

ALTER TABLE [Mi].[ActivityPayment]
    ADD CONSTRAINT [PK_ActivityPayment] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE NONCLUSTERED INDEX [ix_ActivityPayment_ParticipantId]
    ON [Mi].[ActivityPayment]([ParticipantId] ASC, [ContractId] ASC, [ActivityCategory] ASC, [ActivityType] ASC, [ActivityApproved] ASC);
GO

