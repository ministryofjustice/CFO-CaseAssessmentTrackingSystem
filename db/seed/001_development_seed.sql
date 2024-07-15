
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