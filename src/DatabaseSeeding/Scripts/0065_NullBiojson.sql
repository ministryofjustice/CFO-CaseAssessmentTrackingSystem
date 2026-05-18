IF EXISTS (SELECT TOP(1) [Id] FROM [Participant].[Bio] WHERE [BioJson] IS NOT NULL)
BEGIN
  -- This is in a separate file to ensure it doesn't run if the previous step fails due to timeout.
  UPDATE [Participant].[Bio]
    SET [BioJson] = NULL;
END