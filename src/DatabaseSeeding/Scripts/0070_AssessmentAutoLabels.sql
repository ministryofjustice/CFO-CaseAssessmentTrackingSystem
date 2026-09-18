
INSERT INTO [Configuration].[Label] ([Id], [Name], [Description], [Colour], [Variant], [Scope], [AppIcon])
SELECT [Id], [Name], [Description], [Colour], [Variant], [Scope], [AppIcon]
FROM (
    VALUES
             ('c4d54b01-c4a2-4368-a806-63307b514c18', 'Disabled', 'Participant considered themselves disabled', 2, 1, 1, 30),
             ('50176c48-e1f5-48b5-b422-182ef7b54b73', 'Homeless', 'Participant has indicated they are homeless', 2, 1, 1, 31),
             ('499a6380-5f0c-40d5-83c6-0bb1158ccc21', 'Neurodiverse', 'Participant has indicated they are neurodiverse', 2, 1, 1, 32)
) AS NewLabels ([Id], [Name], [Description], [Colour], [Variant], [Scope], [AppIcon])
WHERE NOT EXISTS (
    SELECT 1
    FROM [Configuration].[Label] AS ExistingLabels
    WHERE ExistingLabels.[Id] = NewLabels.[Id]
);
