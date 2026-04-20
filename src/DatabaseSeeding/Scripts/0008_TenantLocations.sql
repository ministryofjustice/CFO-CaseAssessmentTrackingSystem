




IF NOT EXISTS (SELECT TOP(1) LocationId FROM [Configuration].[Tenantlocation])
BEGIN

    insert into [Configuration].Tenantlocation
    (
        Locationid,
        Tenantid
    )
    select Id as [LocationId],
           '1.1.2.3.1.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'HMP Foston Hall', 'HMP Stocken', 'HMP Lincoln', 'HMP Onley', 'HMP North Sea Camp', 'HMP Whatton',
                      'HMP Ranby', 'HMP Nottingham', 'HMP Five Wells', 'Nottingham', 'Leicester', 'Derby',
                      'East Midlands Community','HMP Leicester'
        )
    union
    select Id as [LocationId],
           '1.1.2.3.2.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'HMP Foston Hall', 'Nottingham', 'Leicester', 'Derby', 'North East Community' )
    union
    select Id as [LocationId],
           '1.1.2.3.3.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'HMP Stocken', 'HMP Lincoln', 'HMP North Sea Camp', 'HMP Whatton', 'North East Community' )
    union
    select Id as [LocationId],
           '1.1.2.2.1.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'HMP Drake Hall', 'HMP Featherstone', 'HMP Birmingham', 'North West Community', 'HMP Oakwood',
                      'HMP Dovegate', 'HMP Stoke Heath', 'HMP Brinsford', 'HMP Hewell', 'HMP Swinfen Hall', 'Birmingham',
                      'Wolverhampton', 'Stoke', 'West Midlands Community'
        )
    union
    select Id as [LocationId],
           '1.1.2.2.2.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'HMP Drake Hall' )
    union
    select Id as [LocationId],
           '1.1.2.2.3.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'HMP Drake Hall', 'Birmingham', 'Wolverhampton', 'Stoke', 'West Midlands Community' )
    union
    select Id as [LocationId],
           '1.1.2.1.1.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'HMP Holme House', 'HMP Low Newton', 'HMP Northumberland', 'HMP Durham', 'HMP Kirklevington Grange',
                      'Durham', 'Sunderland', 'Middlesbrough', 'Darlington', 'Newcastle', 'North East Community'
        )
    union
    select Id as [LocationId],
           '1.1.2.1.2.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'HMP Low Newton', 'Durham', 'Sunderland', 'Middlesbrough', 'Darlington', 'Newcastle',
                      'North East Community'
        )
    union
    select Id as [LocationId],
           '1.1.2.1.3.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'HMP Holme House' )
    union
    select Id as [LocationId],
           '1.1.6.1.1.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'HMP Portland', 'HMP Exeter', 'HMP Bristol', 'HMP Dartmoor', 'HMP Channings Wood', 'HMP Leyhill',
                      'HMP Guys Marsh', 'HMP The Verne', 'HMP Eastwood Park', 'Bristol', 'Plymouth', 'South West Community'
        )
    union
    select Id as [LocationId],
           '1.1.5.1.1.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'HMP High Down', 'HMP Wandsworth', 'HMP Thameside', 'HMP Pentonville', 'HMP Brixton', 'HMP Feltham',
                      'HMP Isis', 'HMP Bronzefield', 'HMP Downview', 'Croydon', 'Lambeth', 'Lewisham', 'London Community'
        )
    union
    select Id as [LocationId],
           '1.1.5.1.2.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'HMP High Down', 'HMP Wandsworth', 'HMP Thameside', 'HMP Pentonville', 'HMP Brixton', 'HMP Feltham',
                      'HMP Isis', 'HMP Bronzefield', 'HMP Downview', 'London Community'
        )
    union
    select Id as [LocationId],
           '1.1.3.1.1.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'HMP Leeds', 'HMP Askham Grange', 'HMP Full Sutton', 'HMP Wealstun', 'HMP Moorland', 'HMP Humber',
                      'HMP Doncaster', 'HMP Hull', 'HMP Hatfield', 'HMP Lindholme', 'HMP New Hall', 'Leeds', 'Doncaster',
                      'Hull', 'Bradford', 'Huddersfield', 'Sheffield', 'Yorkshire and Humberside Community'
        )
    union
    select Id as [LocationId],
           '1.1.3.1.2.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'Leeds', 'Huddersfield', 'Yorkshire and Humberside Community' )
    union
    select Id as [LocationId],
           '1.1.3.1.3.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'HMP Leeds', 'HMP Askham Grange', 'HMP Full Sutton', 'HMP Wealstun', 'HMP New Hall',
                      'Yorkshire and Humberside Community'
        )
    union
    select Id as [LocationId],
           '1.1.1.1.1.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'HMP Risley', 'HMP Lancaster Farms', 'HMP Forest Bank', 'HMP Altcourse', 'HMP Preston',
                      'HMP Kirkham', 'HMP Liverpool', 'HMP Wymott', 'HMP Buckley Hall', 'HMP Styal', 'HMP Haverigg',
                      'HMP Hindley', 'HMP Manchester', 'HMP Thorn Cross', 'Manchester', 'Liverpool', 'Blackpool',
                      'Warrington', 'Blackburn', 'North West Community'
        )
    union
    select Id as [LocationId],
           '1.1.1.1.2.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'Manchester', 'Liverpool', 'Blackpool', 'Warrington', 'Blackburn', 'Preston', 'North West Community' )
    union
    select Id as [LocationId],
           '1.1.4.2.1.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'HMP Rochester', 'HMP Elmley', 'HMP Lewes', 'HMP Winchester', 'HMP Bullingdon', 'HMP Swaleside',
                      'HMP Aylesbury', 'HMP Ford', 'HMP Spring Hill', 'HMP Stanford Hill', 'HMP Woodhill',
                      'HMP East Sutton Park', 'HMP Send', 'Chatham', 'Southampton', 'Portsmouth', 'South East Community'
        )
    union
    select Id as [LocationId],
           '1.1.4.1.1.' as [TenantId]
    from [Configuration].[Location]
    where [Name] in ( 'HMP The Mount', 'HMP Peterborough (M)', 'HMP Peterborough (F)', 'HMP Bedford', 'HMP Highpoint',
                      'HMP Littlehey', 'HMP Chelmsford', 'HMP Hollesley Bay', 'HMP Norwich', 'HMP Wayland', 'Peterborough',
                      'Luton', 'East Of England Community'
        )

END