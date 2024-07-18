
SET NOCOUNT ON;

BEGIN TRANSACTION;

BEGIN TRY

    INSERT INTO [Configuration].Tenant (Id, Name, Description, Created)
    VALUES
        (N'1.', N'CFO', N'Root tenant for Creating Future Opportunities', N'2024-05-31 12:30:38.3849332'),
        (N'1.1.', N'CFO Evolution', N'Top level tenant for Evolution Programme', N'2024-05-31 12:30:38.3850497'),
        
        -- Achieve
        (N'1.1.1.', N'Achieve', N'Top level tenant for provider Achieve', N'2024-05-31 12:30:38.3850635'),
        (N'1.1.1.1.', N'North West Contract', N'Achieve (North West Contract)', N'2024-05-31 12:30:38.3850635'),
        (N'1.1.1.1.1.', N'Achieve_NW', N'Achieve (North West Team)', N'2024-05-31 12:30:38.3850635'),
        
        -- Igneus
        (N'1.1.2.', N'Ingeus', N'Top level tenant for provider Ingeus', N'2024-05-31 12:30:38.3850637'),
        
        (N'1.1.2.1.', N'North East Contract', N'Ingeus (North East Contract)', N'2024-05-31 12:30:38.3850637'),
        (N'1.1.2.1.1.', N'Igneus_NE', N'Igneus (North East)', N'2024-05-31 12:30:38.3850637'),
        (N'1.1.2.1.2.', N'AWayOut_NE', N'A way out (North East)', N'2024-05-31 12:30:38.3850637'),
        (N'1.1.2.1.3.', N'ForcesEmploymentCharity_NE', N'Forces Employment Charity (North East)', N'2024-05-31 12:30:38.3850637'),
        
        (N'1.1.2.2.', N'West Midlands Contract', N'Ingeus (West Midlands Contract)', N'2024-05-31 12:30:38.3850637'),
        (N'1.1.2.2.1.', N'Igneus_WM', N'Igneus (West Midlands)', N'2024-05-31 12:30:38.3850637'),
        (N'1.1.2.2.2.', N'AWayOut_WM', N'A way out (West Midlands)', N'2024-05-31 12:30:38.3850637'),
        (N'1.1.2.2.3.', N'ChangingLives_WM', N'Changing Lives (West Midlands)', N'2024-05-31 12:30:38.3850637'),
        
        (N'1.1.2.3.', N'East Midlands Contract', N'Ingeus (East Midlands Contract)', N'2024-05-31 12:30:38.3850637'),
        (N'1.1.2.3.1.', N'Igneus_EM', N'Igneus (East Midlands)', N'2024-05-31 12:30:38.3850637'),
        (N'1.1.2.3.2.', N'ChangingLives_EM', N'Changing Lives (East Midlands)', N'2024-05-31 12:30:38.3850637'),
        (N'1.1.2.3.3.', N'LAT_EM', N'LAT (East Midlands)', N'2024-05-31 12:30:38.3850637'),
        
        -- The Growth Co
        (N'1.1.3.', N'The Growth Co', N'Top level tenant for provider The Growth Co', N'2024-05-31 12:30:38.3850645'),
        (N'1.1.3.1.', N'Yorkshire and Humberside Contract', N'The Growth Co (Yorkshire and Humberside Contract)', N'2024-05-31 12:30:38.3850645'),
        (N'1.1.3.1.1.', N'Growth_YH', N'Growth (Yorkshire and Humberside)', N'2024-05-31 12:30:38.3850645'),
        (N'1.1.3.1.2.', N'CommunityLinks_YH', N'Community Links (Yorkshire and Humberside)', N'2024-05-31 12:30:38.3850645'),
        (N'1.1.3.1.3.', N'StGiles_YH', N'St Giles (Yorkshire and Humberside)', N'2024-05-31 12:30:38.3850645'),
        
        -- Shaw Trust
        (N'1.1.4.', N'Shaw Trust', N'Top level tenant for Shaw Trust', N'2024-05-31 12:30:38.3850647'),
        (N'1.1.4.1.', N'East of England Contract', N'Shaw Trust (East of England Contract)', N'2024-05-31 12:30:38.3850647'),
        (N'1.1.4.1.1.', N'ShawTrust_EE', N'Shaw Trust (East of England)', N'2024-05-31 12:30:38.3850647'),
        (N'1.1.4.2.', N'South East Contract', N'Shaw Trust (South East Contract)', N'2024-05-31 12:30:38.3850647'),
        (N'1.1.4.2.1.', N'ShawTrust_SE', N'Shaw Trust (South East)', N'2024-05-31 12:30:38.3850647'),
        
        -- Reed
        (N'1.1.5.', N'Reed', N'Reed', N'2024-05-31 12:30:38.3850651'),
        (N'1.1.5.1.', N'London Contract', N'Reed (London Contract)', N'2024-05-31 12:30:38.3850651'),
        (N'1.1.5.1.1.', N'Reed_LND', N'Reed (London)', N'2024-05-31 12:30:38.3850651'),
        (N'1.1.5.1.2.', N'ShawTrust_LND', N'Reed (Shaw Trust)', N'2024-05-31 12:30:38.3850651'),
        
        -- Seetec
        (N'1.1.6.', N'Seetec', N'Seetec', N'2024-05-31 12:30:38.3850653'),
        (N'1.1.6.1.', N'South West Contract', N'Seetec (South West Contract)', N'2024-05-31 12:30:38.3850653'),
        (N'1.1.6.1.1.', N'Seetec_SW', N'Seetec (South West)', N'2024-05-31 12:30:38.3850653')

    -- temporary values
    INSERT INTO [Configuration].[TenantDomain] (Domain, TenantId, Created, CreatedBy, LastModified, LastModifiedBy)
    VALUES
     ('@justice.gov.uk', '1.', '2024-07-18 10:32:16.4866667', NULL, NULL, NULL),
     ('@justice.gov.uk', '1.1.', '2024-07-18 10:32:16.4866667', NULL, NULL, NULL),
     ('@achieve.org', '1.1.1.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@northwestcontract.org', '1.1.1.1.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@achieve_nw.org', '1.1.1.1.1.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@ingeus.org', '1.1.2.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@northeastcontract.org', '1.1.2.1.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@igneus_ne.org', '1.1.2.1.1.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@awayout_ne.org', '1.1.2.1.2.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@forcesemploymentcharity_ne.org', '1.1.2.1.3.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@westmidlandscontract.org', '1.1.2.2.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@igneus_wm.org', '1.1.2.2.1.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@awayout_wm.org', '1.1.2.2.2.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@changinglives_wm.org', '1.1.2.2.3.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@eastmidlandscontract.org', '1.1.2.3.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@igneus_em.org', '1.1.2.3.1.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@changinglives_em.org', '1.1.2.3.2.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@lat_em.org', '1.1.2.3.3.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@thegrowthco.org', '1.1.3.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@yorkshireandhumbersidecontract.org', '1.1.3.1.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@growth_yh.org', '1.1.3.1.1.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@communitylinks_yh.org', '1.1.3.1.2.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@stgiles_yh.org', '1.1.3.1.3.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@shawtrust.org', '1.1.4.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@eastofenglandcontract.org', '1.1.4.1.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@shawtrust_ee.org', '1.1.4.1.1.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@southeastcontract.org', '1.1.4.2.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@shawtrust_se.org', '1.1.4.2.1.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@reed.org', '1.1.5.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@londoncontract.org', '1.1.5.1.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@reed_lnd.org', '1.1.5.1.1.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@shawtrust_lnd.org', '1.1.5.1.2.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@seetec.org', '1.1.6.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@southwestcontract.org', '1.1.6.1.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL),
     ('@seetec_sw.org', '1.1.6.1.1.', '2024-07-18 10:34:46.0466667', NULL, NULL, NULL);



    INSERT INTO [Identity].[Role] (Id, Description, Name, NormalizedName, ConcurrencyStamp, RoleRank) VALUES
    ('g7hf2g5f-5g9a-7gb1-d5f3-7f3e5f2f5g7g', N'User responsible for system-level support and maintenance.', N'System Support', N'SYSTEM SUPPORT', N'4e5f6d7c-8b9a-7e9f-3a4e-5f6d7c8b9a3e', 0),
    ('f7g8h9i0-1j2k-3l4m-5n6o-7p8q9r0s1t2u', N'Senior Management Team member', N'SMT', N'SMT', N'0h1i2j3k-4l5m-6n7o-8p9q-0r1s2t3u4v5w', 10),
    ('e5f6g7h8-9i0j-1k2l-3m4n-5o6p7q8r9s0t', N'Manager for financial operations and quality assurance', N'CFO QA Manager', N'CFO QA MANAGER', N'9g0h1i2j-3k4l-5m6n-7o8p-9q0r1s2t3u4v', 20),
    ('d3e4f5g6-7h8i-9j0k-1l2m-3n4o5p6q7r8s', N'Support manager for financial operations and quality assurance', N'CFO QA Support Manager', N'CFO QA SUPPORT MANAGER', N'8f9g0h1i-2j3k-4l5m-6n7o-8p9q0r1s2t3u', 40),
    ('c1d2e3f4-5g6h-7i8j-9k0l-1m2n3o4p5q6r', N'Responsible for overseeing financial operations and quality assurance', N'CFO QA Officer', N'CFO QA OFFICER', N'7e8f9g0h-1i2j-3k4l-5m6n-7o8p9q0r1s2t', 60),
    ('a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136', N'User performance QA and finance functionality for providers', N'QA + Finance', N'QA + FINANCE', N'e6340a9c-8144-4f9d-8489-2e21af06bc56', 9999);

    INSERT INTO [Identity].[User]
    (Id, DisplayName, ProviderId, TenantId, TenantName, ProfilePictureDataUrl, IsActive, IsLive, MemorablePlace, MemorableDate, RefreshToken, RequiresPasswordReset, RefreshTokenExpiryTime, SuperiorId, Created, CreatedBy, LastModified, LastModifiedBy, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount)
    VALUES
        (N'1113c865-21f4-43c2-a03d-b7936da3b18b', N'CFO QA Manager', N'1.1.', N'1.1.', N'CFO Evolution', null, 1, 0, N'dsfsdf', N'sddsdfsdfs', null, 0, N'0001-01-01 00:00:00.0000000', null, N'2024-07-18 09:47:04.8211348', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', N'2024-07-18 09:55:30.1500776', null, N'qa.manager@justice.gov.uk', N'QA.MANAGER@JUSTICE.GOV.UK', N'qa.manager@justice.gov.uk', N'QA.MANAGER@JUSTICE.GOV.UK', 1, N'AQAAAAIAAYagAAAAEA9UJWA46Xs8GwIvHSLSVo/flgsDq633xN0qy4UXPuPnTuM/MFLTl0c0jEwV7GDaLw==', N'4FIG7VHOXML4KSETLIGMRIMRIUEURCT7', N'90dfdac5-85ef-449a-8396-5bd72ce88f90', null, 0, 0, null, 1, 0),
        (N'2a9b3450-1feb-4be3-ab94-24e64cd34829', N'System Support', N'1.', N'1.', N'CFO', null, 1, 0, N'sdfjkhsdkjfh', N'sjkdhfsdkjhsfdkjh', null, 0, N'0001-01-01 00:00:00.0000000', null, N'2024-07-18 09:44:06.3118132', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', N'2024-07-18 09:53:15.4328414', null, N'system.support@justice.gov.uk', N'SYSTEM.SUPPORT@JUSTICE.GOV.UK', N'system.support@justice.gov.uk', N'SYSTEM.SUPPORT@JUSTICE.GOV.UK', 1, N'AQAAAAIAAYagAAAAEHSya2/M04HCehtcjHtdXSfyE+bbJBO+RTHHI5/AkofucmxMpe2I1knaWno0/w5wPg==', N'VISGGOKJ4OF6KIN4MHWNHVVL22TD3Q4V', N'2d902b3a-222b-4c6c-b83c-c09a2cad8605', null, 0, 0, null, 1, 0),
        (N'6f74f2b0-3725-4868-b4d3-31bebd2d3e43', N'Achieve QA', N'1.1.1.', N'1.1.1.', N'Achieve', null, 1, 0, N'fdsgdfgfd', N'dfdffdgfdg', null, 0, N'0001-01-01 00:00:00.0000000', null, N'2024-07-18 09:52:22.0162857', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', N'2024-07-18 10:05:43.5018249', null, N'achieve.qa@Achieve.org', N'ACHIEVE.QA@ACHIEVE.ORG', N'achieve.qa@Achieve.org', N'ACHIEVE.QA@ACHIEVE.ORG', 1, N'AQAAAAIAAYagAAAAECvz7CJJOlFHFo7R2oflBL+h2D+NCnyoX4TH7cFlbMwjlYPhfS6pOoMTMrhtDrP/pw==', N'D4LIJ734BYFTEDPOVJJUCAXI65RKYO6D', N'74f6d131-5b41-41eb-ac31-601def3c427b', N'', 0, 0, null, 1, 0),
        (N'77467465-c7b2-40f8-b145-219225ae1459', N'QA Support Manager', N'1.1.', N'1.1.', N'CFO Evolution', null, 1, 0, N'65406540', N'65406540', null, 0, N'0001-01-01 00:00:00.0000000', null, N'2024-07-18 09:46:03.4857600', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', N'2024-07-18 10:06:41.0753982', null, N'qa.supportmanager@justice.gov.uk', N'QA.SUPPORTMANAGER@JUSTICE.GOV.UK', N'qa.supportmanager@justice.gov.uk', N'QA.SUPPORTMANAGER@JUSTICE.GOV.UK', 1, N'AQAAAAIAAYagAAAAEK8RRdqG4KmDoMz57eQ62Qs1M+hIC7sufLULaFTCklUyl9fxlmOWmmhp18+DxkiDag==', N'3OFQEID2KLXHROJSUNXC6XCAH4XPWGQ4', N'6b327831-c7b9-42ac-a083-b51b78904915', null, 0, 0, null, 1, 0),
        (N'9a699330-f706-401b-9fc1-fafd63b69c5c', N'QA Officer', N'1.1.', N'1.1.', N'CFO Evolution', null, 1, 0, N'654654', N'654654654', null, 0, N'0001-01-01 00:00:00.0000000', null, N'2024-07-18 09:45:14.6662875', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', N'2024-07-18 09:54:05.3132649', null, N'qa.officer@justice.gov.uk', N'QA.OFFICER@JUSTICE.GOV.UK', N'qa.officer@justice.gov.uk', N'QA.OFFICER@JUSTICE.GOV.UK', 1, N'AQAAAAIAAYagAAAAEGVgb8aZ6I3Orlww2ybi6FZ7EUSOG06LDs/BcdYoIl5noOibPhDswN77m0rcNblX3w==', N'4DKBJMGUYOUVKHQ7JOMVAJF3O7X4ROLA', N'd5789eab-c7ee-4bb1-9b1f-ceced53f5f86', null, 0, 0, null, 1, 0),
        (N'bd41a35a-f0bf-4fc8-bd39-096a845b600a', N'achive user', N'1.1.1.', N'1.1.1.', N'Achieve', null, 1, 0, N'sdfsdf', N'sdfsdf', null, 0, N'0001-01-01 00:00:00.0000000', null, N'2024-07-18 09:51:32.0432900', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', N'2024-07-18 10:05:20.0851128', null, N'achieve.user@Achieve.org', N'ACHIEVE.USER@ACHIEVE.ORG', N'achieve.user@Achieve.org', N'ACHIEVE.USER@ACHIEVE.ORG', 1, N'AQAAAAIAAYagAAAAEKPQb/8I3liGtSSbE3yME0YjjPOQ1vGfiFlC8nhFzZ7YIcFYGlS0Ju1Y1nkQ+cV/Mw==', N'QIBS7SDSLXKZF45FQGB75XEVQ272QE6X', N'f24f37cc-d020-41b2-9c5c-11823008ebd3', null, 0, 0, null, 1, 0),
        (N'dc339552-334c-41e0-8d5e-a5a4e06398a9', N'SMT', N'1.1.', N'1.1.', N'CFO Evolution', null, 1, 0, N'sdsdf', N'sdfsddsf', null, 0, N'0001-01-01 00:00:00.0000000', null, N'2024-07-18 09:49:00.4535673', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', N'2024-07-18 10:04:18.4860271', null, N'smt@justice.gov.uk', N'SMT@JUSTICE.GOV.UK', N'smt@justice.gov.uk', N'SMT@JUSTICE.GOV.UK', 1, N'AQAAAAIAAYagAAAAELxakrYyp0xNiT0rPi86vASc6GpynC6UHtexDR4g2hR4nObiRcAKB/ZvgcyO3+pidg==', N'VEOLHI5E7JIVNKCNH2J43MSEQIAL6ANZ', N'3b36bf04-352d-4858-b3d2-52785bec5c34', null, 0, 0, null, 1, 0)

    INSERT INTO [Identity].UserRole (UserId, RoleId)
    VALUES
        (N'6f74f2b0-3725-4868-b4d3-31bebd2d3e43', N'a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136'),
        (N'9a699330-f706-401b-9fc1-fafd63b69c5c', N'c1d2e3f4-5g6h-7i8j-9k0l-1m2n3o4p5q6r'),
        (N'77467465-c7b2-40f8-b145-219225ae1459', N'd3e4f5g6-7h8i-9j0k-1l2m-3n4o5p6q7r8s'),
        (N'1113c865-21f4-43c2-a03d-b7936da3b18b', N'e5f6g7h8-9i0j-1k2l-3m4n-5o6p7q8r9s0t'),
        (N'dc339552-334c-41e0-8d5e-a5a4e06398a9', N'f7g8h9i0-1j2k-3l4m-5n6o-7p8q9r0s1t2u'),
        (N'2a9b3450-1feb-4be3-ab94-24e64cd34829', N'g7hf2g5f-5g9a-7gb1-d5f3-7f3e5f2f5g7g')

    INSERT INTO [Identity].Note (Message, CallReference, Created, CreatedBy, LastModified, LastModifiedBy, UserId) VALUES (N'System Support user created via seeding', N'001', N'2024-07-18 09:44:06.3121990', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null, N'2a9b3450-1feb-4be3-ab94-24e64cd34829');
    INSERT INTO [Identity].Note (Message, CallReference, Created, CreatedBy, LastModified, LastModifiedBy, UserId) VALUES (N'QA Officer user created via seeding', N'001', N'2024-07-18 09:45:14.6663701', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null, N'9a699330-f706-401b-9fc1-fafd63b69c5c');
    INSERT INTO [Identity].Note (Message, CallReference, Created, CreatedBy, LastModified, LastModifiedBy, UserId) VALUES (N'QA Support Manager user created via seeding ', N'001', N'2024-07-18 09:46:03.4857794', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null, N'77467465-c7b2-40f8-b145-219225ae1459');
    INSERT INTO [Identity].Note (Message, CallReference, Created, CreatedBy, LastModified, LastModifiedBy, UserId) VALUES (N'CFO QA Manager user created via seeding', N'001', N'2024-07-18 09:47:04.8211639', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null, N'1113c865-21f4-43c2-a03d-b7936da3b18b');
    INSERT INTO [Identity].Note (Message, CallReference, Created, CreatedBy, LastModified, LastModifiedBy, UserId) VALUES (N'SMT user created via seeding', N'001', N'2024-07-18 09:49:00.4535861', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null, N'dc339552-334c-41e0-8d5e-a5a4e06398a9');
    INSERT INTO [Identity].Note (Message, CallReference, Created, CreatedBy, LastModified, LastModifiedBy, UserId) VALUES (N'Achive user created via seeding', N'001', N'2024-07-18 09:51:32.0433115', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null, N'bd41a35a-f0bf-4fc8-bd39-096a845b600a');
    INSERT INTO [Identity].Note (Message, CallReference, Created, CreatedBy, LastModified, LastModifiedBy, UserId) VALUES (N'Achieve QA user created via seeding', N'001', N'2024-07-18 09:52:22.0163087', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null, N'6f74f2b0-3725-4868-b4d3-31bebd2d3e43');

    INSERT INTO [Configuration].[Contract] (Id, LotNumber, LifetimeStart, LifetimeEnd, Description, TenantId, Created, CreatedBy, LastModified, LastModifiedBy)
    VALUES
        (N'con_24036', 1, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'North West', N'1.1.1.1.', N'2024-05-31 12:30:38.9702330', null, null, null),
        (N'con_24037', 2, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'North East', N'1.1.2.1.', N'2024-05-31 12:30:38.9702326', null, null, null),
        (N'con_24038', 3, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'Yorkshire and Humberside', N'1.1.3.1.', N'2024-05-31 12:30:38.9702322', null, null, null),
        (N'con_24041', 4, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'West Midlands', N'1.1.2.2.', N'2024-05-31 12:30:38.9702315', null, null, null),
        (N'con_24042', 5, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'East Midlands', N'1.1.2.3.', N'2024-05-31 12:30:38.9702311', null, null, null),
        (N'con_24043', 6, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'East Of England', N'1.1.4.1.', N'2024-05-31 12:30:38.9702308', null, null, null),
        (N'con_24044', 7, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'London', N'1.1.5.1.', N'2024-05-31 12:30:38.9702304', null, null, null),
        (N'con_24045', 8, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'South West', N'1.1.6.1.', N'2024-05-31 12:30:38.9702300', null, null, null),
        (N'con_24046', 9, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'South East', N'1.1.4.2.', N'2024-05-31 12:30:38.9702153', null, null, null);



    SET IDENTITY_INSERT [Configuration].[KeyValue] ON;

    INSERT INTO [Configuration].[KeyValue] (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy)
    VALUES
        (1, N'ReferralSource', N'CFO Evolution Provider', N'CFO Evolution Provider', N'A referral source', N'2024-05-31 12:30:42.7615462', null, null, null),
        (2, N'ReferralSource', N'Probation', N'Probation', N'A referral source', N'2024-05-31 12:30:42.7615457', null, null, null),
        (3, N'ReferralSource', N'Approved Premises', N'Approved Premises', N'A referral source', N'2024-05-31 12:30:42.7615453', null, null, null),
        (4, N'ReferralSource', N'CAS2', N'CAS2', N'A referral source', N'2024-05-31 12:30:42.7615451', null, null, null),
        (5, N'ReferralSource', N'CAS3', N'CAS3', N'A referral source', N'2024-05-31 12:30:42.7615449', null, null, null),
        (6, N'ReferralSource', N'Custodial Family Services', N'Custodial Family Services', N'A referral source', N'2024-05-31 12:30:42.7615448', null, null, null),
        (7, N'ReferralSource', N'CRS - Women', N'CRS- Women', N'A referral source', N'2024-05-31 12:30:42.7615445', null, null, null),
        (8, N'ReferralSource', N'CRS - Personal Wellbeing', N'CRS- Personal Wellbeing', N'A referral source', N'2024-05-31 12:30:42.7615443', null, null, null),
        (9, N'ReferralSource', N'CRS - Dependency & Recovery', N'CRS- Dependency & Recovery', N'A referral source', N'2024-05-31 12:30:42.7615441', null, null, null),
        (10, N'ReferralSource', N'CRS - Accommodation', N'CRS- Accommodation', N'A referral source', N'2024-05-31 12:30:42.7615439', null, null, null),
        (11, N'ReferralSource', N'Custody staff', N'Custody staff', N'A referral source', N'2024-05-31 12:30:42.7615436', null, null, null),
        (12, N'ReferralSource', N'New Futures Network', N'New Futures Network', N'A referral source', N'2024-05-31 12:30:42.7615428', null, null, null),
        (13, N'ReferralSource', N'Prison Education Provider', N'Prison Education Provider', N'A referral source', N'2024-05-31 12:30:42.7615425', null, null, null),
        (14, N'ReferralSource', N'DWP', N'DWP', N'A referral source', N'2024-05-31 12:30:42.7615421', null, null, null),
        (15, N'ReferralSource', N'Healthcare', N'Healthcare', N'A referral source', N'2024-05-31 12:30:42.7615417', null, null, null),
        (16, N'ReferralSource', N'Community / Voluntary Sector organisation', N'Community / Voluntary Sector organisation', N'A referral source', N'2024-05-31 12:30:42.7615414', null, null, null),
        (17, N'ReferralSource', N'Local Authority', N'Local Authority', N'A referral source', N'2024-05-31 12:30:42.7615410', null, null, null),
        (18, N'ReferralSource', N'Courts', N'Courts', N'A referral source', N'2024-05-31 12:30:42.7615390', null, null, null),
        (19, N'ReferralSource', N'Self-referral', N'Self-referral', N'A referral source', N'2024-05-31 12:30:42.7615372', null, null, null),
        (20, N'ReferralSource', N'Other', N'Other', N'A referral source (please state)', N'2024-05-31 12:30:42.7615265', null, null, null)

    SET IDENTITY_INSERT [Configuration].[KeyValue] OFF;

    SET IDENTITY_INSERT [Configuration].[Location] ON;

    MERGE INTO [Configuration].[Location] AS Target
    USING (VALUES
               (1, N'Risley', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888014', null, null, null),
               (2, N'Lancaster', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888012', null, null, null),
               (3, N'Forest Bank', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888009', null, null, null),
               (4, N'Altcourse', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888007', null, null, null),
               (5, N'Preston', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888004', null, null, null),
               (6, N'Buckley Hall', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888001', null, null, null),
               (7, N'Liverpool', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5887998', null, null, null),
               (8, N'Manchester', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5887995', null, null, null),
               (9, N'Thorn Cross', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5887991', null, null, null),
               (10, N'Haverigg', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5887988', null, null, null),
               (11, N'Hindley', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5887984', null, null, null),
               (12, N'Kirkham', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5887894', null, null, null),
               (13, N'Wymott', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5886872', null, null, null),
               (14, N'Styal', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888017', null, null, null),
               (15, N'Holme House', N'con_24037', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888025', null, null, null),
               (16, N'Northumberland', N'con_24037', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888028', null, null, null),
               (17, N'Durham', N'con_24037', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888030', null, null, null),
               (18, N'Kirklevington Grange', N'con_24037', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888032', null, null, null),
               (19, N'Low Newton', N'con_24037', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888034', null, null, null),
               (20, N'Wealstun', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888037', null, null, null),
               (21, N'Moorland', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888043', null, null, null),
               (22, N'Humber', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888046', null, null, null),
               (23, N'Doncaster', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888051', null, null, null),
               (24, N'Leeds', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888053', null, null, null),
               (25, N'Hull', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888055', null, null, null),
               (26, N'Full Sutton', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888058', null, null, null),
               (27, N'Hatfield', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888060', null, null, null),
               (28, N'Lindholme', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888063', null, null, null),
               (29, N'Askham Grange', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888066', null, null, null),
               (30, N'New Hall', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888076', null, null, null),
               (31, N'Featherstone', N'con_24041', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888088', null, null, null),
               (32, N'Drake Hall', N'con_24041', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888091', null, null, null),
               (33, N'Birmingham', N'con_24041', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888093', null, null, null),
               (34, N'Brinsford', N'con_24041', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888096', null, null, null),
               (35, N'Dovegate', N'con_24041', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888101', null, null, null),
               (36, N'Hewell', N'con_24041', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888104', null, null, null),
               (37, N'Oakwood', N'con_24041', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888106', null, null, null),
               (38, N'Stoke Heath', N'con_24041', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888108', null, null, null),
               (39, N'Swinfen Hall', N'con_24041', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888111', null, null, null),
               (40, N'Ranby', N'con_24042', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888114', null, null, null),
               (41, N'Nottingham', N'con_24042', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888118', null, null, null),
               (42, N'Five Wells', N'con_24042', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888121', null, null, null),
               (43, N'Lincoln', N'con_24042', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888125', null, null, null),
               (44, N'North Sea Camp', N'con_24042', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888127', null, null, null),
               (45, N'Onley', N'con_24042', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888130', null, null, null),
               (46, N'Stocken', N'con_24042', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888132', null, null, null),
               (47, N'Whatton', N'con_24042', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888135', null, null, null),
               (48, N'Foston Hall', N'con_24042', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888138', null, null, null),
               (49, N'The Mount', N'con_24043', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888140', null, null, null),
               (50, N'Peterborough (M)', N'con_24043', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888143', null, null, null),
               (51, N'Bedford', N'con_24043', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888146', null, null, null),
               (52, N'Chelmsford', N'con_24043', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888149', null, null, null),
               (53, N'Highpoint', N'con_24043', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888151', null, null, null),
               (54, N'Hollesley Bay', N'con_24043', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888153', null, null, null),
               (55, N'Littlehey', N'con_24043', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888157', null, null, null),
               (56, N'Norwich', N'con_24043', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888159', null, null, null),
               (57, N'Wayland', N'con_24043', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888162', null, null, null),
               (58, N'Peterborough (F)', N'con_24043', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888165', null, null, null),
               (59, N'High Down', N'con_24044', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888167', null, null, null),
               (60, N'Wandsworth', N'con_24044', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888170', null, null, null),
               (61, N'Thameside', N'con_24044', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888171', null, null, null),
               (62, N'Brixton', N'con_24044', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888179', null, null, null),
               (63, N'Feltham', N'con_24044', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888181', null, null, null),
               (64, N'Isis', N'con_24044', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888184', null, null, null),
               (65, N'Pentonville', N'con_24044', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888186', null, null, null),
               (66, N'Bronzefield', N'con_24044', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888189', null, null, null),
               (67, N'Downview', N'con_24044', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888197', null, null, null),
               (68, N'Portland', N'con_24045', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888199', null, null, null),
               (69, N'Exeter', N'con_24045', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888201', null, null, null),
               (70, N'Bristol', N'con_24045', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888204', null, null, null),
               (71, N'Channings Wood', N'con_24045', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888206', null, null, null),
               (72, N'Dartmoor', N'con_24045', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888209', null, null, null),
               (73, N'Leyhill', N'con_24045', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888212', null, null, null),
               (74, N'Guys Marsh', N'con_24045', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888214', null, null, null),
               (75, N'The Verne', N'con_24045', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888217', null, null, null),
               (76, N'Eastwood Park', N'con_24045', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888219', null, null, null),
               (77, N'Rochester', N'con_24046', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888221', null, null, null),
               (78, N'Elmley', N'con_24046', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888224', null, null, null),
               (79, N'Lewes', N'con_24046', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888227', null, null, null),
               (80, N'Winchester', N'con_24046', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888229', null, null, null),
               (81, N'Aylesbury', N'con_24046', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888232', null, null, null),
               (82, N'Bullingdon', N'con_24046', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888234', null, null, null),
               (83, N'Ford', N'con_24046', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888237', null, null, null),
               (84, N'Springhill', N'con_24046', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888239', null, null, null),
               (85, N'Stanford Hill', N'con_24046', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888241', null, null, null),
               (86, N'Swaleside', N'con_24046', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888244', null, null, null),
               (87, N'Woodhill', N'con_24046', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888246', null, null, null),
               (88, N'Send', N'con_24046', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888253', null, null, null),
               (89, N'East Sutton Park', N'con_24046', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888312', null, null, null),
               (90, N'North West Community', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
               (91, N'North East Community', N'con_24037', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
               (92, N'Yorkshire and Humberside Community', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
               (93, N'West Midlands Community', N'con_24041', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
               (94, N'East Midlands Community', N'con_24042', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
               (95, N'East Of England Community', N'con_24043', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
               (96, N'London Community', N'con_24044', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
               (97, N'South West Community', N'con_24045', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
               (98, N'South East Community', N'con_24046', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
               (99, N'Manchester', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
               (100, N'Liverpool', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
               (101, N'Warrington', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 100, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
               (102, N'Blackpool', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
               (103, N'Preston', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 102, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
               (104, N'Blackburn', N'con_24036', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 102, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
               (105, N'Durham', N'con_24037', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
               (106, N'Middlesbrough', N'con_24037', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 105, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
               (107, N'Darlington', N'con_24037', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 105, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
               (108, N'Sunderland', N'con_24037', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
               (109, N'Newcastle', N'con_24037', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 108, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
               (110, N'Leeds', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
               (111, N'Bradford', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 110, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
               (112, N'Huddersfield', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 110, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
               (113, N'Doncaster', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
               (114, N'Sheffield', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 113, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
               (115, N'Hull', N'con_24038', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
               (116, N'Birmingham', N'con_24041', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
               (117, N'Wolverhampton', N'con_24041', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
               (118, N'Stoke', N'con_24041', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 117, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
               (119, N'Nottingham', N'con_24042', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
               (120, N'Leicester', N'con_24042', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 119, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
               (121, N'Derby', N'con_24042', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 119, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
               (122, N'Peterborough', N'con_24043', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
               (123, N'Luton', N'con_24043', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 122, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
               (124, N'Croydon', N'con_24044', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
               (125, N'Lambeth', N'con_24044', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 124, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
               (126, N'Lewisham', N'con_24044', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 124, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
               (127, N'Bristol', N'con_24045', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
               (128, N'Plymouth', N'con_24045', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 127, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
               (129, N'Medway', N'con_24046', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
               (130, N'Southampton', N'con_24046', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 129, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
               (131, N'Portsmouth', N'con_24046', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 129, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null)
    ) AS Source (Id, Name, ContractId, LifetimeStart, LifetimeEnd, ParentLocationId, GenderProvisionId, LocationTypeId, Created, CreatedBy, LastModified, LastModifiedBy)
    ON Target.Id = Source.Id
    WHEN MATCHED THEN
        UPDATE SET Target.Name = Source.Name, Target.ContractId = Source.ContractId, Target.LifetimeStart = Source.LifetimeStart, Target.LifetimeEnd = Source.LifetimeEnd, Target.ParentLocationId = Source.ParentLocationId, Target.GenderProvisionId = Source.GenderProvisionId, Target.LocationTypeId = Source.LocationTypeId, Target.Created = Source.Created, Target.CreatedBy = Source.CreatedBy, Target.LastModified = Source.LastModified, Target.LastModifiedBy = Source.LastModifiedBy
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Id, Name, ContractId, LifetimeStart, LifetimeEnd, ParentLocationId, GenderProvisionId, LocationTypeId, Created, CreatedBy, LastModified, LastModifiedBy)
        VALUES (Source.Id, Source.Name, Source.ContractId, Source.LifetimeStart, Source.LifetimeEnd, Source.ParentLocationId, Source.GenderProvisionId, Source.LocationTypeId, Source.Created, Source.CreatedBy, Source.LastModified, Source.LastModifiedBy);


    SET IDENTITY_INSERT [Configuration].[Location] OFF;


    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('ACI', 'Prison', 'Altcourse', 'North West', 4);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('AGI', 'Prison', 'Askham Grange', 'Yorkshire and Humberside', 29);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('ASI', 'Prison', 'Ashfield', 'South West', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('AYI', 'Prison', 'Aylesbury', 'South East', 81);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('BAI', 'Prison', 'Belmarsh', 'London', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('BFI', 'Prison', 'Bedford', 'East of England', 51);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('BMI', 'Prison', 'Birmingham', 'West Midlands', 33);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('BHI', 'Prison', 'Blantyre House', 'South East', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('BSI', 'Prison', 'Brinsford', 'West Midlands', 34);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('BLI', 'Prison', 'Bristol', 'South West', 70);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('BXI', 'Prison', 'Brixton', 'London', 62);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('BRI', 'Prison', 'Bure', 'East of England', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('BZI', 'Prison', 'Bronzefield', 'South East', 66);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('BWI', 'Prison', 'Berwyn', 'Unknown', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('BCI', 'Prison', 'Buckley Hall', 'North West', 6);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('BNI', 'Prison', 'Bullingdon', 'South East', 82);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('CWI', 'Prison', 'Channings Wood', 'South West', 71);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('CFI', 'Prison', 'Cardiff', 'Unknown', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('CKI', 'Prison', 'Cookham Wood', 'South East', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('CLI', 'Prison', 'Coldingley', 'South East', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('CDI', 'Prison', 'Chelmsford', 'East of England', 52);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('DAI', 'Prison', 'Dartmoor', 'South West', 72);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('DNI', 'Prison', 'Doncaster', 'Yorkshire and Humberside', 23);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('DGI', 'Prison', 'Dovegate', 'West Midlands', 35);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('DWI', 'Prison', 'Downview', 'South East', 67);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('DHI', 'Prison', 'Drake Hall', 'West Midlands', 32);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('DTI', 'Prison', 'Deerbolt', 'North East', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('DVI', 'Prison', 'Dover', 'South East', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('DMI', 'Prison', 'Durham', 'North East', 17);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('EEI', 'Prison', 'Erlestoke', 'South West', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('ESI', 'Prison', 'East Sutton Park', 'South East', 89);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('EWI', 'Prison', 'Eastwood Park', 'South West', 76);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('EYI', 'Prison', 'Elmley', 'South East', 78);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('EXI', 'Prison', 'Exeter', 'South West', 69);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('FSI', 'Prison', 'Featherstone', 'West Midlands', 31);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('FMI', 'Prison', 'Feltham', 'London', 63);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('FYI', 'Prison', 'Feltham', 'London', 63);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('FEI', 'Prison', 'Fosse Way', 'East Midlands', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('FWI', 'Prison', 'Five Wells', 'East Midlands', 42);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('FKI', 'Prison', 'Frankland', 'North East', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('FDI', 'Prison', 'Ford', 'South East', 83);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('FBI', 'Prison', 'Forest Bank', 'North West', 3);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('FHI', 'Prison', 'Foston Hall', 'East Midlands', 48);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('FNI', 'Prison', 'Full Sutton', 'Yorkshire and Humberside', 26);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('GMI', 'Prison', 'Guys Marsh', 'South West', 74);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('GHI', 'Prison', 'Garth', 'North West', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('HDI', 'Prison', 'Hatfield', 'Yorkshire and Humberside', 27);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('GNI', 'Prison', 'Grendon', 'South East', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('GPI', 'Prison', 'Glen Parva', 'East Midlands', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('GTI', 'Prison', 'Gartree', 'East Midlands', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('HVI', 'Prison', 'Haverigg', 'North West', 10);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('HCI', 'Prison', 'Huntercombe', 'South East', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('HEI', 'Prison', 'Hewell', 'West Midlands', 36);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('HOI', 'Prison', 'High Down', 'South East', 59);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('HPI', 'Prison', 'Highpoint', 'East of England', 53);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('HII', 'Prison', 'Hindley', 'North West', 11);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('HBI', 'Prison', 'Hollesley Bay', 'East of England', 54);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('HHI', 'Prison', 'Holme House', 'North East', 15);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('HLI', 'Prison', 'Hull', 'Yorkshire and Humberside', 25);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('HMI', 'Prison', 'Humber', 'Yorkshire and Humberside', 22);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('HRI', 'Prison', 'Haslar', 'South East', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('ISI', 'Prison', 'Isis', 'London', 64);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('HYI', 'Prison', 'Holloway', 'London', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('KMI', 'Prison', 'Kirkham', 'North West', 12);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('IWI', 'Prison', 'Isle Of Wight', 'South East', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('KVI', 'Prison', 'Kirklevington Grange', 'North East', 18);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('KTI', 'Prison', 'Kennet', 'North West', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('LFI', 'Prison', 'Lancaster Farms', 'North West', 2);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('LCI', 'Prison', 'Leicester', 'East Midlands', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('LEI', 'Prison', 'Leeds', 'Yorkshire and Humberside', 24);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('LWI', 'Prison', 'Lewes', 'South East', 79);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('LGI', 'Prison', 'Lowdham Grange', 'East Midlands', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('LYI', 'Prison', 'Leyhill', 'South West', 73);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('LII', 'Prison', 'Lincoln', 'East Midlands', 43);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('LLI', 'Prison', 'Long Lartin', 'West Midlands', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('LHI', 'Prison', 'Lindholme', 'Yorkshire and Humberside', 28);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('LTI', 'Prison', 'Littlehey', 'East of England', 55);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('LPI', 'Prison', 'Liverpool', 'North West', 7);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('LNI', 'Prison', 'Low Newton', 'North East', 19);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('MRI', 'Prison', 'Manchester', 'North West', 8);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('MDI', 'Prison', 'Moorland', 'Yorkshire and Humberside', 21);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('MHI', 'Prison', 'Morton Hall', 'East Midlands', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('NHI', 'Prison', 'New Hall', 'Yorkshire and Humberside', 30);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('MSI', 'Prison', 'Maidstone', 'South East', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('NSI', 'Prison', 'North Sea Camp', 'East Midlands', 44);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('MWI', 'Prison', 'Medway', 'South East', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('NLI', 'Prison', 'Northumberland', 'North East', 16);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('NWI', 'Prison', 'Norwich', 'East of England', 56);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('NMI', 'Prison', 'Nottingham', 'East Midlands', 41);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('OWI', 'Prison', 'Oakwood', 'West Midlands', 37);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('ONI', 'Prison', 'Onley', 'East Midlands', 45);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('PVI', 'Prison', 'Pentonville', 'London', 65);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('OUT', 'Prison', 'Outside Prison', 'National', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('PBI', 'Prison', 'Peterborough', 'East of England', 50);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('PFI', 'Prison', 'Peterborough', 'East of England', 58);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('PDI', 'Prison', 'Portland', 'South West', 68);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('PNI', 'Prison', 'Preston', 'North West', 5);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('RNI', 'Prison', 'Ranby', 'East Midlands', 40);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('PRI', 'Prison', 'Parc', 'Unknown', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('RSI', 'Prison', 'Risley', 'North West', 1);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('PYI', 'Prison', 'Parc', 'Unknown', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('RCI', 'Prison', 'Rochester', 'South East', 77);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('RHI', 'Prison', 'Rye Hill', 'East Midlands', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('SDI', 'Prison', 'Send', 'South East', 88);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('SPI', 'Prison', 'Spring Hill', 'South East', 84);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('EHI', 'Prison', 'Standford Hill', 'South East', 85);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('SFI', 'Prison', 'Stafford', 'West Midlands', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('SKI', 'Prison', 'Stocken', 'East Midlands', 46);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('SHI', 'Prison', 'Stoke Heath', 'West Midlands', 38);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('STI', 'Prison', 'Styal', 'North West', 14);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('SLI', 'Prison', 'Swaleside', 'South East', 86);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('SNI', 'Prison', 'Swinfen Hall', 'West Midlands', 39);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('TSI', 'Prison', 'Thameside', 'London', 61);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('SUI', 'Prison', 'Sudbury', 'East Midlands', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('SWI', 'Prison', 'Swansea', 'Unknown', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('MTI', 'Prison', 'The Mount', 'East of England', 49);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('TRN', 'Prison', 'In Transfer', 'National', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('VEI', 'Prison', 'The Verne', 'South West', 75);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('UKI', 'Prison', 'Usk', 'Unknown', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('UPI', 'Prison', 'Prescoed', 'Unknown', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('TCI', 'Prison', 'Thorn Cross', 'North West', 9);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('WWI', 'Prison', 'Wandsworth', 'London', 60);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('WDI', 'Prison', 'Wakefield', 'Yorkshire and Humberside', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('WLI', 'Prison', 'Wayland', 'East of England', 57);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('WEI', 'Prison', 'Wealstun', 'Yorkshire and Humberside', 20);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('WII', 'Prison', 'Warren Hill', 'East of England', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('WTI', 'Prison', 'Whatton', 'East Midlands', 47);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('WCI', 'Prison', 'Winchester', 'South East', 80);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('WNI', 'Prison', 'Werrington', 'West Midlands', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('WRI', 'Prison', 'Whitemoor', 'East of England', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('WSI', 'Prison', 'Wormwood Scrubs', 'London', null);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('WHI', 'Prison', 'Woodhill', 'South East', 87);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('WMI', 'Prison', 'Wymott', 'North West', 13);
    INSERT INTO dms.LocationMapping ([Code],  [CodeType], [Description], [DeliveryRegion], [LocationId]) VALUES ('WYI', 'Prison', 'Wetherby', 'Yorkshire and Humberside', null);


    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('ALL', 'Probation', 'No Trust or Trust Unknown', 'National', null);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('ASP', 'Probation', 'Avon & Somerset', 'South West Community', 97);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('BED', 'Probation', 'Bedfordshire', 'East Of England Community', 95);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C01', 'Probation', 'CPA Northumbria', 'North East Community', 91);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C02', 'Probation', 'CPA Cumbria and Lancashire', 'North West Community', 90);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C03', 'Probation', 'CPA Durham Tees Valley', 'North East Community', 91);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C04', 'Probation', 'CPA Humber Lincs & N Yorks', 'Yorkshire and Humberside Community', 92);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C05', 'Probation', 'CPA West Yorkshire', 'Yorkshire and Humberside Community', 92);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C06', 'Probation', 'CPA Merseyside', 'North West Community', 90);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C07', 'Probation', 'CPA Cheshire and Gtr Manchester', 'North West Community', 90);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C08', 'Probation', 'CPA Derby Leics Notts Rutland', 'East Midlands Community', 94);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C09', 'Probation', 'CPA South Yorkshire', 'Yorkshire and Humberside Community', 92);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C10', 'Probation', 'CPA Wales', 'Unknown', null);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C11', 'Probation', 'CPA Staff and West Mids', 'West Midlands Community', 93);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C12', 'Probation', 'CPA Warwickshire and West Mercia', 'West Midlands Community', 93);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C13', 'Probation', 'CPA BeNCH', 'National', null);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C14', 'Probation', 'CPA Norfolk and Suffolk', 'East Of England Community', 95);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C15', 'Probation', 'CPA Brist Gloucs Somerset Wilts', 'South West Community', 97);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C16', 'Probation', 'CPA Thames Valley', 'South East Community', 98);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C17', 'Probation', 'CPA London', 'London Community', 96);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C18', 'Probation', 'CPA Essex', 'East Of England Community', 95);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C19', 'Probation', 'CPA Dorset Devon and Cornwall', 'South West Community', 97);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C20', 'Probation', 'CPA Hampshire and Isle of Wight', 'South East Community', 98);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('C21', 'Probation', 'CPA Kent, Surrey & Sussex', 'South East Community', 98);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('CBS', 'Probation', 'Cambridgeshire &Peterborough', 'East Of England Community', 95);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('CHS', 'Probation', 'Cheshire', 'North West Community', 90);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('CMB', 'Probation', 'Cumbria', 'North West Community', 90);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('DBS', 'Probation', 'Derbyshire', 'East Midlands Community', 94);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('DCP', 'Probation', 'Devon & Cornwall', 'South West Community', 97);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('DRS', 'Probation', 'Dorset', 'South West Community', 97);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('DTV', 'Probation', 'Durham and Tees Valley', 'North East Community', 91);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('ESX', 'Probation', 'Essex', 'East Of England Community', 95);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('GCS', 'Probation', 'Gloucestershire', 'South West Community', 97);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('HBS', 'Probation', 'Humberside', 'Yorkshire and Humberside Community', 92);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('HFS', 'Probation', 'Hertfordshire', 'East Of England Community', 95);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('HPS', 'Probation', 'Hampshire', 'South East Community', 98);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('IWI', 'Probation', 'Isle Of Wight', 'South East Community', 98);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('KNT', 'Probation', 'Kent', 'South East Community', 98);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('LCS', 'Probation', 'Lancashire', 'North West Community', 90);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('LDN', 'Probation', 'London', 'London Community', 96);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('LNS', 'Probation', 'Lincolnshire', 'East Midlands Community', 94);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('LTS', 'Probation', 'Leicestershire & Rutland', 'East Midlands Community', 94);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('MCG', 'Probation', 'Greater Manchester', 'North West Community', 90);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('MRS', 'Probation', 'Merseyside', 'North West Community', 90);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N01', 'Probation', 'NPS North West', 'North West Community', 90);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N02', 'Probation', 'NPS North East', 'North East Community', 91);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N03', 'Probation', 'Wales', 'Unknown', null);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N04', 'Probation', 'NPS Midlands', 'National', null);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N05', 'Probation', 'NPS South West and South Central', 'National', null);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N06', 'Probation', 'NPS South East and Eastern', 'National', null);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N07', 'Probation', 'London', 'London Community', 96);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N21', 'Probation', 'External - London', 'London Community', 96);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N22', 'Probation', 'External - NPS Midlands', 'National', null);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N23', 'Probation', 'External - NPS North East', 'North East Community', 91);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N24', 'Probation', 'External - NPS North West', 'North West Community', 90);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N25', 'Probation', 'External - NPS South East & Estn', 'National', null);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N26', 'Probation', 'External - NPS South West & SC', 'National', null);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N27', 'Probation', 'External - Wales', 'Unknown', null);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N28', 'Probation', 'Ext - Greater Manchester', 'North West Community', 90);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N29', 'Probation', 'Ext - North West Region', 'North West Community', 90);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N30', 'Probation', 'Ext - West Midlands Region', 'West Midlands Community', 93);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N31', 'Probation', 'Ext - East Midlands Region', 'East Midlands Community', 94);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N32', 'Probation', 'Ext - North East Region', 'North East Community', 91);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N33', 'Probation', 'Ext - Yorkshire and The Humber', 'Yorkshire and Humberside Community', 92);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N34', 'Probation', 'Ext - East of England', 'East Of England Community', 95);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N35', 'Probation', 'Ext - Kent Surrey Sussex', 'South East Community', 98);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N36', 'Probation', 'Ext - South West', 'South West Community', 97);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N37', 'Probation', 'Ext - South Central', 'South East Community', 98);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N40', 'Probation', 'Central Projects Team', 'National', null);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N41', 'Probation', 'National Responsibility Divison', 'National', null);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N43', 'Probation', 'National Security Division', 'National', null);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N50', 'Probation', 'Greater Manchester', 'North West Community', 90);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N51', 'Probation', 'North West Region', 'North West Community', 90);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N52', 'Probation', 'West Midlands Region', 'West Midlands Community', 93);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N53', 'Probation', 'East Midlands Region', 'East Midlands Community', 94);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N54', 'Probation', 'North East Region', 'North East Community', 91);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N55', 'Probation', 'Yorkshire and The Humber', 'Yorkshire and Humberside Community', 92);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N56', 'Probation', 'East of England', 'East Of England Community', 95);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N57', 'Probation', 'Kent Surrey Sussex Region', 'South East Community', 98);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N58', 'Probation', 'South West', 'South West Community', 97);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('N59', 'Probation', 'South Central', 'South East Community', 98);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('NBR', 'Probation', 'Northumbria', 'North East Community', 91);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('NHS', 'Probation', 'Nottinghamshire', 'East Midlands Community', 94);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('NSP', 'Probation', 'Norfolk and Suffolk', 'East Of England Community', 95);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('NTS', 'Probation', 'Northamptonshire', 'East Midlands Community', 94);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('SSP', 'Probation', 'Surrey and Sussex', 'South East Community', 98);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('SWM', 'Probation', 'Staffordshire and West Midlands', 'West Midlands Community', 93);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('T01', 'Probation', 'Migration artifact - do not use', 'National', null);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('TVP', 'Probation', 'Thames Valley', 'South East Community', 98);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('WMP', 'Probation', 'West Mercia', 'West Midlands Community', 93);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('WPT', 'Probation', 'Wales Probation Trust', 'Unknown', null);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('WTS', 'Probation', 'Wiltshire', 'South West Community', 97);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('WWS', 'Probation', 'Warwickshire', 'West Midlands Community', 93);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('XXX', 'Probation', 'ZZ BAST Public Provider 1', 'National', null);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('YSN', 'Probation', 'York and North Yorkshire', 'Yorkshire and Humberside Community', 92);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('YSS', 'Probation', 'South Yorkshire', 'Yorkshire and Humberside Community', 92);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('YSW', 'Probation', 'West Yorkshire', 'Yorkshire and Humberside Community', 92);
    INSERT INTO dms.LocationMapping ([Code], [CodeType],  [Description], [DeliveryRegion], [LocationId]) VALUES ('ZMM', 'Probation', 'ZZ - Steria Monitoring Trust', 'National', null);


    -- grant access to the contract tenant to their locations
    insert into [Configuration].[TenantLocation]
    select l.Id, c.TenantId from [Configuration].[Location] l
                                     inner join [configuration].Contract c on l.ContractId = c.Id

    COMMIT TRANSACTION;

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;
        ;throw;
END CATCH