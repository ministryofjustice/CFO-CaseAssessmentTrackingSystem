IF NOT EXISTS (SELECT TOP(1) [Id] FROM [Configuration].[AnnouncementType])
BEGIN
    INSERT INTO [Configuration].[AnnouncementType] ([Id], [Name], [Created])
    SELECT NEWID(), defaults.[Name], SYSUTCDATETIME()
    FROM (VALUES ('General'), ('Release Note')) defaults([Name]);
END;
