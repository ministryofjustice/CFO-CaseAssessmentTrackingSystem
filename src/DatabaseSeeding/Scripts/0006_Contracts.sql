

IF NOT EXISTS (SELECT TOP(1) [Id] FROM [Configuration].[Contract])
BEGIN
    INSERT INTO Configuration.Contract ([Id], [LotNumber], [LifetimeStart], [LifetimeEnd], [Description], [TenantId], [Created], [CreatedBy])
    VALUES 
    (N'con_24036', 1, '01 Aug 2024', N'2029-03-31 23:59:59.0000000', N'North West', N'1.1.1.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
    (N'con_24037', 2, '01 Aug 2024', N'2029-03-31 23:59:59.0000000', N'North East', N'1.1.2.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
    (N'con_24038', 3, '01 Aug 2024', N'2029-03-31 23:59:59.0000000', N'Yorkshire and Humberside', N'1.1.3.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
    (N'con_24041', 4, '01 Aug 2024', N'2029-03-31 23:59:59.0000000', N'West Midlands', N'1.1.2.2.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
    (N'con_24042', 5, '01 Aug 2024', N'2029-03-31 23:59:59.0000000', N'East Midlands', N'1.1.2.3.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
    (N'con_24043', 6, '01 Aug 2024', N'2029-03-31 23:59:59.0000000', N'East Of England', N'1.1.4.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
    (N'con_24044', 7, '01 Aug 2024', N'2029-03-31 23:59:59.0000000', N'London', N'1.1.5.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
    (N'con_24045', 8, '01 Aug 2024', N'2029-03-31 23:59:59.0000000', N'South West', N'1.1.6.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
    (N'con_24046', 9, '01 Aug 2024', N'2029-03-31 23:59:59.0000000', N'South East', N'1.1.4.2.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829');
END


