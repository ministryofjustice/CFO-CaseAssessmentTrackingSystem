-- Script to populate ReturnReason field based on Message field prefix
-- Description: Updates ReturnReason in queue note tables and MI tables

-- Step 1: Update [Enrolment].[Qa2QueueNote]
UPDATE [Enrolment].[Qa2QueueNote]
SET [ReturnReason] = CASE
    WHEN [Message] LIKE 'Ineligible Claim%' THEN 'Ineligible Claim'
    WHEN [Message] LIKE 'Incorrect / Missing Paperwork%' THEN 'Incorrect / Missing Paperwork'
    WHEN [Message] LIKE 'Information incomplete%' THEN 'Information incomplete'
    WHEN [Message] LIKE 'Information conflicts with CATS%' THEN 'Information conflicts with CATS'
    ELSE 'Other'
END
WHERE [ReturnReason] IS NULL
  AND [IsExternal] = 1
  AND [FeedbackType] = 2; -- FeedbackType.Returned = 2

-- Step 2: Update [Enrolment].[EscalationNote]
UPDATE [Enrolment].[EscalationNote]
SET [ReturnReason] = CASE
    WHEN [Message] LIKE 'Ineligible Claim%' THEN 'Ineligible Claim'
    WHEN [Message] LIKE 'Incorrect / Missing Paperwork%' THEN 'Incorrect / Missing Paperwork'
    WHEN [Message] LIKE 'Information incomplete%' THEN 'Information incomplete'
    WHEN [Message] LIKE 'Information conflicts with CATS%' THEN 'Information conflicts with CATS'
    ELSE 'Other'
END
WHERE [ReturnReason] IS NULL
  AND [IsExternal] = 1
  AND [FeedbackType] = 2; -- FeedbackType.Returned = 2

-- Step 3: Update [Activities].[Qa2QueueNote]
UPDATE [Activities].[Qa2QueueNote]
SET [ReturnReason] = CASE
    WHEN [Message] LIKE 'Ineligible Claim%' THEN 'Ineligible Claim'
    WHEN [Message] LIKE 'Incorrect / Missing Paperwork%' THEN 'Incorrect / Missing Paperwork'
    WHEN [Message] LIKE 'Information incomplete%' THEN 'Information incomplete'
    WHEN [Message] LIKE 'Information conflicts with CATS%' THEN 'Information conflicts with CATS'
    ELSE 'Other'
END
WHERE [ReturnReason] IS NULL
  AND [IsExternal] = 1
  AND [FeedbackType] = 2; -- FeedbackType.Returned = 2

-- Step 4: Update [Activities].[EscalationNote]
UPDATE [Activities].[EscalationNote]
SET [ReturnReason] = CASE
    WHEN [Message] LIKE 'Ineligible Claim%' THEN 'Ineligible Claim'
    WHEN [Message] LIKE 'Incorrect / Missing Paperwork%' THEN 'Incorrect / Missing Paperwork'
    WHEN [Message] LIKE 'Information incomplete%' THEN 'Information incomplete'
    WHEN [Message] LIKE 'Information conflicts with CATS%' THEN 'Information conflicts with CATS'
    ELSE 'Other'
END
WHERE [ReturnReason] IS NULL
  AND [IsExternal] = 1
  AND [FeedbackType] = 2; -- FeedbackType.Returned = 2

PRINT 'Updated ' + CAST(@@ROWCOUNT AS VARCHAR) + ' rows in [Activities].[EscalationNote]';

-- Step 5: Update [Mi].[ProviderFeedbackEnrolment] 
Update [Mi].[ProviderFeedbackEnrolment]
SET [ReturnReason] = CASE
    WHEN [Message] LIKE 'Ineligible Claim%' THEN 'Ineligible Claim'
    WHEN [Message] LIKE 'Incorrect / Missing Paperwork%' THEN 'Incorrect / Missing Paperwork'
    WHEN [Message] LIKE 'Information incomplete%' THEN 'Information incomplete'
    WHEN [Message] LIKE 'Information conflicts with CATS%' THEN 'Information conflicts with CATS'
    ELSE 'Other'
END
WHERE [ReturnReason] IS NULL
  AND [SourceTable] IN ('Enrolment.Qa2QueueNote', 'Enrolment.EscalationNote')
  AND FeedbackType = 2; -- FeedbackType.Returned = 2

-- Step 6: Update [Mi].[ProviderFeedbackActivity] 
Update [Mi].[ProviderFeedbackActivity]
SET [ReturnReason] = CASE
    WHEN [Message] LIKE 'Ineligible Claim%' THEN 'Ineligible Claim'
    WHEN [Message] LIKE 'Incorrect / Missing Paperwork%' THEN 'Incorrect / Missing Paperwork'
    WHEN [Message] LIKE 'Information incomplete%' THEN 'Information incomplete'
    WHEN [Message] LIKE 'Information conflicts with CATS%' THEN 'Information conflicts with CATS'
    ELSE 'Other'
END
WHERE [ReturnReason] IS NULL
  AND [SourceTable] IN ('Activities.Qa2QueueNote', 'Activities.EscalationNote')
  AND FeedbackType = 2; -- FeedbackType.Returned = 2

