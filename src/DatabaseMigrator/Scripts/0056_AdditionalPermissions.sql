

IF NOT EXISTS (SELECT Id FROM [Identity].[RoleClaim] WHERE ClaimType='Permission' AND ClaimValue='PQA')
BEGIN
    INSERT INTO [Identity].[RoleClaim] ([Description], [Group], RoleId, ClaimType, ClaimValue) 
    SELECT 'Provider QA', 'Permission', Id, 'Permission', 'PQA' FROM [Identity].[Role]
    WHERE [Name] in ('System Support', 'SMT', 'QA + Finance', 'CFO QA Manager', 'CFO QA Support Manager')
END

IF NOT EXISTS (SELECT Id FROM [Identity].[RoleClaim] WHERE ClaimType='Permission' AND ClaimValue='QA1')
BEGIN
    INSERT INTO [Identity].[RoleClaim] ([Description], [Group], RoleId, ClaimType, ClaimValue)
    SELECT 'First Pass QA', 'Permission', Id, 'Permission', 'QA1' FROM [Identity].[Role]
    WHERE [Name] in ('System Support', 'SMT', 'CFO QA Manager', 'CFO QA Support Manager', 'CFO QA Officer')
END

IF NOT EXISTS (SELECT Id FROM [Identity].[RoleClaim] WHERE ClaimType='Permission' AND ClaimValue='User Management')
BEGIN
    INSERT INTO [Identity].[RoleClaim] ([Description], [Group], RoleId, ClaimType, ClaimValue)
    SELECT 'User Management', 'Permission', Id, 'Permission', 'User Management' FROM [Identity].[Role]
    WHERE [Name] in ('System Support', 'SMT', 'CFO QA Manager', 'CFO QA Support Manager', 'CFO QA Officer')
END

IF NOT EXISTS (SELECT Id FROM [Identity].[RoleClaim] WHERE ClaimType='Permission' AND ClaimValue='QA2')
BEGIN
    INSERT INTO [Identity].[RoleClaim] ([Description], [Group], RoleId, ClaimType, ClaimValue)
    SELECT 'Second Pass QA', 'Permission', Id, 'Permission', 'QA2' FROM [Identity].[Role]
    WHERE [Name] in ('System Support', 'SMT', 'CFO QA Manager', 'CFO QA Support Manager')
END

IF NOT EXISTS (SELECT Id FROM [Identity].[RoleClaim] WHERE ClaimType='Permission' AND ClaimValue='Senior Internal')
BEGIN
    INSERT INTO [Identity].[RoleClaim] ([Description], [Group], RoleId, ClaimType, ClaimValue)
    SELECT 'Senior Internal Users', 'Permission', Id, 'Permission', 'Senior Internal' FROM [Identity].[Role]
    WHERE [Name] in ('System Support', 'SMT', 'CFO QA Manager')
END

IF NOT EXISTS (SELECT Id FROM [Identity].[RoleClaim] WHERE ClaimType='Permission' AND ClaimValue='Outcome Quality Dip Checks')
BEGIN
    INSERT INTO [Identity].[RoleClaim] ([Description], [Group], RoleId, ClaimType, ClaimValue)
    SELECT 'Outcome Quality Dip Checks', 'Permission', Id, 'Permission', 'Outcome Quality Dip Checks' FROM [Identity].[Role]
    WHERE [Name] in ('System Support', 'SMT', 'Contract Support Officer', 'Contract Performance Manager', 'Contract Management Process Support Manager')
END

IF NOT EXISTS (SELECT Id FROM [Identity].[RoleClaim] WHERE ClaimType='Permission' AND ClaimValue='Outcome Quality Dip Review')
BEGIN
    INSERT INTO [Identity].[RoleClaim] ([Description], [Group], RoleId, ClaimType, ClaimValue)
    SELECT 'Outcome Quality Dip Review', 'Permission', Id, 'Permission', 'Outcome Quality Dip Review' FROM [Identity].[Role]
    WHERE [Name] in ('System Support', 'SMT', 'Contract Support Officer', 'Contract Management Process Support Manager')
