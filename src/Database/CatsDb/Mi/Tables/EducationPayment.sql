CREATE TABLE [Mi].[EducationPayment] (
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
    [CourseLevel]         NVARCHAR (100)   DEFAULT (N'') NOT NULL,
    [CourseTitle]         NVARCHAR (200)   DEFAULT (N'') NOT NULL,
    [ActivityInput]       DATE             DEFAULT ('0001-01-01') NOT NULL,
    [CommencedDate]       DATE             DEFAULT ('0001-01-01') NOT NULL,
    [PaymentPeriod]       DATE             DEFAULT ('0001-01-01') NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [ix_ActivityPayment_ParticipantId]
    ON [Mi].[EducationPayment]([ParticipantId] ASC, [ContractId] ASC, [CourseLevel] ASC, [CourseTitle] ASC, [EligibleForPayment] ASC);
GO

ALTER TABLE [Mi].[EducationPayment]
    ADD CONSTRAINT [PK_EducationPayment] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

