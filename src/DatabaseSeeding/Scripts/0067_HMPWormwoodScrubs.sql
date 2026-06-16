
-- Add HMP Wormwood Scrubs as a London (con_24044) location
IF NOT EXISTS (SELECT 1 FROM [Configuration].[Location] WHERE [Name] = N'HMP Wormwood Scrubs')
BEGIN
    INSERT INTO [Configuration].[Location] (Name, ContractId, LifetimeStart, LifetimeEnd, ParentLocationId, GenderProvisionId, LocationTypeId, Created, CreatedBy)
    VALUES (N'HMP Wormwood Scrubs', N'con_24044', '12 Jun 2026', N'2029-03-31 23:59:59.0000000', null, 0, 1, GETUTCDATE(), N'2a9b3450-1feb-4be3-ab94-24e64cd34829');
END;

-- Map HMP Wormwood Scrubs to tenant 1.1.5.1.1.
IF NOT EXISTS (
    SELECT 1 FROM [Configuration].[TenantLocation] tl
    INNER JOIN [Configuration].[Location] l ON tl.LocationId = l.Id
    WHERE l.[Name] = N'HMP Wormwood Scrubs' AND tl.TenantId = N'1.1.5.1.1.'
)
BEGIN
    INSERT INTO [Configuration].[TenantLocation] (LocationId, TenantId)
    SELECT Id, N'1.1.5.1.1.'
    FROM [Configuration].[Location]
    WHERE [Name] = N'HMP Wormwood Scrubs';
END;

-- Update the DMS location mapping for WSI (Wormwood Scrubs) to reference the new location
UPDATE [Dms].[LocationMapping]
SET LocationId = (SELECT Id FROM [Configuration].[Location] WHERE [Name] = N'HMP Wormwood Scrubs')
WHERE Code = N'WSI' 
    AND ISNULL(LocationId, -1) = -1;