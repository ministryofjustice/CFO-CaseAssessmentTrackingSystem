
IF NOT EXISTS (SELECT TOP(1) Id FROM [Configuration].[Label])
BEGIN

    INSERT INTO [Configuration].[Label] ([Id], [Name], [Description], [Colour], [Variant], [ContractId], [Scope], [AppIcon])
    VALUES
        ('1f611df9-1fb2-4059-b1ff-dcb4db6e761b', 'ISC Referral', 'Intensive Supervision Courts (ISCs) divert offenders with complex needs from short prison terms to community sentences, tackling root causes like substance misuse and supporting women.', 1, 0,  null, 0, 16),
        ('a154163b-63c8-43b1-887f-9853848c58f6', 'Veteran', 'Participant is a veteran',  2, 1, null, 1, 11)
 
 
END