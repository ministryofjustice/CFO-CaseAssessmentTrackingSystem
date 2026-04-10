CREATE TABLE [Mi].[OutcomeQualityDipSampleParticipant] (
    [ParticipantId]                 NVARCHAR (9)     NOT NULL,
    [DipSampleId]                   UNIQUEIDENTIFIER NOT NULL,
    [LocationType]                  NVARCHAR (64)    NOT NULL,
    [HasClearParticipantJourney]    INT              DEFAULT ((0)) NOT NULL,
    [ShowsTaskProgression]          INT              DEFAULT ((0)) NOT NULL,
    [TTGDemonstratesGoodPRIProcess] INT              DEFAULT ((0)) NOT NULL,
    [SupportsJourney]               INT              DEFAULT ((0)) NOT NULL,
    [CsoComments]                   NVARCHAR (1000)  NULL,
    [LastModified]                  DATETIME2 (7)    NULL,
    [FinalReviewedBy]               NVARCHAR (36)    NULL,
    [Id]                            INT              IDENTITY (1, 1) NOT NULL,
    [CpmComments]                   NVARCHAR (MAX)   NULL,
    [CpmIsCompliant]                INT              DEFAULT ((0)) NOT NULL,
    [CpmReviewedBy]                 NVARCHAR (36)    NULL,
    [CpmReviewedOn]                 DATETIME2 (7)    NULL,
    [Created]                       DATETIME2 (7)    NULL,
    [CreatedBy]                     NVARCHAR (MAX)   NULL,
    [CsoIsCompliant]                INT              DEFAULT ((0)) NOT NULL,
    [CsoReviewedBy]                 NVARCHAR (36)    NULL,
    [CsoReviewedOn]                 DATETIME2 (7)    NULL,
    [FinalComments]                 NVARCHAR (MAX)   NULL,
    [FinalIsCompliant]              INT              DEFAULT ((0)) NOT NULL,
    [FinalReviewedOn]               DATETIME2 (7)    NULL,
    [LastModifiedBy]                NVARCHAR (MAX)   NULL,
    [AlignsWithDoS]                 INT              DEFAULT ((0)) NOT NULL,
    [TtgObjectiveTasks]             INT              DEFAULT ((0)) NOT NULL,
    [PreReleasePractical]           INT              DEFAULT ((0)) NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_OutcomeQualityDipSampleParticipant_CsoReviewedBy]
    ON [Mi].[OutcomeQualityDipSampleParticipant]([CsoReviewedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_OutcomeQualityDipSampleParticipant_FinalReviewedBy]
    ON [Mi].[OutcomeQualityDipSampleParticipant]([FinalReviewedBy] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_OutcomeQualityDipSampleParticipant_ParticipantId]
    ON [Mi].[OutcomeQualityDipSampleParticipant]([ParticipantId] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_OutcomeQualityDipSampleParticipant_DipSampleId_ParticipantId]
    ON [Mi].[OutcomeQualityDipSampleParticipant]([DipSampleId] ASC, [ParticipantId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_OutcomeQualityDipSampleParticipant_CpmReviewedBy]
    ON [Mi].[OutcomeQualityDipSampleParticipant]([CpmReviewedBy] ASC);
GO

ALTER TABLE [Mi].[OutcomeQualityDipSampleParticipant]
    ADD CONSTRAINT [FK_OutcomeQualityDipSampleParticipant_User_CsoReviewedBy] FOREIGN KEY ([CsoReviewedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Mi].[OutcomeQualityDipSampleParticipant]
    ADD CONSTRAINT [FK_OutcomeQualityDipSampleParticipant_User_FinalReviewedBy] FOREIGN KEY ([FinalReviewedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Mi].[OutcomeQualityDipSampleParticipant]
    ADD CONSTRAINT [FK_OutcomeQualityDipSampleParticipant_OutcomeQualityDipSample_DipSampleId] FOREIGN KEY ([DipSampleId]) REFERENCES [Mi].[OutcomeQualityDipSample] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Mi].[OutcomeQualityDipSampleParticipant]
    ADD CONSTRAINT [FK_OutcomeQualityDipSampleParticipant_User_CpmReviewedBy] FOREIGN KEY ([CpmReviewedBy]) REFERENCES [Identity].[User] ([Id]);
GO

ALTER TABLE [Mi].[OutcomeQualityDipSampleParticipant]
    ADD CONSTRAINT [FK_OutcomeQualityDipSampleParticipant_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant].[Participant] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Mi].[OutcomeQualityDipSampleParticipant]
    ADD CONSTRAINT [PK_OutcomeQualityDipSampleParticipant] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

