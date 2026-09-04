-- Backfill EngagedAtLocationType for existing ParticipantEngagement records
-- This updates all existing records by joining to the Locations table and using CASE to map LocationTypeId to Name
IF EXISTS (SELECT TOP(1) 1 FROM [Mi].[ParticipantEngagement] WHERE [EngagedAtLocationType] = '')
BEGIN
    UPDATE pe
    SET pe.EngagedAtLocationType =
            CASE l.LocationTypeId
                WHEN 0 THEN 'Wing'
                WHEN 1 THEN 'Feeder'
                WHEN 2 THEN 'Outlying'
                WHEN 3 THEN 'Female'
                WHEN 4 THEN 'Community'
                WHEN 5 THEN 'Hub'
                WHEN 6 THEN 'Satellite'
                WHEN 7 THEN 'Unknown'
                WHEN 8 THEN 'Unmapped Custody'
                WHEN 9 THEN 'Unmapped Community'
                ELSE 'Unknown'
                END
    FROM [Mi].[ParticipantEngagement] pe
             INNER JOIN [Configuration].[Location] l
                        ON pe.EngagedAtLocation = l.Name
    WHERE pe.EngagedAtLocationType = ''
       OR pe.EngagedAtLocationType IS NULL;
    -- Report how many rows were updated
    SELECT @@ROWCOUNT AS [Rows Updated];

    SELECT
        EngagedAtLocation,
        EngagedAtLocationType,
        COUNT(*) AS Count
    FROM [Mi].[ParticipantEngagement]
    WHERE EngagedAtLocationType IS NOT NULL AND EngagedAtLocationType <> ''
    GROUP BY EngagedAtLocation, EngagedAtLocationType
    ORDER BY EngagedAtLocation;
END