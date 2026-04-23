-- =============================================================================
-- FIX: Backfill missing MI.ProviderFeedbackActivity / ProviderFeedbackEnrolment
--      rows and remove duplicate entries sourced from internal notes.
--
-- Background:
--   Missing rows were caused by a broken ContractId lookup in the original
--   consumers. IsExternal was introduced after initial implementation and some
--   note records were never updated, leaving sole-note queue entries with
--   IsExternal = 0 and therefore no MI record.
--
-- Execution order:
--   Steps 1–4: UPDATE IsExternal = 1 on note records that are the sole note
--              for a completed queue entry. Safe to flip because there is no
--              other note on the entry — no internal content is exposed.
--   Steps 5–6: DELETE corrupt MI rows sourced from internal notes where an
--              external note also exists (genuine duplicates from before
--              IsExternal filtering was enforced).
--   Steps 7–10: INSERT missing MI rows using the now-corrected note data.
--   Step 11: Correct Provider QA User, Provider QA User submitted date etc.
--            for Enrolment and Activity feedback where incorrect (moved from migration script)
-- =============================================================================

SET NOCOUNT ON;
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

DECLARE @MaxExpectedRows INT = 10000; -- Safety ceiling per step; tune if data volumes differ significantly
DECLARE @rc              INT;

BEGIN TRANSACTION;
BEGIN TRY


-- -------------------------------------------------------
-- 1.  Set IsExternal = 1 on sole Activity QA2 notes
-- -------------------------------------------------------
UPDATE n
SET    n.IsExternal = 1
FROM   Activities.Qa2QueueNote       n
JOIN   Activities.ActivityQa2Queue   q  ON q.Id = n.ActivityQa2QueueEntryId
JOIN   Activities.Activity           a  ON a.Id = q.ActivityId
WHERE  q.IsCompleted = 1
    AND q.IsAccepted = 0
    AND q.IsEscalated = 0
  AND  n.IsExternal  = 0
  AND  a.Definition  BETWEEN 266 AND 300
  AND  NOT EXISTS (
           SELECT 1 FROM Activities.Qa2QueueNote n2
           WHERE  n2.ActivityQa2QueueEntryId = q.Id
             AND  n2.Id <> n.Id
       );

SET @rc = @@ROWCOUNT;
IF @rc > @MaxExpectedRows
    THROW 50001, 'Step 1: UPDATE row count exceeds expected threshold -- aborting.', 1;

-- -------------------------------------------------------
-- 2.  Set IsExternal = 1 on sole Activity Escalation notes
-- -------------------------------------------------------
UPDATE n
SET    n.IsExternal = 1
FROM   Activities.EscalationNote           n
JOIN   Activities.ActivityEscalationQueue  q  ON q.Id = n.ActivityEscalationQueueEntryId
JOIN   Activities.Activity                 a  ON a.Id = q.ActivityId
WHERE  q.IsCompleted = 1
  AND  q.IsAccepted = 0
  AND  n.IsExternal  = 0
  AND  a.Definition  BETWEEN 266 AND 300
  AND  NOT EXISTS (
           SELECT 1 FROM Activities.EscalationNote n2
           WHERE  n2.ActivityEscalationQueueEntryId = q.Id
             AND  n2.Id <> n.Id
       );

SET @rc = @@ROWCOUNT;
IF @rc > @MaxExpectedRows
    THROW 50002, 'Step 2: UPDATE row count exceeds expected threshold -- aborting.', 1;

-- -------------------------------------------------------
-- 3.  Set IsExternal = 1 on sole Enrolment QA2 notes
-- -------------------------------------------------------
UPDATE n
SET    n.IsExternal = 1
FROM   Enrolment.Qa2QueueNote    n
JOIN   Enrolment.Qa2Queue        q  ON q.Id = n.EnrolmentQa2QueueEntryId
WHERE  q.IsCompleted = 1
  AND  q.IsAccepted = 0
  AND  q.IsEscalated = 0
  AND  n.IsExternal  = 0
  AND  NOT EXISTS (
           SELECT 1 FROM Enrolment.Qa2QueueNote n2
           WHERE  n2.EnrolmentQa2QueueEntryId = q.Id
             AND  n2.Id <> n.Id
       );

SET @rc = @@ROWCOUNT;
IF @rc > @MaxExpectedRows
    THROW 50003, 'Step 3: UPDATE row count exceeds expected threshold -- aborting.', 1;

