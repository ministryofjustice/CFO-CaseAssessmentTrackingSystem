CREATE TABLE [Mi].[OutcomeQualityDipSample] (
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [ContractId]      NVARCHAR (12)    NOT NULL,
    [CreatedOn]       DATETIME2 (7)    NOT NULL,
    [PeriodFrom]      DATETIME2 (7)    NOT NULL,
    [PeriodTo]        DATETIME2 (7)    NOT NULL,
    [ReviewedOn]      DATETIME2 (7)    NULL,
    [ReviewedBy]      NVARCHAR (36)    NULL,
    [Status]          INT              NOT NULL,
    [CpmCompliant]    INT              NULL,
    [CpmPercentage]   INT              NULL,
    [CpmScore]        INT              NULL,
    [Created]         DATETIME2 (7)    NULL,
    [CreatedBy]       NVARCHAR (MAX)   NULL,
    [CsoCompliant]    INT              NULL,
    [CsoPercentage]   INT              NULL,
    [CsoScore]        INT              NULL,
    [FinalCompliant]  INT              NULL,
    [FinalPercentage] INT              NULL,
    [FinalScore]      INT              NULL,
    [LastModified]    DATETIME2 (7)    NULL,
    [LastModifiedBy]  NVARCHAR (MAX)   NULL,
    [Size]            INT              DEFAULT ((0)) NOT NULL,
    [VerifiedBy]      NVARCHAR (36)    NULL,
    [VerifiedOn]      DATETIME2 (7)    NULL,
    [DocumentId]      UNIQUEIDENTIFIER NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_OutcomeQualityDipSample_VerifiedBy]
    ON [Mi].[OutcomeQualityDipSample]([VerifiedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_OutcomeQualityDipSample_ContractId]
    ON [Mi].[OutcomeQualityDipSample]([ContractId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_OutcomeQualityDipSample_ReviewedBy]
    ON [Mi].[OutcomeQualityDipSample]([ReviewedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_OutcomeQualityDipSample_DocumentId]
    ON [Mi].[OutcomeQualityDipSample]([DocumentId] ASC);
GO

ALTER TABLE [Mi].[OutcomeQualityDipSample]
    ADD CONSTRAINT [PK_OutcomeQualityDipSample] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

ALTER TABLE [Mi].[OutcomeQualityDipSample]
    ADD CONSTRAINT [FK_OutcomeQualityDipSample_User_VerifiedBy] FOREIGN KEY ([VerifiedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Mi].[OutcomeQualityDipSample]
    ADD CONSTRAINT [FK_OutcomeQualityDipSample_Contract_ContractId] FOREIGN KEY ([ContractId]) REFERENCES [Configuration].[Contract] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Mi].[OutcomeQualityDipSample]
    ADD CONSTRAINT [FK_OutcomeQualityDipSample_User_ReviewedBy] FOREIGN KEY ([ReviewedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Mi].[OutcomeQualityDipSample]
    ADD CONSTRAINT [FK_OutcomeQualityDipSample_Document_DocumentId] FOREIGN KEY ([DocumentId]) REFERENCES [Document].[Document] ([Id]);
GO

