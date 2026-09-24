
INSERT INTO [Configuration].[Label] ([Id], [Name], [Description], [Colour], [Variant], [Scope], [AppIcon])
SELECT [Id], [Name], [Description], [Colour], [Variant], [Scope], [AppIcon]
FROM (
    VALUES
             ('185546c8-000d-4860-9180-5e44e60e7f7b', 'ISC Birmingham (female)', 'Intensive supervision court referral from Birmingham (female)', 1, 0, 0, 16),
             ('f557e67c-9772-48f4-8a3a-dc8c341fe397', 'ISC Merseyside (substance misuse)', 'Intensive supervision court referral from Merseyside (substance misuse)', 1, 0, 0, 16),
             ('ca00c195-9b5c-4172-b3ab-9739f39af9e1', 'ISC Merseyside (female)', 'Intensive supervision court referral from Merseyside (female)', 1, 0, 0, 16),
             ('5d0f3bba-4c3d-4f1f-89d1-88fcdd0b8442', 'ISC Teesside (substance misuse)', 'Intensive supervision court referral from Teesside (substance misuse)', 1, 0, 0, 16),
             ('84213841-2206-4735-852d-64d702424c62', 'ISC Bristol (substance misuse)', 'Intensive supervision court referral from Bristol (substance misuse)', 1, 0, 0, 16),
             ('580d3b41-be95-415f-b6e7-1fa498b164b8', 'ISC Leeds (female)', 'Intensive supervision court referral from Leeds (female)', 1, 0, 0, 16),
             ('252f69b9-d83f-4e30-a058-b0b8ce81fab9', 'ISC Northumbria (female)', 'Intensive supervision court referral from Northumbria (female)', 1, 0, 0, 16),
             ('7f38783c-21cf-4fe2-9924-d7ba60f15287', 'ISC London Thames (female)', 'Intensive supervision court referral from London Thames (female)', 1, 0, 0, 16),
             ('1b5ca19d-0411-451d-8f2b-49b82beed9c0', 'ISC Inner London (prolific offender)', 'Intensive supervision court referral from Inner London (prolific offender)', 1, 0, 0, 16),
             ('db064f90-3e59-4426-8ea0-bfdcd5044c67', 'ISC Cambridge (substance misuse)', 'Intensive supervision court referral from Cambridge (substance misuse)', 1, 0, 0, 16)
) AS NewLabels ([Id], [Name], [Description], [Colour], [Variant], [Scope], [AppIcon])
WHERE NOT EXISTS (
    SELECT 1
    FROM [Configuration].[Label] AS ExistingLabels
    WHERE ExistingLabels.[Id] = NewLabels.[Id]
);