-- -------------------------------------------------------
-- 4.  Set IsExternal = 1 on sole Enrolment Escalation notes
-- -------------------------------------------------------
UPDATE n
SET    n.IsExternal = 1
FROM   Enrolment.EscalationNote      n
JOIN   Enrolment.EscalationQueue     q  ON q.Id = n.EnrolmentEscalationQueueEntryId
WHERE  q.IsCompleted = 1
  AND  q.IsAccepted = 0
  AND  n.IsExternal  = 0
  AND  NOT EXISTS (
           SELECT 1 FROM Enrolment.EscalationNote n2
           WHERE  n2.EnrolmentEscalationQueueEntryId = q.Id
             AND  n2.Id <> n.Id
       );

SET @rc = @@ROWCOUNT;
IF @rc > @MaxExpectedRows
    THROW 50004, 'Step 4: UPDATE row count exceeds expected threshold -- aborting.', 1;


-- -------------------------------------------------------
-- 5.  Delete ProviderFeedbackActivity rows sourced from
--     internal notes where an external note also exists
-- -------------------------------------------------------
-- -------------------------------------------------------

DELETE pfa
FROM  Mi.ProviderFeedbackActivity   pfa
JOIN  Activities.Qa2QueueNote       n
        ON  n.Id                        = pfa.NoteId
        AND n.ActivityQa2QueueEntryId   = pfa.QueueEntryId
WHERE pfa.SourceTable = 'Activities.Qa2QueueNote'
  AND n.IsExternal    = 0
  AND EXISTS (
        SELECT 1 FROM Activities.Qa2QueueNote n2
        WHERE  n2.ActivityQa2QueueEntryId = pfa.QueueEntryId
          AND  n2.IsExternal = 1
      );


DELETE pfa
FROM  Mi.ProviderFeedbackActivity   pfa
JOIN  Activities.EscalationNote     n
        ON  n.Id                             = pfa.NoteId
        AND n.ActivityEscalationQueueEntryId = pfa.QueueEntryId
WHERE pfa.SourceTable = 'Activities.EscalationNote'
  AND n.IsExternal    = 0
  AND EXISTS (
        SELECT 1 FROM Activities.EscalationNote n2
        WHERE  n2.ActivityEscalationQueueEntryId = pfa.QueueEntryId
          AND  n2.IsExternal = 1
      );



-- -------------------------------------------------------
-- 6.  Delete ProviderFeedbackEnrolment rows sourced from
--     internal notes where an external note also exists
-- -------------------------------------------------------

DELETE pfe
FROM  Mi.ProviderFeedbackEnrolment  pfe
JOIN  Enrolment.Qa2QueueNote        n
        ON  n.Id                        = pfe.NoteId
        AND n.EnrolmentQa2QueueEntryId  = pfe.QueueEntryId
WHERE pfe.SourceTable = 'Enrolment.Qa2QueueNote'
  AND n.IsExternal    = 0
  AND EXISTS (
        SELECT 1 FROM Enrolment.Qa2QueueNote n2
        WHERE  n2.EnrolmentQa2QueueEntryId = pfe.QueueEntryId
          AND  n2.IsExternal = 1
      );


DELETE pfe
FROM  Mi.ProviderFeedbackEnrolment  pfe
JOIN  Enrolment.EscalationNote      n
        ON  n.Id                              = pfe.NoteId
        AND n.EnrolmentEscalationQueueEntryId = pfe.QueueEntryId
WHERE pfe.SourceTable = 'Enrolment.EscalationNote'
  AND n.IsExternal    = 0
  AND EXISTS (
        SELECT 1 FROM Enrolment.EscalationNote n2
        WHERE  n2.EnrolmentEscalationQueueEntryId = pfe.QueueEntryId
          AND  n2.IsExternal = 1
      );



