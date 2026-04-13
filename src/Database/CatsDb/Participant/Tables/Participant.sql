CREATE TABLE [Participant].[Participant] (
    [Id]                             NVARCHAR (9)    NOT NULL,
    [FirstName]                      NVARCHAR (50)   NOT NULL,
    [MiddleName]                     NVARCHAR (50)   NULL,
    [LastName]                       NVARCHAR (50)   NOT NULL,
    [DateOfBirth]                    DATE            NOT NULL,
    [ReferralSource]                 NVARCHAR (100)  NOT NULL,
    [ReferralComments]               NVARCHAR (1000) NULL,
    [EnrolmentStatus]                INT             NOT NULL,
    [ConsentStatus]                  INT             NOT NULL,
    [CurrentLocationId]              INT             NOT NULL,
    [EnrolmentLocationId]            INT             NULL,
    [EnrolmentLocationJustification] NVARCHAR (1000) NULL,
    [Created]                        DATETIME2 (7)   NULL,
    [CreatedBy]                      NVARCHAR (36)   NULL,
    [LastModified]                   DATETIME2 (7)   NULL,
    [LastModifiedBy]                 NVARCHAR (36)   NULL,
    [OwnerId]                        NVARCHAR (36)   NULL,
    [EditorId]                       NVARCHAR (36)   NULL,
    [AssessmentJustification]        NVARCHAR (MAX)  NULL,
    [Gender]                         NVARCHAR (29)   NULL,
    [RegistrationDetailsJson]        NVARCHAR (MAX)  NULL,
    [RiskDue]                        DATE            NULL,
    [Nationality]                    NVARCHAR (50)   NULL,
    [DateOfFirstConsent]             DATE            NULL,
    [LastSyncDate]                   DATETIME2 (7)   NULL,
    [BioDue]                         DATETIME2 (7)   NULL,
    [RiskDueReason]                  INT             DEFAULT ((0)) NOT NULL,
    [PrimaryRecordKeyAtCreation]     NVARCHAR (16)   NULL,
    [DeactivatedInFeed]              DATE            NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_Participant_CurrentLocationId]
    ON [Participant].[Participant]([CurrentLocationId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Participant_EnrolmentLocationId]
    ON [Participant].[Participant]([EnrolmentLocationId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Participant_OwnerId]
    ON [Participant].[Participant]([OwnerId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Participant_EditorId]
    ON [Participant].[Participant]([EditorId] ASC);
GO

ALTER TABLE [Participant].[Participant]
    ADD CONSTRAINT [FK_Participant_User_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Participant].[Participant]
    ADD CONSTRAINT [FK_Participant_Location] FOREIGN KEY ([CurrentLocationId]) REFERENCES [Configuration].[Location] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Participant].[Participant]
    ADD CONSTRAINT [FK_Participant_User_EditorId] FOREIGN KEY ([EditorId]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Participant].[Participant]
    ADD CONSTRAINT [FK_Participant_EnrolmentLocation] FOREIGN KEY ([EnrolmentLocationId]) REFERENCES [Configuration].[Location] ([Id]);
GO

ALTER TABLE [Participant].[Participant]
    ADD CONSTRAINT [PK_Participant] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

--These lookups cannot be applied until after Production is updated.

--ALTER TABLE [Participant].[Participant]
--    ADD CONSTRAINT [FK_Participant_EnrolmentStatus] FOREIGN KEY ( [EnrolmentStatus] ) REFERENCES [Lookup].[EnrolmentStatus] ( [Value] );

--GO

--ALTER TABLE [Participant].[Participant]
--    ADD CONSTRAINT [FK_Participant_ConsentStatus] FOREIGN KEY  ( [ConsentStatus] ) REFERENCES [Lookup].[ConsentStatus] ( [Value] );

--GO

--ALTER TABLE [Participant].[Participant]
--    ADD CONSTRAINT [FK_Participant_RiskDueReason] FOREIGN KEY  ( [RiskDueReason] ) REFERENCES [Lookup].[RiskDueReason] ( [Value] );

--GO