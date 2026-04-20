

IF NOT EXISTS (SELECT TOP(1) [Id] FROM [Participant].[Note])
BEGIN

    SET IDENTITY_INSERT [Participant].[Note] ON;

    INSERT INTO [Participant].[Note] ([Id], [Message], [CallReference], [Created], [CreatedBy], [LastModified], [LastModifiedBy], [ParticipantId], [TenantId]) 
    VALUES 
    (1, N'amend the salary as it is a minus', NULL, '20250401 08:32:40.0571500', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG4936J', N'1.1.3.1.'),
    (2, N'agree', NULL, '20250401 08:39:18.9638067', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG4936J', N'1.1.3.1.'),
    (3, N'new consent form required', NULL, '20250401 09:05:55.2460517', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG2457U', N'1.1.3.1.'),
    (4, N'signature is a line', NULL, '20250414 12:42:30.9196703', N'b0275b64-7357-436c-88d2-3b67ea3a361b', NULL, NULL, N'1CFG1553F', N'1.1.4.1.'),
    (5, N'still a line', NULL, '20250414 13:12:53.8116421', N'b0275b64-7357-436c-88d2-3b67ea3a361b', NULL, NULL, N'1CFG1553F', N'1.1.4.1.'),
    (6, N'Testing new consent date', NULL, '20250416 13:43:02.4294244', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG8432I', N'1.1.3.1.'),
    (7, N'test', NULL, '20250423 13:18:47.2663414', N'b224e476-233c-4488-a11a-6b7a4e11eb78', NULL, NULL, N'1CFG5437L', N'1.1.2.3.'),
    (8, N'add RTW ', NULL, '20250423 14:20:35.7206570', N'b224e476-233c-4488-a11a-6b7a4e11eb78', NULL, NULL, N'1CFG5437L', N'1.1.2.3.'),
    (9, N'no risk ', NULL, '20250507 15:09:33.8068290', N'8cb4b1a7-c338-46e1-ad41-ef151384ec3e', NULL, NULL, N'1CFG5131C', N'1.1.4.2.'),
    (10, N'TESTING TESTING 123', NULL, '20250507 15:30:02.7402922', N'907a005d-2b19-413b-bdd2-f48e3b52ab6e', NULL, NULL, N'1CFG6390M', N'1.1.3.1.'),
    (11, N'tetsing', NULL, '20250516 11:53:21.9486821', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG2457U', N'1.1.3.1.'),
    (12, N'testing', NULL, '20250519 09:59:16.2567014', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG2457U', N'1.1.3.1.'),
    (13, N'testing note function time stamp', NULL, '20250609 13:11:47.6550893', N'907a005d-2b19-413b-bdd2-f48e3b52ab6e', NULL, NULL, N'1CFG8432I', N'1.1.3.1.'),
    (14, N'testing testing ', NULL, '20250609 13:12:42.9454756', N'9a699330-f706-401b-9fc1-fafd63b69c5c', NULL, NULL, N'1CFG9475S', N'1.1.'),
    (15, N'over 3 months', NULL, '20250609 14:15:35.6348033', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG7276C', N'1.1.3.1.'),
    (16, N'wet signature required', NULL, '20250627 13:24:53.8067533', N'8cb4b1a7-c338-46e1-ad41-ef151384ec3e', NULL, NULL, N'1CFG8236D', N'1.1.4.2.'),
    (17, N'helllo', NULL, '20250718 07:10:23.3857599', N'907a005d-2b19-413b-bdd2-f48e3b52ab6e', NULL, NULL, N'1CFG9960M', N'1.1.3.1.'),
    (18, N'testing testing ', NULL, '20250731 15:45:25.2488712', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG9960M', N'1.1.3.1.'),
    (19, N'testing to see if comments appear', NULL, '20250731 15:48:20.0009922', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG4261Y', N'1.1.3.1.'),
    (20, N'Training Demo', NULL, '20250811 12:53:36.1761542', N'98f8ac5b-8151-4342-896d-add3a146d44a', NULL, NULL, N'1CFG9798U', N'1.1.6.1.1.'),
    (21, N'Demo number 2', NULL, '20250811 12:53:49.7932514', N'98f8ac5b-8151-4342-896d-add3a146d44a', NULL, NULL, N'1CFG9798U', N'1.1.6.1.1.'),
    (22, N'hello', NULL, '20250916 08:22:16.1339889', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG9960M', N'1.1.3.1.'),
    (23, N'testing 12', NULL, '20250916 10:09:53.0126215', N'06098b9b-2a53-4bca-81fa-81e8e1befc47', NULL, NULL, N'1CFG1626P', N'1.1.3.'),
    (24, N're-submit with document', NULL, '20250917 09:09:54.0464739', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG1626P', N'1.1.3.1.'),
    (25, N'replace document', NULL, '20250917 09:15:24.5400896', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG8432I', N'1.1.3.1.'),
    (26, N'Demo', NULL, '20250917 09:33:56.4830660', N'4772fd06-ff8e-4745-8da2-fd97cd45b1f4', NULL, NULL, N'1CFG8236D', N'1.1.4.2.'),
    (27, N'Participant name and course info does not match template. ', NULL, '20250917 09:55:29.7730313', N'8cb4b1a7-c338-46e1-ad41-ef151384ec3e', NULL, NULL, N'1CFG8236D', N'1.1.4.2.'),
    (28, N'No RTW', NULL, '20250917 10:43:53.5569431', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG1109X', N'1.1.3.1.'),
    (29, N'A sample note that has been added for reasons.', NULL, '20250926 07:31:31.5001437', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', NULL, NULL, N'1CFG5469L', N'1.'),
    (30, N'no RAG justification', NULL, '20250930 08:24:02.2922272', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG8516T', N'1.1.3.1.'),
    (31, N'no signature', NULL, '20250930 08:51:34.6605011', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG8516T', N'1.1.3.1.'),
    (32, N'Adding note', NULL, '20250930 09:36:10.1187076', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', NULL, NULL, N'1CFG2622V', N'1.'),
    (33, N'testing abandonded', NULL, '20250930 13:05:48.5985369', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG4261Y', N'1.1.3.1.'),
    (34, N'testing to abandon', NULL, '20250930 13:08:44.8928299', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG4261Y', N'1.1.3.1.'),
    (35, N'no signature', NULL, '20250930 13:09:26.7819201', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG4261Y', N'1.1.3.1.'),
    (36, N'incorrect', NULL, '20250930 13:21:07.1892395', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG8432I', N'1.1.3.1.'),
    (37, N'Participant signature is missing', NULL, '20251002 08:19:31.0977709', N'8a9711cd-5d08-477c-aa94-b2e72cf1101b', NULL, NULL, N'1CFG8432I', N'1.1.3.1.');
    
    SET IDENTITY_INSERT [Participant].[Note] OFF;

END;