-- -------------------------------------------------------
-- 7.  Insert missing ProviderFeedbackActivity rows  (Advisory)
-- -------------------------------------------------------
;WITH ActivityAdvisorySource AS
(
    SELECT
        q.Id                        AS QueueEntryId,
        n.Id                        AS NoteId,
        'Activities.Qa2QueueNote'   AS SourceTable,
        'QA2'                       AS Queue,
        q.TenantId,
        q.ParticipantId,
        q.SupportWorkerId,
        ISNULL(pqa.LastModifiedBy, '') AS ProviderQaUserId,
        ISNULL(n.CreatedBy,        '') AS CfoUserId,
        pqa.LastModified AS PqaSubmittedDate,
        n.Created AS ActionDate,
        n.Message,
        n.FeedbackType,
        n.IsExternal,
        q.LastModified,
        CAST(a.Id AS NVARCHAR(36))  AS ActivityId,
        a.Type                      AS ActivityType,
        a.TookPlaceAtContractId     AS ContractId
    FROM       Activities.ActivityQa2Queue   q
    JOIN       Activities.Qa2QueueNote       n   ON n.ActivityQa2QueueEntryId = q.Id
    JOIN       Activities.Activity           a   ON a.Id  = q.ActivityId
    OUTER APPLY (
        SELECT TOP 1 pqa2.LastModifiedBy, pqa2.LastModified
        FROM   Activities.ActivityPqaQueue pqa2
        WHERE  pqa2.ActivityId   = q.ActivityId
          AND  pqa2.LastModified < q.Created
        ORDER BY pqa2.LastModified DESC
    ) pqa
    WHERE q.IsCompleted = 1
      AND q.IsAccepted  = 1
      AND (
            n.IsExternal = 1
            OR NOT EXISTS (
                SELECT 1 FROM Activities.Qa2QueueNote n2
                WHERE  n2.ActivityQa2QueueEntryId = q.Id AND n2.IsExternal = 1
            )
          )
      AND n.FeedbackType IN (0, 1)

    UNION ALL

    SELECT
        q.Id                            AS QueueEntryId,
        n.Id                            AS NoteId,
        'Activities.EscalationNote'     AS SourceTable,
        'Escalation'                    AS Queue,
        q.TenantId,
        q.ParticipantId,
        q.SupportWorkerId,
        ISNULL(pqa.LastModifiedBy, '') AS ProviderQaUserId,
        ISNULL(n.CreatedBy,        '') AS CfoUserId,
        pqa.LastModified AS PqaSubmittedDate,
        n.Created AS ActionDate,
        n.Message,
        n.FeedbackType,
        n.IsExternal,
        q.LastModified,
        CAST(a.Id AS NVARCHAR(36))      AS ActivityId,
        a.Type                          AS ActivityType,
        a.TookPlaceAtContractId         AS ContractId
    FROM       Activities.ActivityEscalationQueue  q
    JOIN       Activities.EscalationNote           n   ON n.ActivityEscalationQueueEntryId = q.Id
    JOIN       Activities.Activity                 a   ON a.Id = q.ActivityId
    OUTER APPLY (
        SELECT TOP 1 pqa2.LastModifiedBy, pqa2.LastModified
        FROM   Activities.ActivityPqaQueue pqa2
        WHERE  pqa2.ActivityId   = q.ActivityId
          AND  pqa2.LastModified < q.Created
        ORDER BY pqa2.LastModified DESC
    ) pqa
    WHERE q.IsCompleted = 1
      AND q.IsAccepted  = 1
      AND (
            n.IsExternal = 1
            OR NOT EXISTS (
                SELECT 1 FROM Activities.EscalationNote n2
                WHERE  n2.ActivityEscalationQueueEntryId = q.Id AND n2.IsExternal = 1
            )
          )
      AND n.FeedbackType IN (0, 1)
),
RankedActivityAdvisory AS
(
    SELECT *, ROW_NUMBER() OVER (PARTITION BY QueueEntryId ORDER BY IsExternal DESC, LastModified DESC) AS rn
    FROM ActivityAdvisorySource
)
INSERT INTO Mi.ProviderFeedbackActivity
    (Id, CreatedOn, SourceTable, Queue, QueueEntryId, NoteId,
     ActivityId, ActivityType, TenantId, ContractId, ParticipantId,
     SupportWorkerId, ProviderQaUserId, CfoUserId,
     PqaSubmittedDate, ActionDate, Message, FeedbackType)
SELECT
    NEWID(),
    GETUTCDATE(),
    r.SourceTable,
    r.Queue,
    r.QueueEntryId,
    r.NoteId,
    r.ActivityId,
    r.ActivityType,
    r.TenantId,
    r.ContractId,
    r.ParticipantId,
    r.SupportWorkerId,
    r.ProviderQaUserId,
    r.CfoUserId,
    r.PqaSubmittedDate,
    r.ActionDate,
    r.Message,
    r.FeedbackType
FROM RankedActivityAdvisory r
WHERE r.rn = 1
  AND NOT EXISTS (
        SELECT 1 FROM Mi.ProviderFeedbackActivity pfa
        WHERE  pfa.QueueEntryId = r.QueueEntryId
          AND  pfa.NoteId       = r.NoteId
          AND  pfa.SourceTable  = r.SourceTable
      );



