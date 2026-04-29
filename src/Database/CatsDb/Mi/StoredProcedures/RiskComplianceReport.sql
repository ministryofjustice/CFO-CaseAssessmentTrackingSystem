CREATE   PROCEDURE [mi].[RiskComplianceReport]
(
    @Date DATE,
    @ContractId NVARCHAR(50) = NULL,
    @Aggregate BIT = 1
)
AS 
BEGIN

    /*
        Licence End date and conditions are completed
        Initial Risk level at enrolment is set (all indicators as per CATS now)
        Risk set within 4 weeks of enrolment
        Probation Practitioner/OMU details to be documented on risk additional page
        Review the overall numbers of live cases set as unknown and length of time it has been set
    */

    CREATE TABLE #Cohort
    (
        [ParticipantId] NVARCHAR(9) NOT NULL,
        [LocationId] INT NULL,
        [Contract] NVARCHAR(50) NULL,
        [LicenceEndDateCompleted] BIT NULL,
        [LicenceConditionsCompleted] BIT NULL,
        [RiskLevelAtEnrolment] NVARCHAR(20) NULL,
        [RiskSetWithin4Weeks] BIT NULL,
        [ProbationPractitionerDetailsDocumented] BIT NULL,
        [NoUnknownRiskSet] BIT NULL
    )

    INSERT INTO #Cohort (ParticipantId)
    SELECT [ParticipantId] FROM Participant.EnrolmentHistory as eh
    WHERE
        eh.[From] <= DATEADD(DAY, 1, @Date)
    AND (
        eh.[To] >= @Date
            OR eh.[To] IS NULL
        )
    AND eh.EnrolmentStatus = 3

    UPDATE #Cohort
    SET LocationId = lh.LocationId,
        [Contract] = lh.Contract
    FROM #Cohort p
    inner join (

    SELECT ROW_NUMBER() OVER (partition by p.ParticipantId ORDER BY pl.[FROM] desc) AS [RowNum],
        p.ParticipantId,
        l.Id AS LocationId,
        c.[Description] AS Contract
    FROM #Cohort p
    INNER JOIN Participant.LocationHistory pl ON pl.ParticipantId = p.ParticipantId
    INNER JOIN Configuration.Location l ON l.Id = pl.LocationId
    INNER JOIN Configuration.Contract c ON c.Id = l.ContractId
    WHERE
        pl.[From] <= DATEADD(DAY, 1, @Date)
    AND (
        pl.[To] >= @Date
            OR pl.[To] IS NULL
        )
    ) as Lh on Lh.ParticipantId = p.ParticipantId
    WHERE Lh.RowNum = 1

    -- throw away anyone not in a contract area on that day
    DELETE FROM #Cohort
    WHERE LocationId IS NULL

    -- throw away anyone not in the specified contract if provided
    DELETE c FROM #Cohort c
    inner join Configuration.Contract con on c.Contract = con.Description
    WHERE @ContractId IS NOT NULL AND con.Id <> @ContractId

    -- throw away anyone who was only 'approved' on CATS within the preceeding 2 weeks.
    DELETE c FROM #Cohort c
    INNER JOIN mi.EnrolmentPayment ep on c.ParticipantId = ep.ParticipantId
    AND ep.Approved >= DATEADD(DAY, -14, @Date) AND EligibleForPayment = 1

    UPDATE c
    SET LicenceEndDateCompleted = 
        CASE 
            WHEN r.LicenseEnd IS NOT NULL THEN 1
            WHEN r.LicenseEnd IS NULL AND NoLicenseEndDate = 1 THEN 1
        ELSE NULL END,
        LicenceConditionsCompleted = 
            CASE 
                WHEN r.LicenseConditions = '' THEN NULL
                WHEN r.LicenseConditions = 'unknown' THEN NULL
                WHEN r.LicenseConditions = 'TBC' THEN NULL
                WHEN r.LicenseConditions = 'Awaiting full risk from pp' THEN NULL
                WHEN r.LicenseConditions = 'Not yet known' THEN NULL
                WHEN r.LicenseConditions = 'N/A-Unknown' THEN NULL
                WHEN r.LicenseConditions IS NOT NULL THEN 1 
                ELSE 0 END
    from #Cohort c
    LEFT JOIN (
        SELECT ROW_NUMBER() OVER (PARTITION BY ParticipantId ORDER BY Completed DESC) AS RowNum, *
        FROM Participant.Risk
        WHERE Completed <= DATEADD(DAY, 1, @Date)
    ) r on r.ParticipantId = c.ParticipantId AND r.RowNum = 1

    update c 
    set RiskSetWithin4Weeks = 1
    from #Cohort c
    where exists 
    (
        SELECT r.Id FROM Participant.Risk r
        inner join Participant.Participant as p on p.Id = r.ParticipantId
        WHERE r.ParticipantId = c.ParticipantId
        AND r.Completed <= DATEADD(DAY, 29, p.DateOfFirstConsent)
    )
    
    UPDATE 
        c 
    SET 
        [ProbationPractitionerDetailsDocumented] = 1
    FROM #Cohort c
    WHERE EXISTS (
        SELECT * FROM Participant.Supervisor
        WHERE ParticipantId = c.ParticipantId
        AND Created <= dateadd(d, 1, @Date)
    )

    UPDATE 
        c
        SET NoUnknownRiskSet = 

        CASE WHEN r.IsRelevantToCustody = 1 AND r.IsRelevantToCommunity = 1 THEN 
            CASE WHEN 
                RiskToChildrenInCustody > -1 
                AND RiskToPublicInCustody > -1 
                AND RiskToKnownAdultInCustody > -1 
                AND RiskToStaffInCustody > -1 
                AND RiskToOtherPrisonersInCustody > -1
                AND COALESCE(RiskToSelfInCustody, RiskToSelfInCustodyNew) > -1
                AND RiskToChildrenInCommunity > -1 
                AND RiskToPublicInCommunity > -1 
                AND RiskToKnownAdultInCommunity > -1 
                AND RiskToStaffInCommunity > -1 
                -- AND RiskToOtherPrisonersInCommunity > -1
                AND COALESCE(RiskToSelfInCommunity, RiskToSelfInCommunityNew) > -1
                THEN 1 ELSE NULL END
            WHEN r.IsRelevantToCustody = 1 AND r.IsRelevantToCommunity = 0 THEN 
                CASE WHEN 
                RiskToChildrenInCustody > -1 
                AND RiskToPublicInCustody > -1 
                AND RiskToKnownAdultInCustody > -1 
                AND RiskToStaffInCustody > -1 
                AND RiskToOtherPrisonersInCustody > -1
                AND COALESCE(RiskToSelfInCustody, RiskToSelfInCustodyNew) > -1
                THEN 1 ELSE NULL END
            WHEN r.IsRelevantToCustody = 0 AND r.IsRelevantToCommunity = 1 THEN CASE WHEN 
                RiskToChildrenInCommunity > -1 
                AND RiskToPublicInCommunity > -1 
                AND RiskToKnownAdultInCommunity > -1 
                AND RiskToStaffInCommunity > -1 
                -- AND RiskToOtherPrisonersInCommunity > -1
                AND COALESCE(RiskToSelfInCommunity, RiskToSelfInCommunityNew) > -1
                THEN 1 ELSE NULL END
            ELSE NULL 
        END
    FROM #Cohort c
    LEFT JOIN (
        SELECT ROW_NUMBER() OVER (PARTITION BY ParticipantId ORDER BY Completed DESC) AS RowNum, *
        FROM Participant.Risk
        WHERE Completed <= DATEADD(DAY, 1, @Date)
    ) r on r.ParticipantId = c.ParticipantId AND r.RowNum = 1

    IF @Aggregate = 1
    BEGIN
        SELECT 
            Contract, 
                COUNT(*) AS Total, 
                COUNT( LicenceEndDateCompleted ) as LicenceEndDateCompleted, 
                COUNT( LicenceConditionsCompleted ) as LicenceConditionsCompleted,
                COUNT( RiskLevelAtEnrolment ) as RiskLevelAtEnrolment,
                COUNT( RiskSetWithin4Weeks ) as RiskSetWithin4Weeks,
                COUNT( ProbationPractitionerDetailsDocumented ) as ProbationPractitionerDetailsDocumented,
                COUNT( NoUnknownRiskSet ) as NoUnknownRiskSet
            FROM #Cohort
            GROUP BY Contract
    END
    ELSE
    BEGIN
        SELECT * FROM #Cohort
    END
END
GO

