

IF NOT EXISTS (SELECT TOP(1) Domain FROM [Configuration].[TenantDomain])
BEGIN
    INSERT INTO Configuration.TenantDomain (Domain, TenantId, Created, CreatedBy, LastModified, LastModifiedBy) 
    VALUES 
        (N'@justice.gov.uk', N'1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@justice.gov.uk', N'1.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@achieve.org', N'1.1.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@northwestcontract.org', N'1.1.1.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@achieve_nw.org', N'1.1.1.1.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@ingeus.org', N'1.1.2.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@northeastcontract.org', N'1.1.2.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@igneus_ne.org', N'1.1.2.1.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@awayout_ne.org', N'1.1.2.1.2.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@forcesemploymentcharity_ne.org', N'1.1.2.1.3.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@westmidlandscontract.org', N'1.1.2.2.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@igneus_wm.org', N'1.1.2.2.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@awayout_wm.org', N'1.1.2.2.2.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@changinglives_wm.org', N'1.1.2.2.3.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@eastmidlandscontract.org', N'1.1.2.3.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@igneus_em.org', N'1.1.2.3.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@changinglives_em.org', N'1.1.2.3.2.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@lat_em.org', N'1.1.2.3.3.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@thegrowthco.org', N'1.1.3.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@yorkshireandhumbersidecontract.org', N'1.1.3.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@growth_yh.org', N'1.1.3.1.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@communitylinks_yh.org', N'1.1.3.1.2.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@stgiles_yh.org', N'1.1.3.1.3.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@shawtrust.org', N'1.1.4.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@eastofenglandcontract.org', N'1.1.4.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@shawtrust_ee.org', N'1.1.4.1.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@southeastcontract.org', N'1.1.4.2.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@shawtrust_se.org', N'1.1.4.2.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@reed.org', N'1.1.5.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@londoncontract.org', N'1.1.5.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@reed_lnd.org', N'1.1.5.1.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@shawtrust_lnd.org', N'1.1.5.1.2.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@seetec.org', N'1.1.6.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@southwestcontract.org', N'1.1.6.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null),
        (N'@seetec_sw.org', N'1.1.6.1.1.', '01 Aug 2024', N'2a9b3450-1feb-4be3-ab94-24e64cd34829', null, null);
END;