-- -------------------------------------------------------
-- 8.  Insert missing ProviderFeedbackActivity rows  (Returned)
-- -------------------------------------------------------
;WITH ActivityReturnedSource AS
(
    SELECT
        q.Id                        AS QueueEntryId,
        n.Id                        AS NoteId,
        'Activities.Qa2QueueNote'   AS SourceTable,
        'QA2'                       AS Queue,
        q.TenantId,
        q.ParticipantId,
        q.SupportWorkerId,
        ISNULL(pqa.LastModifiedBy, '') AS ProviderQaUserId,
        ISNULL(n.CreatedBy,        '') AS CfoUserId,
        pqa.LastModified AS PqaSubmittedDate,
        n.Created AS ActionDate,
        n.Message,
        n.FeedbackType,
        n.IsExternal,
        q.LastModified,
        CAST(a.Id AS NVARCHAR(36))  AS ActivityId,
        a.Type                      AS ActivityType,
        a.TookPlaceAtContractId     AS ContractId
    FROM       Activities.ActivityQa2Queue   q
    JOIN       Activities.Qa2QueueNote       n   ON n.ActivityQa2QueueEntryId = q.Id
    JOIN       Activities.Activity           a   ON a.Id  = q.ActivityId
    OUTER APPLY (
        SELECT TOP 1 pqa2.LastModifiedBy, pqa2.LastModified
        FROM   Activities.ActivityPqaQueue pqa2
        WHERE  pqa2.ActivityId   = q.ActivityId
          AND  pqa2.LastModified < q.Created
        ORDER BY pqa2.LastModified DESC
    ) pqa
    WHERE q.IsCompleted = 1
      AND q.IsAccepted  = 0
      AND q.IsEscalated = 0
      AND (
            n.IsExternal = 1
            OR NOT EXISTS (
                SELECT 1 FROM Activities.Qa2QueueNote n2
                WHERE  n2.ActivityQa2QueueEntryId = q.Id AND n2.IsExternal = 1
            )
          )

    UNION ALL

    SELECT
        q.Id                            AS QueueEntryId,
        n.Id                            AS NoteId,
        'Activities.EscalationNote'     AS SourceTable,
        'Escalation'                    AS Queue,
        q.TenantId,
        q.ParticipantId,
        q.SupportWorkerId,
        ISNULL(pqa.LastModifiedBy, '') AS ProviderQaUserId,
        ISNULL(n.CreatedBy,        '') AS CfoUserId,
        pqa.LastModified AS PqaSubmittedDate,
        n.Created AS ActionDate,
        n.Message,
        n.FeedbackType,
        n.IsExternal,
        q.LastModified,
        CAST(a.Id AS NVARCHAR(36))      AS ActivityId,
        a.Type                          AS ActivityType,
        a.TookPlaceAtContractId         AS ContractId
    FROM       Activities.ActivityEscalationQueue  q
    JOIN       Activities.EscalationNote           n   ON n.ActivityEscalationQueueEntryId = q.Id
    JOIN       Activities.Activity                 a   ON a.Id = q.ActivityId
    OUTER APPLY (
        SELECT TOP 1 pqa2.LastModifiedBy, pqa2.LastModified
        FROM   Activities.ActivityPqaQueue pqa2
        WHERE  pqa2.ActivityId   = q.ActivityId
          AND  pqa2.LastModified < q.Created
        ORDER BY pqa2.LastModified DESC
    ) pqa
    WHERE q.IsCompleted = 1
      AND q.IsAccepted  = 0
      AND (
            n.IsExternal = 1
            OR NOT EXISTS (
                SELECT 1 FROM Activities.EscalationNote n2
                WHERE  n2.ActivityEscalationQueueEntryId = q.Id AND n2.IsExternal = 1
            )
          )
),
RankedActivityReturned AS
(
    -- Partition by QueueEntryId (not ActivityId) so every QA cycle gets its own
    -- MI row. An activity can be returned, resubmitted, and returned again —
    -- each cycle is a separate queue entry and a separate feedback decision.
    SELECT *, ROW_NUMBER() OVER (PARTITION BY QueueEntryId ORDER BY IsExternal DESC, LastModified DESC) AS rn
    FROM ActivityReturnedSource
)
INSERT INTO Mi.ProviderFeedbackActivity
    (Id, CreatedOn, SourceTable, Queue, QueueEntryId, NoteId,
     ActivityId, ActivityType, TenantId, ContractId, ParticipantId,
     SupportWorkerId, ProviderQaUserId, CfoUserId,
     PqaSubmittedDate, ActionDate, Message, FeedbackType)
SELECT
    NEWID(),
    GETUTCDATE(),
    r.SourceTable,
    r.Queue,
    r.QueueEntryId,
    r.NoteId,
    r.ActivityId,
    r.ActivityType,
    r.TenantId,
    r.ContractId,
    r.ParticipantId,
    r.SupportWorkerId,
    r.ProviderQaUserId,
    r.CfoUserId,
    r.PqaSubmittedDate,
    r.ActionDate,
    r.Message,
    r.FeedbackType
