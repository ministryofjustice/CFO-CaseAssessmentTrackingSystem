-- ============================================================
-- Seed: Innovation Fund Initiatives
--
-- Contract mapping
--   North East            -> con_24037
--   Yorkshire & Humber    -> con_24038
--   West Midlands         -> con_24041
--   East Midlands         -> con_24042
--   East of England       -> con_24043
--   London                -> con_24044
--   South East            -> con_24046
-- ============================================================
IF NOT EXISTS(SELECT 1 FROM [Configuration].[Initiative])
BEGIN
    INSERT INTO [Configuration].[Initiative]
        ([Id], [Code], [Description], [ContractId], [LifetimeStart], [LifetimeEnd], [Created], [CreatedBy])
    SELECT
        v.[Id], 
        v.[Code], 
        v.[Description], 
        v.[ContractId],
        v.[LifetimeStart], 
        v.[LifetimeEnd],
        GETUTCDATE(), 
        N'2a9b3450-1feb-4be3-ab94-24e64cd34829' -- System Support
    FROM (VALUES
        -- London
        (N'c1a2b3c4-0001-0001-0001-000000000001', N'IF-25-15', N'RR Barista Training',                          N'con_24044', '2025-12-01 00:00:00', '2026-11-30 23:59:59'),

        -- East of England
        (N'c1a2b3c4-0002-0002-0002-000000000002', N'IF-25-24', N'Tattoo Course',                                N'con_24043', '2025-12-01 00:00:00', '2026-09-30 23:59:59'),

        -- West Midlands
        (N'c1a2b3c4-0003-0003-0003-000000000003', N'IF-25-21', N'Inside Academy',                               N'con_24041', '2026-01-19 00:00:00', '2026-07-31 23:59:59'),
        (N'c1a2b3c4-0004-0004-0004-000000000004', N'IF-25-22', N'Making Changes',                               N'con_24041', '2026-01-15 00:00:00', '2026-07-31 23:59:59'),
        (N'c1a2b3c4-0012-0012-0012-000000000012', N'IF-26-05', N'New Tracks',                                   N'con_24041', '2026-02-24 00:00:00', '2026-05-29 23:59:59'),

        -- South East
        (N'c1a2b3c4-0005-0005-0005-000000000005', N'IF-25-23', N'BearFace Theatre',                             N'con_24046', '2026-01-08 00:00:00', '2027-01-31 23:59:59'),
        (N'c1a2b3c4-0008-0008-0008-000000000008', N'IF-25-29', N'Making It Out',                                N'con_24046', '2026-02-16 00:00:00', '2027-03-16 23:59:59'),

        -- East Midlands
        (N'c1a2b3c4-0006-0006-0006-000000000006', N'IF-25-01', N'Seeds of Change',                              N'con_24042', '2026-01-02 00:00:00', '2027-03-31 23:59:59'),

        -- North East
        (N'c1a2b3c4-0007-0007-0007-000000000007', N'IF-25-26', N'Doncaster Rugby',                              N'con_24037', '2026-01-07 00:00:00', '2027-04-30 23:59:59'),
        (N'c1a2b3c4-0010-0010-0010-000000000010', N'IF-25-35', N'Understanding Me',                             N'con_24037', '2026-01-05 00:00:00', '2027-04-30 23:59:59'),
        (N'c1a2b3c4-0011-0011-0011-000000000011', N'IF-26-04', N'My Sister''s Place',                           N'con_24037', '2026-02-01 00:00:00', '2027-03-31 23:59:59'),
        (N'c1a2b3c4-0014-0014-0014-000000000014', N'IF-25-18', N'Garden Renovation',                            N'con_24037', '2025-09-18 00:00:00', '2026-02-28 23:59:59'),
        (N'c1a2b3c4-0015-0015-0015-000000000015', N'IF-25-27', N'Maizel''s Method',                             N'con_24037', '2026-01-02 00:00:00', '2026-03-31 23:59:59'),

        -- Yorkshire & Humber
        (N'c1a2b3c4-0009-0009-0009-000000000009', N'IF-25-34', N'CSCS',                                         N'con_24038', '2026-03-02 00:00:00', '2027-03-31 23:59:59'),
        (N'c1a2b3c4-0013-0013-0013-000000000013', N'IF-25-17', N'Doncaster RLFC Personal Development Programme', N'con_24038', '2025-09-23 00:00:00', '2026-06-20 23:59:59')

    ) AS v([Id], [Code], [Description], [ContractId], [LifetimeStart], [LifetimeEnd])
END