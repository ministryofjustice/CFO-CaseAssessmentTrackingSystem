
IF NOT EXISTS (SELECT TOP (1) [Id] FROM [Configuration].[Tenant])
BEGIN
    
    INSERT INTO Configuration.Tenant (Id, Name, Description, Created, CreatedBy) 
    VALUES 
        (N'1.', N'CFO', N'Root tenant for Creating Future Opportunities', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.', N'CFO Evolution', N'Top level tenant for Evolution Programme', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.1.', N'Achieve', N'Top level tenant for provider Achieve', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.1.1.', N'North West Contract', N'Achieve (North West Contract)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.1.1.1.', N'Achieve_NW', N'Achieve (North West Team)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.1.1.2.', N'Growth_NW', N'Achieve (Growth North West Team)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.2.', N'Ingeus', N'Top level tenant for provider Ingeus', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.2.1.', N'North East Contract', N'Ingeus (North East Contract)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.2.1.1.', N'Ingeus_NE', N'Ingeus (North East)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.2.1.2.', N'AWayOut_NE', N'A way out (North East)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.2.1.3.', N'ForcesEmploymentCharity_NE', N'Forces Employment Charity (North East)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.2.2.', N'West Midlands Contract', N'Ingeus (West Midlands Contract)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),        
        (N'1.1.2.2.1.', N'Ingeus_WM', N'Ingeus (West Midlands)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.2.2.2.', N'AWayOut_WM', N'A way out (West Midlands)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.2.2.3.', N'ChangingLives_WM', N'Changing Lives (West Midlands)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.2.3.', N'East Midlands Contract', N'Ingeus (East Midlands Contract)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.2.3.1.', N'Ingeus_EM', N'Ingeus (East Midlands)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.2.3.2.', N'ChangingLives_EM', N'Changing Lives (East Midlands)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.2.3.3.', N'LAT_EM', N'LAT (East Midlands)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.3.', N'The Growth Co', N'Top level tenant for provider The Growth Co', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.3.1.', N'Yorkshire and Humberside Contract', N'The Growth Co (Yorkshire and Humberside Contract)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.3.1.1.', N'Growth_YH', N'Growth (Yorkshire and Humberside)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.3.1.2.', N'CommunityLinks_YH', N'Community Links (Yorkshire and Humberside)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.3.1.3.', N'StGiles_YH', N'St Giles (Yorkshire and Humberside)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.4.', N'Shaw Trust', N'Top level tenant for Shaw Trust', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.4.1.', N'East of England Contract', N'Shaw Trust (East of England Contract)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.4.1.1.', N'ShawTrust_EE', N'Shaw Trust (East of England)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.4.2.', N'South East Contract', N'Shaw Trust (South East Contract)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.4.2.1.', N'ShawTrust_SE', N'Shaw Trust (South East)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.5.', N'Reed', N'Reed', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.5.1.', N'London Contract', N'Reed (London Contract)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.5.1.1.', N'Reed_LDN', N'Reed (London)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.5.1.2.', N'ShawTrust_LDN', N'Reed (Shaw Trust)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.6.', N'Seetec', N'Seetec', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.6.1.', N'South West Contract', N'Seetec (South West Contract)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),
        (N'1.1.6.1.1.', N'Seetec_SW', N'Seetec (South West)', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829')

END