FROM RankedActivityReturned r
WHERE r.rn = 1
  AND NOT EXISTS (
        SELECT 1 FROM Mi.ProviderFeedbackActivity pfa
        WHERE  pfa.QueueEntryId = r.QueueEntryId
          AND  pfa.NoteId       = r.NoteId
          AND  pfa.SourceTable  = r.SourceTable
      );



-- -------------------------------------------------------
-- 9.  Insert missing ProviderFeedbackEnrolment rows  (Advisory)
-- -------------------------------------------------------
;WITH EnrolmentAdvisorySource AS
(
    SELECT
        q.Id                            AS QueueEntryId,
        n.Id                            AS NoteId,
        'Enrolment.Qa2QueueNote'        AS SourceTable,
        'QA2'                           AS Queue,
        q.TenantId,
        q.ParticipantId,
        q.SupportWorkerId,
        ISNULL(h.CreatedBy,     '') AS ProviderQaUserId,
        ISNULL(n.CreatedBy,     '') AS CfoUserId,
        h.Created AS PqaSubmittedDate,
        n.Created AS ActionDate,
        n.Message,
        n.FeedbackType,
        n.IsExternal,
        q.LastModified
    FROM       Enrolment.Qa2Queue        q
    JOIN       Enrolment.Qa2QueueNote    n   ON n.EnrolmentQa2QueueEntryId = q.Id
    OUTER APPLY (
        SELECT TOP 1 h2.CreatedBy, h2.Created
        FROM   Participant.EnrolmentHistory h2
        WHERE  h2.ParticipantId    = q.ParticipantId
          AND  h2.EnrolmentStatus  = 2
          AND  h2.Created          < q.Created
        ORDER BY h2.Created DESC
    ) h
    WHERE q.IsCompleted = 1
      AND q.IsAccepted  = 1
      AND (
            n.IsExternal = 1
            OR NOT EXISTS (
                SELECT 1 FROM Enrolment.Qa2QueueNote n2
                WHERE  n2.EnrolmentQa2QueueEntryId = q.Id AND n2.IsExternal = 1
            )
          )
      AND n.FeedbackType IN (0, 1)
      AND h.CreatedBy IS NOT NULL

    UNION ALL

    SELECT
        q.Id                            AS QueueEntryId,
        n.Id                            AS NoteId,
        'Enrolment.EscalationNote'      AS SourceTable,
        'Escalation'                    AS Queue,
        q.TenantId,
        q.ParticipantId,
        q.SupportWorkerId,
        ISNULL(h.CreatedBy,     '') AS ProviderQaUserId,
        ISNULL(n.CreatedBy,     '') AS CfoUserId,
        h.Created AS PqaSubmittedDate,
        n.Created AS ActionDate,
        n.Message,
        n.FeedbackType,
        n.IsExternal,
        q.LastModified
    FROM       Enrolment.EscalationQueue    q
    JOIN       Enrolment.EscalationNote     n   ON n.EnrolmentEscalationQueueEntryId = q.Id
    OUTER APPLY (
        SELECT TOP 1 h2.CreatedBy, h2.Created
        FROM   Participant.EnrolmentHistory h2
        WHERE  h2.ParticipantId    = q.ParticipantId
          AND  h2.EnrolmentStatus  = 2
          AND  h2.Created          < q.Created
        ORDER BY h2.Created DESC
    ) h
    WHERE q.IsCompleted = 1
      AND q.IsAccepted  = 1
      AND (
            n.IsExternal = 1
            OR NOT EXISTS (
                SELECT 1 FROM Enrolment.EscalationNote n2
                WHERE  n2.EnrolmentEscalationQueueEntryId = q.Id AND n2.IsExternal = 1
            )
          )
      AND n.FeedbackType IN (0, 1)
      AND h.CreatedBy IS NOT NULL
),
RankedEnrolmentAdvisory AS
(
    SELECT *, ROW_NUMBER() OVER (PARTITION BY QueueEntryId ORDER BY IsExternal DESC, LastModified DESC) AS rn
    FROM EnrolmentAdvisorySource
)
INSERT INTO Mi.ProviderFeedbackEnrolment
    (Id, CreatedOn, SourceTable, Queue, QueueEntryId, NoteId,
     TenantId, ContractId, ParticipantId,
     SupportWorkerId, ProviderQaUserId, CfoUserId,
     PqaSubmittedDate, ActionDate, Message, FeedbackType)
