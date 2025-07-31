

IF NOT EXISTS (SELECT * FROM [Identity].[Role] WHERE Id = 'db35e765-8c85-44c7-a569-ce1630b8890e')
BEGIN
    INSERT INTO [Identity].Role (Id, Description, Name, NormalizedName, ConcurrencyStamp, RoleRank) VALUES (N'db35e765-8c85-44c7-a569-ce1630b8890e', N'Contract Performance Manager', N'Performance Manager', N'PERFORMANCE MANAGER', N'83b160ac-8e4a-43b6-9cdc-2e914318ede2', 60)
END


-- ROLE CLAIMS IN

IF NOT EXISTS (SELECT * FROM [Identity].[RoleClaim])
BEGIN
    INSERT INTO [Identity].[RoleClaim] (Description, [Group], RoleId, ClaimType, ClaimValue)
    SELECT 'CSO Functions', '', Id, 'CsoFunctions', ''
    FROM [Identity].[Role]
    WHERE Name IN ('SMT', 'System Support', 'Performance Manager') 
END