END

IF NOT EXISTS (SELECT Id FROM [Identity].[RoleClaim] WHERE ClaimType='Permission' AND ClaimValue='Outcome Quality Dip Verification')
BEGIN
    INSERT INTO [Identity].[RoleClaim] ([Description], [Group], RoleId, ClaimType, ClaimValue)
    SELECT 'Outcome Quality Dip Verification', 'Permission', Id, 'Permission', 'Outcome Quality Dip Verification' FROM [Identity].[Role]
    WHERE [Name] in ('System Support', 'SMT', 'Contract Performance Manager', 'Contract Management Process Support Manager')
END

IF NOT EXISTS (SELECT Id FROM [Identity].[RoleClaim] WHERE ClaimType='Permission' AND ClaimValue='Outcome Quality Dip Finalise')
BEGIN
    INSERT INTO [Identity].[RoleClaim] ([Description], [Group], RoleId, ClaimType, ClaimValue)
    SELECT 'Outcome Quality Dip Finalise', 'Permission', Id, 'Permission', 'Outcome Quality Dip Finalise' FROM [Identity].[Role]
    WHERE [Name] in ('System Support', 'SMT', 'Contract Management Process Support Manager')
END

IF NOT EXISTS (SELECT Id FROM [Identity].[RoleClaim] WHERE ClaimType='Permission' AND ClaimValue='Contract Data')
BEGIN
    INSERT INTO [Identity].[RoleClaim] ([Description], [Group], RoleId, ClaimType, ClaimValue)
    SELECT 'Contract Data', 'Permission', Id, 'Permission', 'Contract Data' FROM [Identity].[Role]
    WHERE [Name] in ('System Support', 'SMT', 'CFO QA Manager')
END

IF NOT EXISTS (SELECT Id FROM [Identity].[RoleClaim] WHERE ClaimType='Permission' AND ClaimValue='Reassign')
BEGIN
    INSERT INTO [Identity].[RoleClaim] ([Description], [Group], RoleId, ClaimType, ClaimValue)
    SELECT 'Reassign', 'Permission', Id, 'Permission', 'Reassign' FROM [Identity].[Role]
    WHERE [Name] in ('QA + Finance', 'CFO QA Officer', 'CFO QA Support Manager', 'System Support', 'SMT', 'CFO QA Manager')
END

IF NOT EXISTS (SELECT Id FROM [Identity].[RoleClaim] WHERE ClaimType='Permission' AND ClaimValue='Transfers')
BEGIN
    INSERT INTO [Identity].[RoleClaim] ([Description], [Group], RoleId, ClaimType, ClaimValue)
    SELECT 'Transfers', 'Permission', Id, 'Permission', 'Transfers' FROM [Identity].[Role]
    WHERE [Name] in ('QA + Finance', 'CFO QA Officer', 'CFO QA Support Manager', 'Contract Support Officer' ,'Contract Performance Manager' , 'System Support', 'SMT', 'CFO QA Manager')
END

IF NOT EXISTS (SELECT Id FROM [Identity].[RoleClaim] WHERE ClaimType='Permission' AND ClaimValue='Finance')
BEGIN
    INSERT INTO [Identity].[RoleClaim] ([Description], [Group], RoleId, ClaimType, ClaimValue)
    SELECT 'Finance', 'Permission', Id, 'Permission', 'Finance' FROM [Identity].[Role]
    WHERE [Name] in ('Finance', 'System Support', 'SMT', 'CFO QA Manager')
END

IF NOT EXISTS (SELECT Id FROM [Identity].[RoleClaim] WHERE ClaimType='Permission' AND ClaimValue='Service Desk Management')
BEGIN
    INSERT INTO [Identity].[RoleClaim] ([Description], [Group], RoleId, ClaimType, ClaimValue)
    SELECT 'Service Desk Management', 'Permission', Id, 'Permission', 'Service Desk Management' FROM [Identity].[Role]
    WHERE [Name] in ('System Support', 'SMT', 'CFO QA Manager', 'CFO QA Support Manager')
END