SELECT
    NEWID(),
    GETUTCDATE(),
    r.SourceTable,
    r.Queue,
    r.QueueEntryId,
    r.NoteId,
    r.TenantId,
    l.ContractId,
    r.ParticipantId,
    r.SupportWorkerId,
    r.ProviderQaUserId,
    r.CfoUserId,
    r.PqaSubmittedDate,
    r.ActionDate,
    r.Message,
    r.FeedbackType
FROM RankedEnrolmentAdvisory r
JOIN Participant.Participant  p ON p.Id = r.ParticipantId
JOIN Configuration.Location  l ON l.Id = p.EnrolmentLocationId
WHERE r.rn = 1
  AND l.ContractId IS NOT NULL
  AND NOT EXISTS (
        SELECT 1 FROM Mi.ProviderFeedbackEnrolment pfe
        WHERE  pfe.QueueEntryId = r.QueueEntryId
          AND  pfe.NoteId       = r.NoteId
          AND  pfe.SourceTable  = r.SourceTable
      );



-- -------------------------------------------------------
-- 10. Insert missing ProviderFeedbackEnrolment rows  (Returned)
-- -------------------------------------------------------
;WITH EnrolmentReturnedSource AS
(
    SELECT
        q.Id                            AS QueueEntryId,
        n.Id                            AS NoteId,
        'Enrolment.Qa2QueueNote'        AS SourceTable,
        'QA2'                           AS Queue,
        q.TenantId,
        q.ParticipantId,
        q.SupportWorkerId,
        ISNULL(h.CreatedBy,     '') AS ProviderQaUserId,
        ISNULL(n.CreatedBy,     '') AS CfoUserId,
        h.Created AS PqaSubmittedDate,
        n.Created AS ActionDate,
        n.Message,
        n.FeedbackType,
        n.IsExternal,
        q.LastModified
    FROM       Enrolment.Qa2Queue        q
    JOIN       Enrolment.Qa2QueueNote    n   ON n.EnrolmentQa2QueueEntryId = q.Id
    OUTER APPLY (
        SELECT TOP 1 h2.CreatedBy, h2.Created
        FROM   Participant.EnrolmentHistory h2
        WHERE  h2.ParticipantId    = q.ParticipantId
          AND  h2.EnrolmentStatus  = 2
          AND  h2.Created          < q.Created
        ORDER BY h2.Created DESC
    ) h
    WHERE q.IsCompleted = 1
      AND q.IsAccepted  = 0
      AND (
            n.IsExternal = 1
            OR NOT EXISTS (
                SELECT 1 FROM Enrolment.Qa2QueueNote n2
                WHERE  n2.EnrolmentQa2QueueEntryId = q.Id AND n2.IsExternal = 1
            )
          )
      AND h.CreatedBy IS NOT NULL

    UNION ALL

    SELECT
        q.Id                            AS QueueEntryId,
        n.Id                            AS NoteId,
        'Enrolment.EscalationNote'      AS SourceTable,
        'Escalation'                    AS Queue,
        q.TenantId,
        q.ParticipantId,
        q.SupportWorkerId,
        ISNULL(h.CreatedBy,     '') AS ProviderQaUserId,
        ISNULL(n.CreatedBy,     '') AS CfoUserId,
        h.Created AS PqaSubmittedDate,
        n.Created AS ActionDate,
        n.Message,
        n.FeedbackType,
        n.IsExternal,
        q.LastModified
    FROM       Enrolment.EscalationQueue    q
    JOIN       Enrolment.EscalationNote     n   ON n.EnrolmentEscalationQueueEntryId = q.Id
    OUTER APPLY (
        SELECT TOP 1 h2.CreatedBy, h2.Created
        FROM   Participant.EnrolmentHistory h2
        WHERE  h2.ParticipantId    = q.ParticipantId
          AND  h2.EnrolmentStatus  = 2
          AND  h2.Created          < q.Created
        ORDER BY h2.Created DESC
    ) h
    WHERE q.IsCompleted = 1
      AND q.IsAccepted  = 0
      AND (
            n.IsExternal = 1
            OR NOT EXISTS (
                SELECT 1 FROM Enrolment.EscalationNote n2
                WHERE  n2.EnrolmentEscalationQueueEntryId = q.Id AND n2.IsExternal = 1
            )
          )
      AND h.CreatedBy IS NOT NULL
),
RankedEnrolmentReturned AS
(
    SELECT *, ROW_NUMBER() OVER (PARTITION BY QueueEntryId ORDER BY IsExternal DESC, LastModified DESC) AS rn
    FROM EnrolmentReturnedSource
)
INSERT INTO Mi.ProviderFeedbackEnrolment
    (Id, CreatedOn, SourceTable, Queue, QueueEntryId, NoteId,
     TenantId, ContractId, ParticipantId,
     SupportWorkerId, ProviderQaUserId, CfoUserId,
     PqaSubmittedDate, ActionDate, Message, FeedbackType)
