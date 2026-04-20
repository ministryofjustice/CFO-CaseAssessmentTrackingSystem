
IF NOT EXISTS (SELECT TOP (1) Id FROM [Identity].[Role])
BEGIN

    INSERT INTO [Identity].[Role] ([Id], [Description], [Name], [NormalizedName], [ConcurrencyStamp], [RoleRank]) 
    VALUES 
        (N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136', N'User performance QA and finance functionality for providers', N'QA + Finance', N'QA + FINANCE', N'e6340a9c-8144-4f9d-8489-2e21af06bc56', 9999),
        (N'c1d2e3f4-5g6h-7i8j-9k0l-1m2n3o4p5q6r', N'Responsible for overseeing financial operations and quality assurance', N'CFO QA Officer', N'CFO QA OFFICER', N'7e8f9g0h-1i2j-3k4l-5m6n-7o8p9q0r1s2t', 60),
        (N'd3e4f5g6-7h8i-9j0k-1l2m-3n4o5p6q7r8s', N'Support manager for financial operations and quality assurance', N'CFO QA Support Manager', N'CFO QA SUPPORT MANAGER', N'8f9g0h1i-2j3k-4l5m-6n7o-8p9q0r1s2t3u', 40),
        (N'e5f6g7h8-9i0j-1k2l-3m4n-5o6p7q8r9s0t', N'Manager for financial operations and quality assurance', N'CFO QA Manager', N'CFO QA MANAGER', N'9g0h1i2j-3k4l-5m6n-7o8p-9q0r1s2t3u4v', 20),
        (N'f7g8h9i0-1j2k-3l4m-5n6o-7p8q9r0s1t2u', N'Senior Management Team member', N'SMT', N'SMT', N'0h1i2j3k-4l5m-6n7o-8p9q-0r1s2t3u4v5w', 10),
        (N'g7hf2g5f-5g9a-7gb1-d5f3-7f3e5f2f5g7g', N'User responsible for system-level support and maintenance.', N'System Support', N'SYSTEM SUPPORT', N'4e5f6d7c-8b9a-7e9f-3a4e-5f6d7c8b9a3e', 0),
        (N'be470a0e-e5b4-4530-a98a-d93835a7801c', N'Finance User (can access payment screens)', N'Finance', N'FINANCE', N'77a1e519-b3c9-41f5-9680-af92ce34f541', 9999),
        (N'afd11113-9006-4f7e-bd24-f236b8bb90fb', N'Contract Support Officer', 'Contract Support Officer', 'Contract Support Officer', 'a445b5dd-d0b3-4d42-8537-3245ee1300e4', 60),
        (N'db35e765-8c85-44c7-a569-ce1630b8890e', N'Contract Performance Manager', 'Contract Performance Manager', 'Contract Performance Manager', '2726acf5-90e2-405b-bd04-8d7d9f5a5484', 60),
        (N'b1caa941-4185-4877-ad88-78786ac4be46', N'Contract Management Process Support Manager', 'Contract Management Process Support Manager', 'Contract Management Process Support Manager', '01b80c54-7719-496a-9c6d-3e6fddcec9fa', 60);

END