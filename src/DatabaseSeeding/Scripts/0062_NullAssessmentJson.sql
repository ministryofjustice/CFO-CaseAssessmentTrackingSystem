

IF EXISTS (SELECT TOP(1) [Id] FROM [Participant].[Assessment] WHERE [AssessmentJson] IS NOT NULL)
BEGIN
  -- This is in a separate file to ensure it doesn't run if the previous step fails due to timeout.
  UPDATE [Participant].[Assessment]
    SET [AssessmentJson] = NULL;
END