
IF NOT EXISTS (SELECT TOP(1) [Value] FROM [Lookup].[EnrolmentStatus])
BEGIN

    INSERT INTO [Lookup].[EnrolmentStatus] ( [Value], [Name], [LogicalOrder] )
    VALUES 
    (0, 'Identified', 0),
    (1, 'Submitted To Provider', 2),
    (2, 'Submitted To Authority', 3),
    (3, 'Approved', 4),
    (4, 'Archived', 5),
    (5, 'Dormant', 6),
    (6, 'Enrolling', 1);

END

IF NOT EXISTS (SELECT TOP(1) [Value] FROM [Lookup].[ConsentStatus])
BEGIN

    INSERT INTO [Lookup].[ConsentStatus] ( [Value], [Name] )
    VALUES
    (0, 'Pending'),
    (1, 'Granted');

END

IF NOT EXISTS (SELECT TOP(1) [Value] FROM [Lookup].[RiskDueReason])
BEGIN

    INSERT INTO [Lookup].[RiskDueReason] ( [Value], [Name] )
    VALUES
    (0, 'No Reason Specified'),
    (1, 'New Entry'),
    (2, 'Initial Review'),
    (3, 'Data Feed Updated'),
    (4, 'Ten Week Review'),
    (5, 'Removed From Archive');

END