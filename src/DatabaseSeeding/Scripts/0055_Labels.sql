
INSERT INTO [Configuration].[Label] ([Id], [Name], [Description], [Colour], [Variant], [ContractId], [Scope], [AppIcon])
SELECT [Id], [Name], [Description], [Colour], [Variant], [ContractId], [Scope], [AppIcon]
FROM (
    VALUES
        ('1f611df9-1fb2-4059-b1ff-dcb4db6e761b', 'ISC Referral', 'Intensive Supervision Courts (ISCs) divert offenders with complex needs from short prison terms to community sentences, tackling root causes like substance misuse and supporting women.', 1, 0,  null, 0, 16),
        ('a154163b-63c8-43b1-887f-9853848c58f6', 'Veteran', 'Participant is a veteran',  2, 1, null, 1, 11),
        ('a8e17308-74f6-4f7c-9a1a-c50b5cc85b73', 'Care Leaver', 'Spent time in care as a child',  2, 1, null, 1, 27),
        ('f182441a-905f-4846-ad5e-f2acffe86e9a', 'Gang Member', 'Participant has indicated they have current or previous gang membership',  2, 1, null, 1, 28),
        ('c4962b22-8897-4771-b80f-334eb98b6920', 'Transgender', 'A participant whose gender identity doers not correspond with the sex registered for them at birth',  2, 1, null, 1, 29)
) AS NewLabels ([Id], [Name], [Description], [Colour], [Variant], [ContractId], [Scope], [AppIcon])
WHERE NOT EXISTS (
    SELECT 1
    FROM [Configuration].[Label] AS ExistingLabels
    WHERE ExistingLabels.[Id] = NewLabels.[Id]
);