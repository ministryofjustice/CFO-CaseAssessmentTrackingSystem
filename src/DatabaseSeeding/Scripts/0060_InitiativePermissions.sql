IF NOT EXISTS (SELECT Id FROM [Identity].[RoleClaim] WHERE ClaimType='Permission' AND ClaimValue='Initiatives')
BEGIN
    INSERT INTO [Identity].[RoleClaim] ([Description], [Group], RoleId, ClaimType, ClaimValue)
    SELECT 'Initiatives', 'Permission', Id, 'Permission', 'Initiatives' FROM [Identity].[Role]
    WHERE [Name] in ('System Support', 'SMT', 'Contract Support Officer', 'Contract Performance Manager', 'Contract Management Process Support Manager')
END

IF NOT EXISTS (SELECT Id FROM [Identity].[RoleClaim] WHERE ClaimType='Permission' AND ClaimValue='Manage Initiatives')
BEGIN
    INSERT INTO [Identity].[RoleClaim] ([Description], [Group], RoleId, ClaimType, ClaimValue)
    SELECT 'Manage Initiatives', 'Permission', Id, 'Permission', 'Manage Initiatives' FROM [Identity].[Role]
    WHERE [Name] in ('System Support', 'SMT', 'Contract Management Process Support Manager')
END
