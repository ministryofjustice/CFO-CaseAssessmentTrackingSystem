
IF NOT EXISTS (SELECT TOP(1) [ObjectiveId] FROM [Participant].[InitiativeObjective])
BEGIN
    -- Link a selection of non-mandatory objectives to seeded initiatives.
    -- ObjectiveId references [Participant].[Objective]; ParticipantId is denormalised from the owning PathwayPlan.
    INSERT INTO [Participant].[InitiativeObjective] ([Id], [ObjectiveId], [InitiativeId], [ParticipantId], [Created], [CreatedBy])
    VALUES
    -- Active initiatives
    (NEWID(), N'0195d736-5840-7a7b-be4a-17deff70eb3a', N'a1b2c3d4-0001-0001-0001-000000000001', N'1CFG5437L', GETUTCDATE(), N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),  -- EDUCATION / IF-01-01 North West
    (NEWID(), N'0195d747-dbd9-7daa-9127-44196616b95c', N'a1b2c3d4-0002-0002-0002-000000000002', N'1CFG9326Y', GETUTCDATE(), N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),  -- preparing for work / IF-02-01 North East
    (NEWID(), N'0195d77a-a8fa-7782-a761-205d33ec59c2', N'a1b2c3d4-0003-0003-0003-000000000003', N'1CFG4936J', GETUTCDATE(), N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),  -- testing testing / IF-03-01 Yorkshire

    -- Expiring soon
    (NEWID(), N'0195f145-803e-7c6c-baa3-42c034679203', N'a1b2c3d4-0004-0004-0004-000000000004', N'1CFG4261Y', GETUTCDATE(), N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),  -- housing support / IF-04-01 West Midlands
    (NEWID(), N'01968c43-6fc9-72d3-8128-5a16cb8affb9', N'a1b2c3d4-0005-0005-0005-000000000005', N'1CFG6390M', GETUTCDATE(), N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),  -- working towards CSCS / IF-05-01 East Midlands

    -- Ended initiatives
    (NEWID(), N'01989949-8c6e-7a6c-9fb1-5e103061309a', N'a1b2c3d4-0006-0006-0006-000000000006', N'1CFG9798U', GETUTCDATE(), N'2a9b3450-1feb-4be3-ab94-24e64cd34829'),  -- HEALTH & ADDICTION / IF-06-01 London
    (NEWID(), N'01995708-b707-7f59-a3fc-5077ac018cf2', N'a1b2c3d4-0007-0007-0007-000000000007', N'1CFG8236D', GETUTCDATE(), N'2a9b3450-1feb-4be3-ab94-24e64cd34829');  -- HEALTH & ADDICTION / IF-07-01 South West
END
