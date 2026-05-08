-- ============================================================
-- Seed: Initiative Objectives
-- Links objectives to initiatives where the objective description
-- starts with the initiative code,
-- e.g. 'IF-25-15 RR Barista Training: 17/09/2025' -> IF-25-15.
-- ============================================================
IF NOT EXISTS(SELECT 1 FROM [Participant].[InitiativeObjective])
BEGIN
    INSERT INTO [Participant].[InitiativeObjective]
        ([Id], [ObjectiveId], [InitiativeId], [ParticipantId], [TenantId], [Created], [CreatedBy])
    SELECT
        NEWID(),
        o.[Id],
        i.[Id],
        pp.[ParticipantId],
        u.[TenantId],
        GETUTCDATE(),
        N'2a9b3450-1feb-4be3-ab94-24e64cd34829'
    FROM        [Participant].[Objective]   o
    JOIN        [Participant].[PathwayPlan] pp ON pp.[Id]   = o.[PathwayPlanId]
    JOIN        [Configuration].[Initiative] i ON o.[Description] LIKE i.[Code] + '%'
    JOIN        [Identity].[User] u ON u.[Id] = o.[CreatedBy]
    WHERE NOT EXISTS (
        SELECT 1
        FROM [Participant].[InitiativeObjective] io
        WHERE io.[ObjectiveId] = o.[Id]
    );
END;