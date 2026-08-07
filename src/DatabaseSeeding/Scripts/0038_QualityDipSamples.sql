

IF NOT EXISTS (SELECT TOP (1) [Id] FROM [Mi].[OutcomeQualityDipSample])
BEGIN


	DECLARE @FirstOfTheMonth DATETIME2 = DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0);
	DECLARE @EndOfTheMonth DATETIME2 = DATEADD(SECOND, -1, DATEADD(MONTH, 1, @FirstOfTheMonth));

	INSERT INTO [Mi].[OutcomeQualityDipSample] ([Id], [ContractId], [CreatedOn], [PeriodFrom], [PeriodTo], [ReviewedOn], [ReviewedBy], [Status], [CpmCompliant], [CpmPercentage], [CpmScore], [Created], [CreatedBy], [CsoCompliant], [CsoPercentage], [CsoScore], [FinalCompliant], [FinalPercentage], [FinalScore], [LastModified], [LastModifiedBy], [Size], [VerifiedBy], [VerifiedOn], [DocumentId]) 
	VALUES 
	(N'01990585-d759-717c-94b9-867b3fde1e4f', N'con_24038', @FirstOfTheMonth, @FirstOfTheMonth, @EndOfTheMonth, NULL, NULL, 0, NULL, NULL, NULL, @FirstOfTheMonth, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 5, NULL, NULL, NULL),
	(N'01990585-d759-7a75-bc0b-b09567674a50', N'con_24043', @FirstOfTheMonth, @FirstOfTheMonth, @EndOfTheMonth, NULL, NULL, 0, NULL, NULL, NULL, @FirstOfTheMonth, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 2, NULL, NULL, NULL);
    
	SET IDENTITY_INSERT [Mi].[OutcomeQualityDipSampleParticipant] ON
	
	INSERT INTO [Mi].[OutcomeQualityDipSampleParticipant] ([Id], [ParticipantId], [DipSampleId], [LocationType], [HasClearParticipantJourney], [ShowsTaskProgression], [TTGDemonstratesGoodPRIProcess], [SupportsJourney], [CsoComments], [LastModified], [FinalReviewedBy], [CpmComments], [CpmIsCompliant], [CpmReviewedBy], [CpmReviewedOn], [Created], [CreatedBy], [CsoIsCompliant], [CsoReviewedBy], [CsoReviewedOn], [FinalComments], [FinalIsCompliant], [FinalReviewedOn], [LastModifiedBy], [AlignsWithDoS]) 
	VALUES 
	(31, N'1FAK0003A', N'01990585-d759-7a75-bc0b-b09567674a50', N'Wider Custody', 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0, NULL, NULL, @FirstOfTheMonth, NULL, 0, NULL, NULL, NULL, 0, NULL, NULL, 0),
	(32, N'1FAK0015A', N'01990585-d759-7a75-bc0b-b09567674a50', N'Wider Custody', 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0, NULL, NULL, @FirstOfTheMonth, NULL, 0, NULL, NULL, NULL, 0, NULL, NULL, 0),
	(33, N'1FAK0008A', N'01990585-d759-717c-94b9-867b3fde1e4f', N'Wider Custody', 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0, NULL, NULL, @FirstOfTheMonth, NULL, 0, NULL, NULL, NULL, 0, NULL, NULL, 0),
	(34, N'1FAK0005A', N'01990585-d759-717c-94b9-867b3fde1e4f', N'Wider Custody', 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0, NULL, NULL, @FirstOfTheMonth, NULL, 0, NULL, NULL, NULL, 0, NULL, NULL, 0),
	(35, N'1FAK0006A', N'01990585-d759-717c-94b9-867b3fde1e4f', N'Wider Custody', 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0, NULL, NULL, @FirstOfTheMonth, NULL, 0, NULL, NULL, NULL, 0, NULL, NULL, 0),
	(36, N'1FAK0023A', N'01990585-d759-717c-94b9-867b3fde1e4f', N'Wider Custody', 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0, NULL, NULL, @FirstOfTheMonth, NULL, 0, NULL, NULL, NULL, 0, NULL, NULL, 0),
	(37, N'1FAK0028A', N'01990585-d759-717c-94b9-867b3fde1e4f', N'Wider Custody', 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0, NULL, NULL, @FirstOfTheMonth, NULL, 0, NULL, NULL, NULL, 0, NULL, NULL, 0);
	
	SET IDENTITY_INSERT [Mi].[OutcomeQualityDipSampleParticipant] OFF

END