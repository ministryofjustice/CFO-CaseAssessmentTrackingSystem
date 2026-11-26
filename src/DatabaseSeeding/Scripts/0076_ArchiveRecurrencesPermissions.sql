INSERT INTO [Identity].[RoleClaim] ([Description], [Group], RoleId, ClaimType, ClaimValue)
SELECT 'Archive Recurrences', 'Permission', r.Id, 'Permission', 'ArchiveRecurrences'
FROM [Identity].[Role] AS r
WHERE r.[Name] IN (
    'System Support',
    'SMT',
    'Contract Support Officer',
    'Contract Performance Manager',
    'Contract Management Process Support Manager'
)
AND NOT EXISTS (
    SELECT 1
    FROM [Identity].[RoleClaim] AS rc
    WHERE rc.RoleId = r.Id
      AND rc.ClaimType = 'Permission'
      AND rc.ClaimValue = 'ArchiveRecurrences'
);