SELECT
    NEWID(),
    GETUTCDATE(),
    r.SourceTable,
    r.Queue,
    r.QueueEntryId,
    r.NoteId,
    r.TenantId,
    l.ContractId,
    r.ParticipantId,
    r.SupportWorkerId,
    r.ProviderQaUserId,
    r.CfoUserId,
    r.PqaSubmittedDate,
    r.ActionDate,
    r.Message,
    r.FeedbackType
FROM RankedEnrolmentReturned r
JOIN Participant.Participant  p ON p.Id = r.ParticipantId
JOIN Configuration.Location  l ON l.Id = p.EnrolmentLocationId
WHERE r.rn = 1
  AND l.ContractId IS NOT NULL
  AND NOT EXISTS (
        SELECT 1 FROM Mi.ProviderFeedbackEnrolment pfe
        WHERE  pfe.QueueEntryId = r.QueueEntryId
          AND  pfe.NoteId       = r.NoteId
          AND  pfe.SourceTable  = r.SourceTable
      );

-- -------------------------------------------------------
-- 11. Correct Provider QA User and Provider QA User submitted date etc. 
--     for Enrolment and Activity feedback where incorrect (copied from migration script)
-- -------------------------------------------------------
UPDATE pfe
SET 
    ProviderQaUserId = q.ProviderQaUserId,
    CfoUserId = q.CfoUserId,
    PqaSubmittedDate = q.PqaSubmittedDate,
    ActionDate = q.ActionDate
FROM 
    [Mi].ProviderFeedbackEnrolment pfe
    INNER JOIN 
    (
        SELECT 
            q.ParticipantId,                
            e2.CreatedBy as ProviderQaUserId,
            qn.CreatedBy as CfoUserId,
            e2.Created as PqaSubmittedDate,
            qn.Created as ActionDate,
            q.LastModified as QueueLastModified,
            qn.EnrolmentQa2QueueEntryId as Qa2QueueEntryId,
            qn.Id as NoteId
        FROM [Enrolment].[Qa2Queue] q 
        INNER JOIN [Enrolment].[Qa2QueueNote] qn on qn.EnrolmentQa2QueueEntryId = q.Id
        OUTER APPLY (
            SELECT TOP(1) [e1].[Created], [e1].[CreatedBy]
            FROM [Participant].[EnrolmentHistory] AS [e1]
            WHERE [e1].[ParticipantId] = [q].[ParticipantId] 
            AND CAST([e1].[EnrolmentStatus] AS int) = 2 
            AND [e1].[Created] < [qn].[Created]
            ORDER BY [e1].[Created] DESC
        ) AS [e2]
        WHERE qn.IsExternal = 1
    ) q ON 
        pfe.QueueEntryId = q.Qa2QueueEntryId 
        AND pfe.NoteId = q.NoteId 
        AND pfe.SourceTable = 'Enrolment.Qa2QueueNote'
WHERE 
    pfe.ProviderQaUserId <> q.ProviderQaUserId 
    OR pfe.CfoUserId <> q.CfoUserId 
    OR pfe.PqaSubmittedDate <> q.PqaSubmittedDate
    OR pfe.ActionDate <> q.ActionDate 


UPDATE pfe
SET 
    ProviderQaUserId = q.ProviderQaUserId,
    CfoUserId = q.CfoUserId,
    PqaSubmittedDate = q.PqaSubmittedDate,
    ActionDate = q.ActionDate
FROM [Mi].ProviderFeedbackEnrolment pfe
    INNER JOIN 
    (
        SELECT
            eq.ParticipantId,                 
            e2.CreatedBy as ProviderQaUserId,
            en.CreatedBy as CfoUserId,
            e2.Created as PqaSubmittedDate,
            en.Created as ActionDate,
            eq.LastModified as QueueLastModified,
            eq.Id as EscalationQueueEntryId,
            en.Id as NoteId
        FROM [Enrolment].[EscalationQueue] eq 
        INNER JOIN [Enrolment].[EscalationNote] en on en.EnrolmentEscalationQueueEntryId = eq.Id
        OUTER APPLY (
            SELECT TOP(1) [e1].[Created], [e1].[CreatedBy]
            FROM [Participant].[EnrolmentHistory] AS [e1]
            WHERE [e1].[ParticipantId] = [eq].[ParticipantId] 
            AND CAST([e1].[EnrolmentStatus] AS int) = 2 
            AND [e1].[Created] < [en].[Created]
            ORDER BY [e1].[Created] DESC
        ) AS [e2]
        WHERE en.IsExternal = 1 
    ) q ON 
        pfe.QueueEntryId = q.EscalationQueueEntryId 
        AND pfe.NoteId = q.NoteId 
        AND pfe.SourceTable = 'Enrolment.EscalationNote'
