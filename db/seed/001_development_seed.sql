
SET NOCOUNT ON;

BEGIN TRANSACTION;

BEGIN TRY

    INSERT INTO dbo.Tenant (Id, Name, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'1.', N'CFO', N'Root tenant for Creating Future Opportunities', N'2024-05-31 12:30:38.3849332', null, null, null);
    INSERT INTO dbo.Tenant (Id, Name, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'1.1.', N'CFO Evolution', N'Top level tenant for Evolution Programme', N'2024-05-31 12:30:38.3850497', null, null, null);
    INSERT INTO dbo.Tenant (Id, Name, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'1.1.1.', N'Career Connect', N'Career Connect', N'2024-05-31 12:30:38.3850635', null, null, null);
    INSERT INTO dbo.Tenant (Id, Name, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'1.1.2.', N'Ingeus', N'Top level tenant for provider Ingeus', N'2024-05-31 12:30:38.3850637', null, null, null);
    INSERT INTO dbo.Tenant (Id, Name, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'1.1.2.1.', N'Ingeus (NE)', N'Ingeus (North East)', N'2024-05-31 12:30:38.3850637', null, null, null);
    INSERT INTO dbo.Tenant (Id, Name, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'1.1.2.2.', N'Ingeus (WM)', N'Ingeus (West Midlands)', N'2024-05-31 12:30:38.3850637', null, null, null);
    INSERT INTO dbo.Tenant (Id, Name, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'1.1.2.3.', N'Ingeus (EM)', N'Ingeus (East Midlands)', N'2024-05-31 12:30:38.3850637', null, null, null);
    INSERT INTO dbo.Tenant (Id, Name, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'1.1.3.', N'The Growth Co', N'The Growth Co', N'2024-05-31 12:30:38.3850645', null, null, null);
    INSERT INTO dbo.Tenant (Id, Name, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'1.1.4.', N'Shaw Trust', N'Top level tenant for Shaw Trust', N'2024-05-31 12:30:38.3850647', null, null, null);
    INSERT INTO dbo.Tenant (Id, Name, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'1.1.4.1.', N'Shaw Trust (EoE)', N'Shaw Trust (East of England)', N'2024-05-31 12:30:38.3850647', null, null, null);
    INSERT INTO dbo.Tenant (Id, Name, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'1.1.4.2.', N'Shaw Trust (SE)', N'Shaw Trust (South East)', N'2024-05-31 12:30:38.3850647', null, null, null);
    INSERT INTO dbo.Tenant (Id, Name, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'1.1.5.', N'Reed', N'Reed', N'2024-05-31 12:30:38.3850651', null, null, null);
    INSERT INTO dbo.Tenant (Id, Name, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'1.1.6.', N'Seetec', N'Seetec', N'2024-05-31 12:30:38.3850653', null, null, null);

    INSERT INTO dbo.ApplicationRole (Id, Description, Name, NormalizedName, ConcurrencyStamp) VALUES ('a6bc93b6-0c06-43e7-bc8f-8d1f1ff6f136', N'User responsible for finance-related tasks', N'Finance', N'FINANCE', N'e6340a9c-8144-4f9d-8489-2e21af06bc56');
    INSERT INTO dbo.ApplicationRole (Id, Description, Name, NormalizedName, ConcurrencyStamp) VALUES ('b2ac2d5a-9f2a-43c1-a2b6-4b9f2b8b6c5a', N'User responsible for monitoring and enhancing performance', N'Performance', N'PERFORMANCE', N'fc3d1f2e-7d34-4eb9-8748-6f23d6e1b1a2');
    INSERT INTO dbo.ApplicationRole (Id, Description, Name, NormalizedName, ConcurrencyStamp) VALUES ('c3db7d1b-1d5a-4e79-9d0f-3e9d1f3d1f1c', N'User responsible for ensuring quality and standards', N'Quality Assurance', N'QUALITY ASSURANCE', N'f6a1d7e3-2c4b-4f8a-93d2-4e6b1f7d2c3b');
    INSERT INTO dbo.ApplicationRole (Id, Description, Name, NormalizedName, ConcurrencyStamp) VALUES ('d4ec8e2c-2e6a-4f8b-a2f0-4f0d2e8e2e4d', N'User responsible for providing assistance and solutions to users', N'Service Desk', N'SERVICE DESK', N'1f2e3d4c-5a6b-4e8c-9d1a-2e3f4d1c5b6a');
    INSERT INTO dbo.ApplicationRole (Id, Description, Name, NormalizedName, ConcurrencyStamp) VALUES ('e5fd9f3d-3f7a-5f9c-b3f1-5f1e3f9f3f5e', N'User responsible for analyzing and interpreting statistical data', N'Statistics', N'STATISTICS', N'2e3f4d5c-6b7a-5e9d-1a2e-3f4d5c6b7a1b');
    INSERT INTO dbo.ApplicationRole (Id, Description, Name, NormalizedName, ConcurrencyStamp) VALUES ('f6ge1f4e-4f8a-6fa0-c4f2-6f2e4f1f4f6f', N'The core user of the system, works with participants and records information about the participant journey.', N'Support Worker', N'SUPPORT WORKER', N'3e4f5d6c-7b8a-6e9e-2a3e-4f5d6c7b8a2e');
    INSERT INTO dbo.ApplicationRole (Id, Description, Name, NormalizedName, ConcurrencyStamp) VALUES ('g7hf2g5f-5g9a-7gb1-d5f3-7f3e5f2f5g7g', N'User responsible for system-level support and maintenance.', N'System Support', N'SYSTEM SUPPORT', N'4e5f6d7c-8b9a-7e9f-3a4e-5f6d7c8b9a3e');


    INSERT INTO dbo.ApplicationUser (Id, DisplayName, ProviderId, TenantId, TenantName, ProfilePictureDataUrl, IsActive, IsLive, RefreshToken, RequiresPasswordReset, RefreshTokenExpiryTime, SuperiorId, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount) 
    VALUES ('b6228bae-476d-4816-ba62-f12687d40a96', N'Support Worker', N'1.', N'1.', N'CFO', N'https://avatars.githubusercontent.com/u/9332472?s=400&u=73c208bf07ba967d5407aae9068580539cfc80a2&v=4', 1, 0, null, 0, N'0001-01-01 00:00:00.0000000', null, N'support.worker@justice.gov.uk', N'SUPPORT.WORKER@JUSTICE.GOV.UK', N'support.worker@justice.gov.uk', N'SUPPORT.WORKER@JUSTICE.GOV.UK', 1, N'AQAAAAIAAYagAAAAEAVdTO5H3lP4OTTB1CzJun94RqDWYMHHxDYYLCcf3zER/g2IRxukunJEOYggb0xWZg==', N'5OAC7CKMMFBY3SMD4MZ7GKVDZRZ7CVUZ', N'49dbf347-de6b-4579-997b-3cfd4d79afa0', null, 0, 0, null, 1, 0);

    INSERT INTO dbo.ApplicationUserRole (UserId, RoleId) VALUES ('b6228bae-476d-4816-ba62-f12687d40a96', 'g7hf2g5f-5g9a-7gb1-d5f3-7f3e5f2f5g7g');
    


    INSERT INTO dbo.Contract (Id, LotNumber, LifetimeStart, LifetimeEnd, Description, TenantId, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'Evolution1', 1, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'North West', N'1.1.1.', N'2024-05-31 12:30:38.9702330', null, null, null);
    INSERT INTO dbo.Contract (Id, LotNumber, LifetimeStart, LifetimeEnd, Description, TenantId, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'Evolution2', 2, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'North East', N'1.1.2.1.', N'2024-05-31 12:30:38.9702326', null, null, null);
    INSERT INTO dbo.Contract (Id, LotNumber, LifetimeStart, LifetimeEnd, Description, TenantId, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'Evolution3', 3, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'Yorkshire and Humberside', N'1.1.3.', N'2024-05-31 12:30:38.9702322', null, null, null);
    INSERT INTO dbo.Contract (Id, LotNumber, LifetimeStart, LifetimeEnd, Description, TenantId, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'Evolution4', 4, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'West Midlands', N'1.1.2.2.', N'2024-05-31 12:30:38.9702315', null, null, null);
    INSERT INTO dbo.Contract (Id, LotNumber, LifetimeStart, LifetimeEnd, Description, TenantId, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'Evolution5', 5, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'East Midlands', N'1.1.2.3.', N'2024-05-31 12:30:38.9702311', null, null, null);
    INSERT INTO dbo.Contract (Id, LotNumber, LifetimeStart, LifetimeEnd, Description, TenantId, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'Evolution6', 6, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'East Of England', N'1.1.4.1.', N'2024-05-31 12:30:38.9702308', null, null, null);
    INSERT INTO dbo.Contract (Id, LotNumber, LifetimeStart, LifetimeEnd, Description, TenantId, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'Evolution7', 7, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'London', N'1.1.5.', N'2024-05-31 12:30:38.9702304', null, null, null);
    INSERT INTO dbo.Contract (Id, LotNumber, LifetimeStart, LifetimeEnd, Description, TenantId, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'Evolution8', 8, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'South West', N'1.1.6.', N'2024-05-31 12:30:38.9702300', null, null, null);
    INSERT INTO dbo.Contract (Id, LotNumber, LifetimeStart, LifetimeEnd, Description, TenantId, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (N'Evolution9', 9, N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', N'South East', N'1.1.4.2.', N'2024-05-31 12:30:38.9702153', null, null, null);

    

    SET IDENTITY_INSERT KeyValue ON;

    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (1, N'ReferralSource', N'CFO Evolution Provider', N'CFO Evolution Provider', N'A referral source', N'2024-05-31 12:30:42.7615462', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (2, N'ReferralSource', N'Probation', N'Probation', N'A referral source', N'2024-05-31 12:30:42.7615457', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (3, N'ReferralSource', N'Approved Premises', N'Approved Premises', N'A referral source', N'2024-05-31 12:30:42.7615453', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (4, N'ReferralSource', N'CAS2', N'CAS2', N'A referral source', N'2024-05-31 12:30:42.7615451', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (5, N'ReferralSource', N'CAS3', N'CAS3', N'A referral source', N'2024-05-31 12:30:42.7615449', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (6, N'ReferralSource', N'Custodial Family Services', N'Custodial Family Services', N'A referral source', N'2024-05-31 12:30:42.7615448', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (7, N'ReferralSource', N'CRS - Women', N'CRS- Women', N'A referral source', N'2024-05-31 12:30:42.7615445', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (8, N'ReferralSource', N'CRS - Personal Wellbeing', N'CRS- Personal Wellbeing', N'A referral source', N'2024-05-31 12:30:42.7615443', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (9, N'ReferralSource', N'CRS - Dependency & Recovery', N'CRS- Dependency & Recovery', N'A referral source', N'2024-05-31 12:30:42.7615441', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (10, N'ReferralSource', N'CRS - Accommodation', N'CRS- Accommodation', N'A referral source', N'2024-05-31 12:30:42.7615439', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (11, N'ReferralSource', N'Custody staff', N'Custody staff', N'A referral source', N'2024-05-31 12:30:42.7615436', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (12, N'ReferralSource', N'New Futures Network', N'New Futures Network', N'A referral source', N'2024-05-31 12:30:42.7615428', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (13, N'ReferralSource', N'Prison Education Provider', N'Prison Education Provider', N'A referral source', N'2024-05-31 12:30:42.7615425', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (14, N'ReferralSource', N'DWP', N'DWP', N'A referral source', N'2024-05-31 12:30:42.7615421', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (15, N'ReferralSource', N'Healthcare', N'Healthcare', N'A referral source', N'2024-05-31 12:30:42.7615417', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (16, N'ReferralSource', N'Community / Voluntary Sector organisation', N'Community / Voluntary Sector organisation', N'A referral source', N'2024-05-31 12:30:42.7615414', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (17, N'ReferralSource', N'Local Authority', N'Local Authority', N'A referral source', N'2024-05-31 12:30:42.7615410', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (18, N'ReferralSource', N'Courts', N'Courts', N'A referral source', N'2024-05-31 12:30:42.7615390', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (19, N'ReferralSource', N'Self-referral', N'Self-referral', N'A referral source', N'2024-05-31 12:30:42.7615372', null, null, null);
    INSERT INTO dbo.KeyValue (Id, Name, Value, Text, Description, Created, CreatedBy, LastModified, LastModifiedBy) VALUES (20, N'ReferralSource', N'Other', N'Other', N'A referral source (please state)', N'2024-05-31 12:30:42.7615265', null, null, null);

    SET IDENTITY_INSERT KeyValue OFF;

    

    SET IDENTITY_INSERT dbo.Location ON;

    MERGE INTO CatsDb.dbo.Location AS Target
    USING (VALUES
            (1, N'Risley', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888014', null, null, null),
            (2, N'Lancaster', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888012', null, null, null),
            (3, N'Forest Bank', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888009', null, null, null),
            (4, N'Altcourse', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888007', null, null, null),
            (5, N'Preston', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888004', null, null, null),
            (6, N'Buckley Hall', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888001', null, null, null),
            (7, N'Liverpool', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5887998', null, null, null),
            (8, N'Manchester', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5887995', null, null, null),
            (9, N'Thorn Cross', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5887991', null, null, null),
            (10, N'Haverigg', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5887988', null, null, null),
            (11, N'Hindley', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5887984', null, null, null),
            (12, N'Kirkham', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5887894', null, null, null),
            (13, N'Wymott', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5886872', null, null, null),
            (14, N'Styal', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888017', null, null, null),
            (15, N'Holme House', N'Evolution2', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888025', null, null, null),
            (16, N'Northumberland', N'Evolution2', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888028', null, null, null),
            (17, N'Durham', N'Evolution2', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888030', null, null, null),
            (18, N'Kirklevington Grange', N'Evolution2', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888032', null, null, null),
            (19, N'Low Newton', N'Evolution2', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888034', null, null, null),
            (20, N'Wealstun', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888037', null, null, null),
            (21, N'Moorland', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888043', null, null, null),
            (22, N'Humber', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888046', null, null, null),
            (23, N'Doncaster', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888051', null, null, null),
            (24, N'Leeds', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888053', null, null, null),
            (25, N'Hull', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888055', null, null, null),
            (26, N'Full Sutton', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888058', null, null, null),
            (27, N'Hatfield', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888060', null, null, null),
            (28, N'Lindholme', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888063', null, null, null),
            (29, N'Askham Grange', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888066', null, null, null),
            (30, N'New Hall', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888076', null, null, null),
            (31, N'Featherstone', N'Evolution4', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888088', null, null, null),
            (32, N'Drake Hall', N'Evolution4', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888091', null, null, null),
            (33, N'Birmingham', N'Evolution4', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888093', null, null, null),
            (34, N'Brinsford', N'Evolution4', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888096', null, null, null),
            (35, N'Dovegate', N'Evolution4', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888101', null, null, null),
            (36, N'Hewell', N'Evolution4', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888104', null, null, null),
            (37, N'Oakwood', N'Evolution4', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888106', null, null, null),
            (38, N'Stoke Heath', N'Evolution4', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888108', null, null, null),
            (39, N'Swinfen Hall', N'Evolution4', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888111', null, null, null),
            (40, N'Ranby', N'Evolution5', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888114', null, null, null),
            (41, N'Nottingham', N'Evolution5', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888118', null, null, null),
            (42, N'Five Wells', N'Evolution5', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888121', null, null, null),
            (43, N'Lincoln', N'Evolution5', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888125', null, null, null),
            (44, N'North Sea Camp', N'Evolution5', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888127', null, null, null),
            (45, N'Onley', N'Evolution5', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888130', null, null, null),
            (46, N'Stocken', N'Evolution5', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888132', null, null, null),
            (47, N'Whatton', N'Evolution5', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888135', null, null, null),
            (48, N'Foston Hall', N'Evolution5', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888138', null, null, null),
            (49, N'The Mount', N'Evolution6', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888140', null, null, null),
            (50, N'Peterborough (M)', N'Evolution6', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888143', null, null, null),
            (51, N'Bedford', N'Evolution6', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888146', null, null, null),
            (52, N'Chelmsford', N'Evolution6', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888149', null, null, null),
            (53, N'Highpoint', N'Evolution6', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888151', null, null, null),
            (54, N'Hollesley Bay', N'Evolution6', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888153', null, null, null),
            (55, N'Littlehey', N'Evolution6', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888157', null, null, null),
            (56, N'Norwich', N'Evolution6', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888159', null, null, null),
            (57, N'Wayland', N'Evolution6', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888162', null, null, null),
            (58, N'Peterborough (F)', N'Evolution6', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888165', null, null, null),
            (59, N'High Down', N'Evolution7', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888167', null, null, null),
            (60, N'Wandsworth', N'Evolution7', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888170', null, null, null),
            (61, N'Thameside', N'Evolution7', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888171', null, null, null),
            (62, N'Brixton', N'Evolution7', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888179', null, null, null),
            (63, N'Feltham', N'Evolution7', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888181', null, null, null),
            (64, N'Isis', N'Evolution7', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888184', null, null, null),
            (65, N'Pentonville', N'Evolution7', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888186', null, null, null),
            (66, N'Bronzefield', N'Evolution7', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888189', null, null, null),
            (67, N'Downview', N'Evolution7', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888197', null, null, null),
            (68, N'Portland', N'Evolution8', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888199', null, null, null),
            (69, N'Exeter', N'Evolution8', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888201', null, null, null),
            (70, N'Bristol', N'Evolution8', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888204', null, null, null),
            (71, N'Channings Wood', N'Evolution8', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888206', null, null, null),
            (72, N'Dartmoor', N'Evolution8', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888209', null, null, null),
            (73, N'Leyhill', N'Evolution8', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888212', null, null, null),
            (74, N'Guys Marsh', N'Evolution8', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888214', null, null, null),
            (75, N'The Verne', N'Evolution8', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888217', null, null, null),
            (76, N'Eastwood Park', N'Evolution8', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888219', null, null, null),
            (77, N'Rochester', N'Evolution9', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 0, N'2024-05-31 12:30:39.5888221', null, null, null),
            (78, N'Elmley', N'Evolution9', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888224', null, null, null),
            (79, N'Lewes', N'Evolution9', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888227', null, null, null),
            (80, N'Winchester', N'Evolution9', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 1, N'2024-05-31 12:30:39.5888229', null, null, null),
            (81, N'Aylesbury', N'Evolution9', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888232', null, null, null),
            (82, N'Bullingdon', N'Evolution9', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888234', null, null, null),
            (83, N'Ford', N'Evolution9', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888237', null, null, null),
            (84, N'Springhill', N'Evolution9', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888239', null, null, null),
            (85, N'Stanford Hill', N'Evolution9', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888241', null, null, null),
            (86, N'Swaleside', N'Evolution9', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888244', null, null, null),
            (87, N'Woodhill', N'Evolution9', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 0, 2, N'2024-05-31 12:30:39.5888246', null, null, null),
            (88, N'Send', N'Evolution9', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888253', null, null, null),
            (89, N'East Sutton Park', N'Evolution9', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 1, 3, N'2024-05-31 12:30:39.5888312', null, null, null),
            (90, N'North West Community', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
            (91, N'North East Community', N'Evolution2', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
            (92, N'Yorkshire and Humberside Community', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
            (93, N'West Midlands Community', N'Evolution4', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
            (94, N'East Midlands Community', N'Evolution5', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
            (95, N'East Of England Community', N'Evolution6', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
            (96, N'London Community', N'Evolution7', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
            (97, N'South West Community', N'Evolution8', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
            (98, N'South East Community', N'Evolution9', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 4, N'2024-05-31 12:30:39.5888312', null, null, null),
            (99, N'Manchester', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
            (100, N'Liverpool', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
            (101, N'Warrington', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 100, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
            (102, N'Blackpool', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
            (103, N'Preston', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 102, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
            (104, N'Blackburn', N'Evolution1', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 102, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
            (105, N'Durham', N'Evolution2', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
            (106, N'Middlesbrough', N'Evolution2', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 105, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
            (107, N'Darlington', N'Evolution2', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 105, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
            (108, N'Sunderland', N'Evolution2', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
            (109, N'Newcastle', N'Evolution2', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 108, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
            (110, N'Leeds', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
            (111, N'Bradford', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 110, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
            (112, N'Huddersfield', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 110, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
            (113, N'Doncaster', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
            (114, N'Sheffield', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 113, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
            (115, N'Hull', N'Evolution3', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
            (116, N'Birmingham', N'Evolution4', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
            (117, N'Wolverhampton', N'Evolution4', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
            (118, N'Stoke', N'Evolution4', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 117, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
            (119, N'Nottingham', N'Evolution5', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
            (120, N'Leicester', N'Evolution5', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 119, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
            (121, N'Derby', N'Evolution5', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 119, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
            (122, N'Peterborough', N'Evolution6', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
            (123, N'Luton', N'Evolution6', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 122, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
            (124, N'Croydon', N'Evolution7', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
            (125, N'Lambeth', N'Evolution7', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 124, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
            (126, N'Lewisham', N'Evolution7', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 124, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
            (127, N'Bristol', N'Evolution8', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
            (128, N'Plymouth', N'Evolution8', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 127, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
            (129, N'Medway', N'Evolution9', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', null, 2, 5, N'2024-05-31 12:30:39.5888312', null, null, null),
            (130, N'Southampton', N'Evolution9', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 129, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null),
            (131, N'Portsmouth', N'Evolution9', N'2024-05-01 00:00:00.0000000', N'2029-03-31 23:59:59.0000000', 129, 2, 6, N'2024-05-31 12:30:39.5888312', null, null, null)
    ) AS Source (Id, Name, ContractId, LifetimeStart, LifetimeEnd, ParentLocationId, GenderProvisionId, LocationTypeId, Created, CreatedBy, LastModified, LastModifiedBy)
    ON Target.Id = Source.Id
    WHEN MATCHED THEN
        UPDATE SET Target.Name = Source.Name, Target.ContractId = Source.ContractId, Target.LifetimeStart = Source.LifetimeStart, Target.LifetimeEnd = Source.LifetimeEnd, Target.ParentLocationId = Source.ParentLocationId, Target.GenderProvisionId = Source.GenderProvisionId, Target.LocationTypeId = Source.LocationTypeId, Target.Created = Source.Created, Target.CreatedBy = Source.CreatedBy, Target.LastModified = Source.LastModified, Target.LastModifiedBy = Source.LastModifiedBy
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Id, Name, ContractId, LifetimeStart, LifetimeEnd, ParentLocationId, GenderProvisionId, LocationTypeId, Created, CreatedBy, LastModified, LastModifiedBy)
        VALUES (Source.Id, Source.Name, Source.ContractId, Source.LifetimeStart, Source.LifetimeEnd, Source.ParentLocationId, Source.GenderProvisionId, Source.LocationTypeId, Source.Created, Source.CreatedBy, Source.LastModified, Source.LastModifiedBy);


    SET IDENTITY_INSERT dbo.Location OFF;


    -- grant access to the contract tenant to their locations
    insert into dbo.TenantLocation
    select l.Id, c.TenantId from dbo.Location l
    inner join dbo.Contract c on l.ContractId = c.Id

    COMMIT TRANSACTION;

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
    BEGIN
        ROLLBACK TRANSACTION;
    END;
    ;throw;
END CATCH