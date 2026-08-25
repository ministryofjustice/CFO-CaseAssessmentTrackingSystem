CREATE TABLE [Mi].[ContractTarget] (
    [Id]                   UNIQUEIDENTIFIER NOT NULL,
    [ContractId]           NVARCHAR (12)    NOT NULL,
    [Year]                 INT              NOT NULL,
    [Month]                INT              NOT NULL,
    [Prison]               INT              NOT NULL,
    [Community]            INT              NOT NULL,
    [Wings]                INT              NOT NULL,
    [Hubs]                 INT              NOT NULL,
    [PreReleaseSupport]    INT              NOT NULL,
    [ThroughTheGate]       INT              NOT NULL,
    [SupportWork]          INT              NOT NULL,
    [HumanCitizenship]     INT              NOT NULL,
    [CommunityAndSocial]   INT              NOT NULL,
    [Interventions]        INT              NOT NULL,
    [Employment]           INT              NOT NULL,
    [TrainingAndEducation] INT              NOT NULL
);
GO

ALTER TABLE [Mi].[ContractTarget]
    ADD CONSTRAINT [PK_ContractTarget] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ContractTarget_ContractId_Year_Month]
    ON [Mi].[ContractTarget]([ContractId] ASC, [Year] ASC, [Month] ASC);
GO
