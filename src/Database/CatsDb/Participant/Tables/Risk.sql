CREATE TABLE [Participant].[Risk] (
    [Id]                              UNIQUEIDENTIFIER NOT NULL,
    [ActivityRecommendations]         NVARCHAR (MAX)   NULL,
    [ActivityRecommendationsReceived] DATETIME2 (7)    NULL,
    [ActivityRestrictions]            NVARCHAR (MAX)   NULL,
    [ActivityRestrictionsReceived]    DATETIME2 (7)    NULL,
    [AdditionalInformation]           NVARCHAR (MAX)   NULL,
    [LicenseConditions]               NVARCHAR (MAX)   NULL,
    [LicenseEnd]                      DATETIME2 (7)    NULL,
    [IsRelevantToCustody]             BIT              NULL,
    [IsRelevantToCommunity]           BIT              NULL,
    [IsSubjectToSHPO]                 INT              NULL,
    [NSDCase]                         INT              NULL,
    [ParticipantId]                   NVARCHAR (9)     NOT NULL,
    [PSFRestrictions]                 NVARCHAR (MAX)   NULL,
    [PSFRestrictionsReceived]         DATETIME2 (7)    NULL,
    [ReferrerName]                    NVARCHAR (200)   NULL,
    [ReferrerEmail]                   NVARCHAR (320)   NULL,
    [ReferredOn]                      DATETIME2 (7)    NULL,
    [ReviewReason]                    INT              NOT NULL,
    [ReviewJustification]             NVARCHAR (MAX)   NULL,
    [RiskToChildrenInCustody]         INT              NULL,
    [RiskToPublicInCustody]           INT              NULL,
    [RiskToKnownAdultInCustody]       INT              NULL,
    [RiskToStaffInCustody]            INT              NULL,
    [RiskToOtherPrisonersInCustody]   INT              NULL,
    [RiskToSelfInCustody]             INT              NULL,
    [RiskToChildrenInCommunity]       INT              NULL,
    [RiskToPublicInCommunity]         INT              NULL,
    [RiskToKnownAdultInCommunity]     INT              NULL,
    [RiskToStaffInCommunity]          INT              NULL,
    [RiskToSelfInCommunity]           INT              NULL,
    [SpecificRisk]                    NVARCHAR (MAX)   NULL,
    [Created]                         DATETIME2 (7)    NULL,
    [CreatedBy]                       NVARCHAR (36)    NULL,
    [LastModified]                    DATETIME2 (7)    NULL,
    [LastModifiedBy]                  NVARCHAR (36)    NULL,
    [Completed]                       DATETIME2 (7)    NULL,
    [CompletedBy]                     NVARCHAR (36)    NULL,
    [RegistrationDetailsJson]         NVARCHAR (MAX)   NULL,
    [NoLicenseEndDate]                BIT              NULL,
    [RiskToSelfInCommunityNew]        INT              NULL,
    [RiskToSelfInCustodyNew]          INT              NULL,
    [LocationId]                      INT              DEFAULT ((0)) NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_Risk_LocationId]
    ON [Participant].[Risk]([LocationId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Risk_ParticipantId]
    ON [Participant].[Risk]([ParticipantId] ASC);
GO

ALTER TABLE [Participant].[Risk]
    ADD CONSTRAINT [FK_Risk_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Participant].[Risk]
    ADD CONSTRAINT [FK_Risk_Location_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Configuration].[Location] ([Id]);
GO

ALTER TABLE [Participant].[Risk]
    ADD CONSTRAINT [PK_Risk] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

