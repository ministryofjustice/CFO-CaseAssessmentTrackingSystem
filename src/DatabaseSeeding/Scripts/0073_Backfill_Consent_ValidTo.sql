-- Backfill Consent.ValidTo for historic records.
-- Historically, adding a new consent did not close off the previous "current" consent, so
-- participants can have multiple consents left open with ValidTo = 9999-12-31.
-- For each participant, order their consents chronologically by ValidFrom and set the ValidTo
-- of every consent (except the most recent one) to the ValidFrom of the consent that follows it.
IF EXISTS (
    SELECT TOP (1) 1
    FROM [Participant].[Consent] c
    WHERE c.[ValidTo] = '99991231'
      AND EXISTS (
          SELECT 1
          FROM [Participant].[Consent] c2
          WHERE c2.[ParticipantId] = c.[ParticipantId]
            AND c2.[ValidFrom] > c.[ValidFrom]
      )
)
BEGIN
    ;WITH OrderedConsents AS (
        SELECT
            c.[ParticipantId],
            c.[Id],
            c.[ValidFrom],
            c.[ValidTo],
            LEAD(c.[ValidFrom]) OVER (PARTITION BY c.[ParticipantId] ORDER BY c.[ValidFrom], c.[Id]) AS [NextValidFrom]
        FROM [Participant].[Consent] c
    )
    UPDATE c
    SET c.[ValidTo] = oc.[NextValidFrom]
    FROM [Participant].[Consent] c
        INNER JOIN OrderedConsents oc
            ON oc.[ParticipantId] = c.[ParticipantId]
            AND oc.[Id] = c.[Id]
    WHERE oc.[NextValidFrom] IS NOT NULL
      AND c.[ValidTo] <> oc.[NextValidFrom];

    -- Report how many rows were updated
    SELECT @@ROWCOUNT AS [Rows Updated];
END;
