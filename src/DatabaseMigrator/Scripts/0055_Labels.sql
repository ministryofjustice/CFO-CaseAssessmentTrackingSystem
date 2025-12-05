
IF NOT EXISTS (SELECT TOP(1) Id FROM [Configuration].[Label])
BEGIN

    INSERT INTO [Configuration].[Label] ([Id], [Name], [Description], [Colour], [Variant], [ContractId])
    VALUES
        ('1f611df9-1fb2-4059-b1ff-dcb4db6e761b', 'Test Label All', 'A test label. This will be deleted before production', 1, 0,  null),
        ('a154163b-63c8-43b1-887f-9853848c58f6', 'Test Label NW', 'A test label. This will be deleted before production',  2, 1, 'con_24036'),
        ('d609c7c8-04c0-4f8f-9612-9d192ab1cf2d', 'Test Label NE', 'A test label. This will be deleted before production',  3, 2, 'con_24037')
    
 
END