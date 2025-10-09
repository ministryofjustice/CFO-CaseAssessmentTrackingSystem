

IF NOT EXISTS (SELECT TOP(1) * FROM [Mi].[ParticipantEngagement])
BEGIN

    CREATE TABLE #ActivityCategories
    (
        [Name] NVARCHAR(256) NOT NULL,
        [Id] int NOT NULL
    );

    INSERT INTO #ActivityCategories ([Name], [Id])
    VALUES
    ('Accessing Health Support', 1),
    ('A Future Focus', 2),
    ('A Sense of a New and Pro-Social Identity', 3),
    ('Accommodation Support / Advice', 4),
    ('Addiction', 5),
    ('Anger Management Support', 6),
    ('Applications (Jobs)', 7),
    ('Approved Premises', 8),
    ('Arts and Crafts', 9),
    ('Assessment of Literacy and Numeracy Skills', 10),
    ('Awareness and use of social media / digital platforms', 11),
    ('Building Relationships', 12),
    ('Building Resilience / Confidence', 13),
    ('Business Start-up Support', 14),
    ('Care Leaver support', 15),
    ('Causes of offending', 16),
    ('CBT Ongoing Support', 17),
    ('Changing Lives', 18),
    ('CIAG Support', 19),
    ('Cognitive Behavioural Therapy', 20),
    ('Communication', 21),
    ('Community Capital', 22),
    ('Community Integration', 23),
    ('Completing Applications', 24),
    ('CV preparation', 25),
    ('Debt Advice', 26),
    ('Disclosure to employers / organisations', 27),
    ('Disclosure to families', 28),
    ('Doing Good, to be Good', 29),
    ('Drama', 30),
    ('Driving Test', 31),
    ('Education and Training', 32),
    ('Employment-focused Programmes', 33),
    ('Employment in Community', 34),
    ('Employment on ROTL', 35),
    ('Equality and Diversity', 36),
    ('Everyday English / Practical Maths', 37),
    ('Faith and beliefs (including religion)', 38),
    ('Family Tree', 39),
    ('Feelings of Hope and Self-efficacy', 40),
    ('Financial Literacy', 41),
    ('Healthy Living', 42),
    ('Horticulture Activities', 43),
    ('How to count money/value of money', 44),
    ('How to tell the time', 45),
    ('In-cell learning (custody only)', 46),
    ('Independent Living', 47),
    ('Indirect Restorative Justice (RJ) initiatives', 48),
    ('Industry Specific Cards', 49),
    ('Interventions which are Cognitive Behavioural', 50),
    ('Interview Preparation', 51),
    ('Intro to Digital Literacy', 52),
    ('Introduction to Self-Employment', 53),
    ('Life after military', 54),
    ('Life Skills', 55),
    ('Literacy and Numeracy', 56),
    ('Media', 57),
    ('Mental Health Support', 58),
    ('Mentoring', 59),
    ('Mock Interviews', 60),
    ('Motivation', 61),
    ('Music and Dance', 62),
    ('Neurodiversity awareness and support', 63),
    ('Non-accredited / accredited structured interventions', 64),
    ('Obtaining a NINO', 65),
    ('Obtaining ID', 66),
    ('Opiate Substitution / Psycho-social Therapy', 67),
    ('Parenting Skills', 68),
    ('Parole preparation', 69),
    ('Personal journal', 70),
    ('Practical Support to Access Services', 71),
    ('Problem Solving', 72),
    ('Referral or signposting to other services (external to CFO)', 73),
    ('Referral to Healthcare (including Reconnect Programme)', 74),
    ('Reflective Practice', 75),
    ('Relationship Coaching Interventions', 76),
    ('Release on Temporary Leave (ROTL) support and preparation', 77),
    ('Safeguarding and confidentiality', 78),
    ('Secure Bank Account', 79),
    ('Self-Care', 80),
    ('Self-Isolation', 81),
    ('Sense of Purpose, Meaning and Recognition of Your Worth from others', 82),
    ('Sentence Condition Requirements', 83),
    ('Sexual Health', 84),
    ('Spark Inside', 85),
    ('Specific Projects (Social Good)', 86),
    ('Sports', 87),
    ('Strong Ties to Family & Pro-Social Personal Support', 88),
    ('Sustainable way of life/Intro to Sustainability', 89),
    ('Therapeutic Approaches for Young Adults', 90),
    ('Understanding stress / stress management', 91),
    ('Unpaid Work', 92),
    ('Using Public Transport', 93),
    ('Victim Awareness', 94),
    ('Victim-Offender Conferencing', 95),
    ('Volunteering / Work Experience', 96),
    ('Well-being and Mindfulness', 97),
    ('Wellbeing', 98),
    ('Where to Start and Introduction to Employability', 99),
    ('Women''s World', 100),
    ('Work-Related Mentoring / In Work Support', 101),
    ('CRS Women', 102),
    ('CRS Finance, Benefit and Debt', 103),
    ('CRS Accommodation', 104),
    ('CRS Wellbeing', 105),
    ('CRS Dependency and Recovery', 106),
    ('Employment in Custody', 107);

    CREATE TABLE #ActivityTypes
    (
        [Name] NVARCHAR(32) NOT NULL,
        [Id] int NOT NULL
    );

    INSERT INTO #ActivityTypes ([NAME], [Id])
    VALUES
    ('Community and Social', 0),
    ('Education and Training', 1),
    ('Employment', 2),
    ('Human Citizenship', 3),
    ('ISW Support', 4),
    ('Support Work', 5);

    TRUNCATE TABLE Mi.ParticipantEngagement;

    WITH Engagements AS (
        -- Hub Induction
        SELECT 
            ParticipantId,
            'Took place at ' + Name + ' by ' + DisplayName AS Description, 
            'Hub Induction' as Category,
            InductionDate as EngagedOn,
            EngagedAtLocation,
            EngagedAtContract,
            DisplayName AS EngagedWith,
            TenantName as EngagedWithTenant,
            Created as CreatedOn
        FROM (
            SELECT HI.ParticipantId, HI.InductionDate, L.Name as EngagedAtLocation, C.Description as EngagedAtContract, U.DisplayName, U.TenantName, L.Name, HI.Created
            FROM Induction.HubInduction HI
            INNER JOIN Configuration.Location L on L.Id = HI.LocationId
            INNER JOIN Configuration.Contract C on L.ContractId = C.Id
            INNER JOIN [Identity].[User] U on U.Id = HI.CreatedBy
        ) x

        UNION ALL

        -- Wing Induction
        SELECT 
            ParticipantId,
            'Took place at ' + Name + ' by ' + DisplayName AS Description, 
            'Wing Induction' as Category,
            InductionDate as EngagedOn,
            EngagedAtLocation,
            EngagedAtContract,
            DisplayName AS EngagedWith,
            TenantName as EngagedWithTenant,
            Created as CreatedOn
        FROM (
            SELECT WI.ParticipantId, WI.InductionDate, L.Name as EngagedAtLocation, C.Description as EngagedAtContract, U.DisplayName, U.TenantName, L.Name, WI.Created
            FROM Induction.WingInduction WI
            INNER JOIN Configuration.Location L on L.Id = WI.LocationId
            INNER JOIN Configuration.Contract C on L.ContractId = C.Id
            INNER JOIN [Identity].[User] U on U.Id = WI.CreatedBy
        ) x

        UNION ALL

        -- Assessment
        SELECT 
            ParticipantId,
            'Completed at ' + Name + ' by ' + DisplayName AS Description, 
            'Assessment' as Category,
            EngagedOn,
            EngagedAtLocation,
            EngagedAtContract,
            DisplayName AS EngagedWith,
            TenantName as EngagedWithTenant,
            Completed as CreatedOn
        FROM (
            SELECT PA.ParticipantId, CONVERT(date, PA.Completed) as EngagedOn, L.Name as EngagedAtLocation, C.Description as EngagedAtContract, U.DisplayName, U.TenantName, L.Name, PA.Completed
            FROM Participant.Assessment PA
            INNER JOIN Configuration.Location L on L.Id = PA.LocationId
            INNER JOIN Configuration.Contract C on L.ContractId = C.Id
            INNER JOIN [Identity].[User] U on U.Id = PA.CompletedBy
            WHERE PA.CompletedBy IS NOT NULL
        ) x

        UNION ALL

        -- Activities
        SELECT 
            ParticipantId,
            [Category] + ' at '  + TookPlaceAtName + ' by ' + DisplayName as Description, 
            [Type] as Category,
            EngagedOn,
            EngagedAtLocation,
            EngagedAtContract,
            DisplayName AS EngagedWith,
            TenantName as EngagedWithTenant,
            Created as CreatedOn
        FROM (
            SELECT A.ParticipantId, CONVERT(date, A.CommencedOn) as EngagedOn, L.Name as EngagedAtLocation, C.Description as EngagedAtContract, U.DisplayName, U.TenantName, A.Created, AD.Name as [Category], AT.Name AS [Type], L.Name as TookPlaceAtName 
            FROM Activities.Activity A
            INNER JOIN #ActivityCategories AD on AD.Id = A.Category
            INNER JOIN #ActivityTypes AT on AT.Id = A.Type
            INNER JOIN [Identity].[User] U on U.Id = A.CreatedBy
            INNER JOIN [Configuration].[Location] L ON A.TookPlaceAtLocationId = L.Id
            INNER JOIN Configuration.Contract C on L.ContractId = C.Id
        ) x
    )
    INSERT INTO Mi.ParticipantEngagement
        (Id, ParticipantId, Description, Category, EngagedOn, EngagedAtLocation, EngagedAtContract, EngagedWith, EngagedWithTenant, CreatedOn)
    SELECT NEWID(), ParticipantId, Description, Category, EngagedOn, EngagedAtLocation, EngagedAtContract, EngagedWith, EngagedWithTenant, CreatedOn
    FROM Engagements

    DROP TABLE #ActivityCategories
    DROP TABLE #ActivityTypes

END