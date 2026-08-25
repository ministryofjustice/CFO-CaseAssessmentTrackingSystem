
IF NOT EXISTS (SELECT TOP(1) [Id] FROM [Mi].[ContractTarget])
BEGIN

    DECLARE @Targets TABLE (
        ContractId NVARCHAR(12) NOT NULL,
        Prison INT, Community INT, Wings INT, Hubs INT, PreReleaseSupport INT, ThroughTheGate INT,
        SupportWork INT, HumanCitizenship INT, CommunityAndSocial INT, Interventions INT,
        Employment INT, TrainingAndEducation INT
    );

    INSERT INTO @Targets VALUES
        (N'con_24036', 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10),  -- North West
        (N'con_24037', 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10),  -- North East
        (N'con_24038', 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10),  -- Yorkshire and Humberside
        (N'con_24041', 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10),  -- West Midlands
        (N'con_24042', 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10),  -- East Midlands
        (N'con_24043', 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10),  -- East Of England
        (N'con_24044', 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10),  -- London
        (N'con_24045', 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10),  -- South West
        (N'con_24046', 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10);  -- South East    

    -- Build the list of months from Jan 2025 to Apr 2029 inclusive.
    ;WITH months AS (
        SELECT CAST('2025-01-01' AS DATE) AS TheMonth
        UNION ALL
        SELECT DATEADD(MONTH, 1, TheMonth) FROM months WHERE TheMonth < '2029-04-01'
    )
    INSERT INTO [Mi].[ContractTarget]
        ([Id], [ContractId], [Year], [Month], [Prison], [Community], [Wings], [Hubs],
         [PreReleaseSupport], [ThroughTheGate], [SupportWork], [HumanCitizenship],
         [CommunityAndSocial], [Interventions], [Employment], [TrainingAndEducation])
    SELECT
        NEWID(),
        t.ContractId,
        YEAR(m.TheMonth),
        MONTH(m.TheMonth),
        t.Prison, t.Community, t.Wings, t.Hubs, t.PreReleaseSupport, t.ThroughTheGate,
        t.SupportWork, t.HumanCitizenship, t.CommunityAndSocial, t.Interventions,
        t.Employment, t.TrainingAndEducation
    FROM months m
    CROSS APPLY (
        SELECT * FROM @Targets
    ) t
    OPTION (MAXRECURSION 0);

END
