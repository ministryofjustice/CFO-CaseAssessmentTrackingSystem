
    IF NOT EXISTS (SELECT TOP (1) [UserId] FROM [Identity].[UserRole])
    BEGIN

        INSERT INTO [Identity].[UserRole] (UserId, RoleId) 
        VALUES 
            (N'0240afb1-78ee-497a-b42f-25f61cce5ecc', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'06098b9b-2a53-4bca-81fa-81e8e1befc47', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'08cdee0b-a6c2-4c5a-8ebe-2cff53789052', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'0df50dc3-8acf-49f6-9509-92793bf0fdfa', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'13a3fb8c-ee5a-47fa-acf7-d3430a6c66fc', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'2c4343a8-50c1-4338-b1f5-c552a68beb5b', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'2fe21b83-49ce-4448-9a99-918d26f8571f', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'31bfbd95-fac0-4413-9388-59e344d70c39', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'31c433ea-6392-4343-8d4d-c90e11ae9a96', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'484bb01b-ad4f-46f0-a2b0-2697b2dbaa51', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'579e92f6-80c4-4578-8902-fa71709c25c8', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'626e57ae-29c5-4c2b-ac4b-4e509ec2677d', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'651a59da-2d85-425a-80df-b85721135ed1', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'65afa19b-908b-4469-8019-2939c7604cb1', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'6634e4db-4dae-44c3-b789-4f7a523f3c94', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'669d758c-df88-4daf-936d-d2777cb2861d', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'6efb5b92-eba8-476e-8535-6cea7d4a8a1f', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'6f74f2b0-3725-4868-b4d3-31bebd2d3e43', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'85bbd0e6-1db3-41b0-affc-ecbf44efd29d', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'897ccf79-c30d-49ea-997b-723985aac12e', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'8cb4b1a7-c338-46e1-ad41-ef151384ec3e', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'9424573a-8b6d-425f-a952-b062c3c5f6be', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'969cd7a3-a9c4-4389-91df-1add45b17dbb', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'9fc33852-8b16-4836-b4f3-21bab6697e70', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'a88b4a23-f5a9-4627-b347-6c7568a716de', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'b0275b64-7357-436c-88d2-3b67ea3a361b', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'b224e476-233c-4488-a11a-6b7a4e11eb78', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'b8ead276-e405-4cfb-b4ac-2e49869524e4', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'bd7e12f9-eac9-4f49-abe5-2239cdc1fa1f', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'bff3e17e-9784-40ae-a8d0-cd085d065b23', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'cff25b43-7c5f-499a-a3f6-d8e32ae3b0de', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'f07e506f-fbd0-4417-8dd5-890c2b7b4edf', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
            (N'9a699330-f706-401b-9fc1-fafd63b69c5c', N'c1d2e3f4-5g6h-7i8j-9k0l-1m2n3o4p5q6r'),
            (N'77467465-c7b2-40f8-b145-219225ae1459', N'd3e4f5g6-7h8i-9j0k-1l2m-3n4o5p6q7r8s'),
            (N'1113c865-21f4-43c2-a03d-b7936da3b18b', N'e5f6g7h8-9i0j-1k2l-3m4n-5o6p7q8r9s0t'),
            (N'dc339552-334c-41e0-8d5e-a5a4e06398a9', N'f7g8h9i0-1j2k-3l4m-5n6o-7p8q9r0s1t2u'),
            (N'01961e09-01db-74f7-9e3b-ed68179f098b', N'be470a0e-e5b4-4530-a98a-d93835a7801c'),
            (N'2a9b3450-1feb-4be3-ab94-24e64cd34829', N'g7hf2g5f-5g9a-7gb1-d5f3-7f3e5f2f5g7g'),
            (N'9fe4e2f0-506a-4870-b3a2-a8e09fa2e73b', N'afd11113-9006-4f7e-bd24-f236b8bb90fb'),
            (N'1afd5d4c-4ca6-440a-ad0a-3d59f03719ef', N'db35e765-8c85-44c7-a569-ce1630b8890e');

        INSERT INTO [identity].[UserRole] ( UserId, roleId )
        SELECT ur.UserId, 'be470a0e-e5b4-4530-a98a-d93835a7801c' from [identity].[Role] as r
        INNER JOIN [identity].[UserRole] as ur on r.Id = ur.RoleId
        WHERE r.[Name] IN ('SMT', 'CFO QA Manager', 'CFO QA Support Manager', 'Contract Support Officer', 'Contract Performance Manager')
        EXCEPT
        SELECT UserId, RoleId FROM [identity].[UserRole];

    END