WHERE 
    pfe.ProviderQaUserId <> q.ProviderQaUserId 
    OR pfe.CfoUserId <> q.CfoUserId 
    OR pfe.PqaSubmittedDate <> q.PqaSubmittedDate
    OR pfe.ActionDate <> q.ActionDate
            
UPDATE pfa
SET 
    ProviderQaUserId = q.ProviderQaUserId,
    CfoUserId = q.CfoUserId,
    PqaSubmittedDate = q.PqaSubmittedDate,
    ActionDate = q.ActionDate
FROM 
    [Mi].ProviderFeedbackActivity pfa
    INNER JOIN
    (
        SELECT
            aq.ActivityId,                 
            a3.LastModifiedBy as ProviderQaUserId,
            qn.CreatedBy as CfoUserId,
            a3.LastModified as PqaSubmittedDate,
            qn.Created as ActionDate,
            qn.[ActivityQa2QueueEntryId] as Qa2QueueEntryId,
            qn.Id as NoteId,
            aq.LastModified as QueueLastModified
        FROM [Activities].[ActivityQA2Queue] aq 
        INNER JOIN [Activities].[QA2QueueNote] qn on qn.[ActivityQA2QueueEntryId] = aq.Id
        OUTER APPLY (
            SELECT TOP(1) [a2].[LastModified], [a2].[LastModifiedBy]
            FROM [Activities].[ActivityPqaQueue] AS [a2]
            WHERE [a2].[ActivityId] = [aq].[ActivityId] 
            AND [a2].[LastModified] < [qn].[Created]
            ORDER BY [a2].[LastModified] DESC
        ) AS [a3]
        WHERE qn.IsExternal = 1
    ) q on 
        pfa.QueueEntryId = q.Qa2QueueEntryId 
        AND pfa.NoteId = q.NoteId 
        AND pfa.SourceTable = 'Activities.Qa2QueueNote'
WHERE 
    pfa.ProviderQaUserId <> q.ProviderQaUserId 
    OR pfa.CfoUserId <> q.CfoUserId 
    OR pfa.PqaSubmittedDate <> q.PqaSubmittedDate
    OR pfa.ActionDate <> q.ActionDate


UPDATE pfa
SET 
    ProviderQaUserId = q.ProviderQaUserId,
    CfoUserId = q.CfoUserId,
    PqaSubmittedDate = q.PqaSubmittedDate,
    ActionDate = q.ActionDate
FROM
    [Mi].ProviderFeedbackActivity pfa
    INNER JOIN 
    (
        SELECT      
            aeq.ActivityId,           
            a3.LastModifiedBy as ProviderQaUserId,
            en.CreatedBy as CfoUserId,
            a3.LastModified as PqaSubmittedDate,
            en.Created as ActionDate,
            en.ActivityEscalationQueueEntryId as EscalationQueueEntryId,
            en.Id as NoteId,
            aeq.LastModified as QueueLastModified
        FROM [Activities].[ActivityEscalationQueue] aeq 
        INNER JOIN [Activities].[EscalationNote] en on en.[ActivityEscalationQueueEntryId] = aeq.Id
        OUTER APPLY (
            SELECT TOP(1) [a2].[LastModified], [a2].[LastModifiedBy]
            FROM [Activities].[ActivityPqaQueue] AS [a2]
            WHERE [a2].[ActivityId] = [aeq].[ActivityId] 
            AND [a2].[LastModified] < [en].[Created]
            ORDER BY [a2].[LastModified] DESC
        ) AS [a3]
        WHERE en.IsExternal = 1
    ) q ON 
        pfa.QueueEntryId = q.EscalationQueueEntryId 
        AND pfa.NoteId = q.NoteId 
        AND pfa.SourceTable = 'Activities.EscalationNote'
WHERE 
    pfa.ProviderQaUserId <> q.ProviderQaUserId 
    OR pfa.CfoUserId <> q.CfoUserId 
    OR pfa.PqaSubmittedDate <> q.PqaSubmittedDate
    OR pfa.ActionDate <> q.ActionDate

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW; -- re-raises the original error with full details
END CATCH;
