

IF NOT EXISTS (SELECT TOP (1) [Id] FROM [Mi].[OutcomeQualityDipSample])
BEGIN

	INSERT INTO [Mi].[OutcomeQualityDipSample] ([Id], [ContractId], [CreatedOn], [PeriodFrom], [PeriodTo], [ReviewedOn], [ReviewedBy], [Status], [CpmCompliant], [CpmPercentage], [CpmScore], [Created], [CreatedBy], [CsoCompliant], [CsoPercentage], [CsoScore], [FinalCompliant], [FinalPercentage], [FinalScore], [LastModified], [LastModifiedBy], [Size], [VerifiedBy], [VerifiedOn], [DocumentId]) 
	VALUES 
	(N'01990585-d759-717c-94b9-867b3fde1e4f', N'con_24038', '20250901 13:44:41.5618738', '20250901 00:00:00.0000000', '20250930 23:59:59.9999999', NULL, NULL, 0, NULL, NULL, NULL, '20250901 13:44:41.6864448', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 5, NULL, NULL, NULL),
	(N'01990585-d759-7a75-bc0b-b09567674a50', N'con_24043', '20250901 13:44:41.5616426', '20250901 00:00:00.0000000', '20250930 23:59:59.9999999', '20250901 13:46:06.0484262', N'9fe4e2f0-506a-4870-b3a2-a8e09fa2e73b', 5, 2, 100, 5, '20250901 13:44:41.6862246', NULL, 2, 100, 5, 2, 100, 5, '20250901 13:47:30.1552600', NULL, 2, N'1afd5d4c-4ca6-440a-ad0a-3d59f03719ef', '20250901 13:47:00.2186514', N'01990588-6980-7332-ac13-282e549f1cf9');
    
	SET IDENTITY_INSERT [Mi].[OutcomeQualityDipSampleParticipant] ON
	
	INSERT INTO [Mi].[OutcomeQualityDipSampleParticipant] ([Id], [ParticipantId], [DipSampleId], [LocationType], [HasClearParticipantJourney], [ShowsTaskProgression], [TTGDemonstratesGoodPRIProcess], [SupportsJourney], [CsoComments], [LastModified], [FinalReviewedBy], [CpmComments], [CpmIsCompliant], [CpmReviewedBy], [CpmReviewedOn], [Created], [CreatedBy], [CsoIsCompliant], [CsoReviewedBy], [CsoReviewedOn], [FinalComments], [FinalIsCompliant], [FinalReviewedOn], [LastModifiedBy], [AlignsWithDoS]) 
	VALUES 
	(31, N'1CFG1553F', N'01990585-d759-7a75-bc0b-b09567674a50', N'Wider Custody', 1, 1, 1, 1, N'werwer', '20250901 13:47:00.2192796', N'1afd5d4c-4ca6-440a-ad0a-3d59f03719ef', N'Looks good to me', 1, N'1afd5d4c-4ca6-440a-ad0a-3d59f03719ef', '20250901 13:46:40.0907780', '20250901 13:44:41.6865382', NULL, 1, N'9fe4e2f0-506a-4870-b3a2-a8e09fa2e73b', '20250901 13:46:02.6176504', N'Looks good to me', 1, '20250901 13:47:00.2185169', NULL, 1),
	(32, N'1CFG5469L', N'01990585-d759-7a75-bc0b-b09567674a50', N'Wider Custody', 1, 1, 1, 1, N'qweqwe', '20250901 13:47:00.2192797', N'1afd5d4c-4ca6-440a-ad0a-3d59f03719ef', N'Agree', 1, N'1afd5d4c-4ca6-440a-ad0a-3d59f03719ef', '20250901 13:46:31.8127581', '20250901 13:44:41.6866324', NULL, 1, N'9fe4e2f0-506a-4870-b3a2-a8e09fa2e73b', '20250901 13:45:52.4281639', N'Agree', 1, '20250901 13:47:00.2186361', NULL, 1),
	(33, N'1CFG4261Y', N'01990585-d759-717c-94b9-867b3fde1e4f', N'Wider Custody', 2, 1, 1, 2, N'testing filters', '20251001 11:05:17.7196947', NULL, NULL, 0, NULL, NULL, '20250901 13:44:41.6866473', NULL, 1, N'2a9b3450-1feb-4be3-ab94-24e64cd34829', '20251001 11:05:17.7065886', NULL, 0, NULL, N'2a9b3450-1feb-4be3-ab94-24e64cd34829', 1),
	(34, N'1CFG2457U', N'01990585-d759-717c-94b9-867b3fde1e4f', N'Wider Custody', 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0, NULL, NULL, '20250901 13:44:41.6866476', NULL, 0, NULL, NULL, NULL, 0, NULL, NULL, 0),
	(35, N'1CFG2622V', N'01990585-d759-717c-94b9-867b3fde1e4f', N'Wider Custody', 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0, NULL, NULL, '20250901 13:44:41.6866489', NULL, 0, NULL, NULL, NULL, 0, NULL, NULL, 0),
	(36, N'1CFG8432I', N'01990585-d759-717c-94b9-867b3fde1e4f', N'Wider Custody', 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0, NULL, NULL, '20250901 13:44:41.6866493', NULL, 0, NULL, NULL, NULL, 0, NULL, NULL, 0),
	(37, N'1CFG9960M', N'01990585-d759-717c-94b9-867b3fde1e4f', N'Wider Custody', 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0, NULL, NULL, '20250901 13:44:41.6866495', NULL, 0, NULL, NULL, NULL, 0, NULL, NULL, 0);
	
	SET IDENTITY_INSERT [Mi].[OutcomeQualityDipSampleParticipant] OFF

END