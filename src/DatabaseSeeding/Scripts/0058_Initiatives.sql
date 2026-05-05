
IF NOT EXISTS (SELECT TOP(1) [Id] FROM [Configuration].[Initiative])
BEGIN
    INSERT INTO [Configuration].[Initiative] ([Id], [Code], [Description], [ContractId], [LifetimeStart], [LifetimeEnd], [Created], [CreatedBy])
    VALUES
    -- Active: started several weeks ago, runs for months
    (N'a1b2c3d4-0001-0001-0001-000000000001', N'IF-01-01', N'Initiative for North West',           N'con_24036', DATEADD(week, -6,  GETUTCDATE()), DATEADD(second, -1, DATEADD(day, 1, CAST(CAST(DATEADD(month,  6, GETUTCDATE()) AS DATE) AS DATETIME))), GETUTCDATE(), N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
    (N'a1b2c3d4-0002-0002-0002-000000000002', N'IF-02-01', N'Initiative for North East',           N'con_24037', DATEADD(week, -4,  GETUTCDATE()), DATEADD(second, -1, DATEADD(day, 1, CAST(CAST(DATEADD(month,  4, GETUTCDATE()) AS DATE) AS DATETIME))), GETUTCDATE(), N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
    (N'a1b2c3d4-0003-0003-0003-000000000003', N'IF-03-01', N'Initiative for Yorkshire',            N'con_24038', DATEADD(week, -2,  GETUTCDATE()), DATEADD(second, -1, DATEADD(day, 1, CAST(CAST(DATEADD(month,  8, GETUTCDATE()) AS DATE) AS DATETIME))), GETUTCDATE(), N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),

    -- Expiring soon: started a few weeks ago, ends within a few days
    (N'a1b2c3d4-0004-0004-0004-000000000004', N'IF-04-01', N'Initiative for West Midlands',        N'con_24041', DATEADD(week, -8,  GETUTCDATE()), DATEADD(second, -1, DATEADD(day, 1, CAST(CAST(DATEADD(day,    3, GETUTCDATE()) AS DATE) AS DATETIME))), GETUTCDATE(), N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
    (N'a1b2c3d4-0005-0005-0005-000000000005', N'IF-05-01', N'Initiative for East Midlands',        N'con_24042', DATEADD(week, -5,  GETUTCDATE()), DATEADD(second, -1, DATEADD(day, 1, CAST(CAST(DATEADD(day,    5, GETUTCDATE()) AS DATE) AS DATETIME))), GETUTCDATE(), N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),

    -- Ended: lifetime is entirely in the past
    (N'a1b2c3d4-0006-0006-0006-000000000006', N'IF-06-01', N'Initiative for London',               N'con_24044', DATEADD(week, -16, GETUTCDATE()), DATEADD(second, -1, DATEADD(day, 1, CAST(CAST(DATEADD(week,  -2, GETUTCDATE()) AS DATE) AS DATETIME))), GETUTCDATE(), N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
    (N'a1b2c3d4-0007-0007-0007-000000000007', N'IF-07-01', N'Initiative for South West',           N'con_24045', DATEADD(week, -20, GETUTCDATE()), DATEADD(second, -1, DATEADD(day, 1, CAST(CAST(DATEADD(week,  -6, GETUTCDATE()) AS DATE) AS DATETIME))), GETUTCDATE(), N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
    (N'a1b2c3d4-0008-0008-0008-000000000008', N'IF-08-01', N'Initiative for South East',           N'con_24046', DATEADD(week, -12, GETUTCDATE()), DATEADD(second, -1, DATEADD(day, 1, CAST(CAST(DATEADD(day,   -3, GETUTCDATE()) AS DATE) AS DATETIME))), GETUTCDATE(), N'2a9b3450-1feb-4be3-ab94-24e64cd34829');
END
