CREATE PROCEDURE [Mi].[GetRunningActuals]
AS
BEGIN

    WITH Targets AS
            (SELECT c.Description                               AS Region,
                    t.ContractId,
                    t.Year,
                    t.Month,

                    t.Prison,
                    ISNULL(PrisonPayments.eligible_count, 0)    AS PrisonActual,

                    t.Community,
                    ISNULL(CommunityPayments.eligible_count, 0) AS CommunityActual,

                    t.Wings,
                    ISNULL(WingPayments.eligible_count, 0)      AS WingActual,

                    t.Hubs,
                    ISNULL(HubPayments.eligible_count, 0)       AS HubActual,

                    t.PreReleaseSupport,
                    ISNULL(PreRelease.eligible_count, 0)        AS PreReleaseActual,

                    t.ThroughTheGate,
                    ISNULL(ttg.eligible_count, 0)               as [ThroughTheGateActual],

                    t.SupportWork,
                    ISNULL(SupportWork.eligible_count, 0)       as [SupportWorkActual],

                    t.HumanCitizenship,
                    ISNULL(hc.eligible_count, 0)                as [HumanCitizenshipActual],

                    t.CommunityAndSocial,
                    ISNULL(cas.eligible_count, 0)               as [CommunityAndSocialActual],

                    t.Interventions,
                    ISNULL(isw.eligible_count, 0)               as [InterventionsActual],

                    t.TrainingAndEducation,
                    ISNULL(ed.eligible_count, 0)                as [TrainingAndEductionActual],

                    t.Employment,
                    ISNULL(emp.eligible_count, 0)               as [EmploymentActual]

            FROM mi.ContractTarget t
                    INNER JOIN Configuration.Contract c
                                ON c.Id = t.ContractId

                    OUTER APPLY (SELECT COUNT(*) AS eligible_count
                                    FROM mi.EnrolmentPayment ep
                                    WHERE EligibleForPayment = 1
                                    AND MONTH(Approved) = t.Month
                                    AND YEAR(Approved) = t.Year
                                    AND ep.ContractId = t.ContractId
                                    AND LocationType IN ('Wing',
                                                        'Feeder',
                                                        'Outlying',
                                                        'Female',
                                                        'Unmapped Custody')
                                    GROUP BY ContractId) as PrisonPayments
                    OUTER APPLY (SELECT COUNT(*) AS eligible_count
                                    FROM mi.EnrolmentPayment ep
                                    WHERE EligibleForPayment = 1
                                    AND MONTH(Approved) = t.Month
                                    AND YEAR(Approved) = t.Year
                                    AND ep.ContractId = t.ContractId
                                    AND LocationType IN ('Community',
                                                        'Hub',
                                                        'Satellite',
                                                        'Unmapped Community')
                                    GROUP BY ContractId) as CommunityPayments
                    OUTER APPLY (SELECT COUNT(*) AS eligible_count
                                    FROM mi.InductionPayment as ip
                                    WHERE EligibleForPayment = 1
                                    AND MONTH(Approved) = t.Month
                                    AND YEAR(Approved) = t.Year
                                    AND ip.ContractId = t.ContractId
                                    AND LocationType IN ('Wing')
                                    GROUP BY ContractId) as WingPayments
                    OUTER APPLY (SELECT COUNT(*) AS eligible_count
                                    FROM mi.InductionPayment as ip
                                    WHERE EligibleForPayment = 1
                                    AND MONTH(Approved) = t.Month
                                    AND YEAR(Approved) = t.Year
                                    AND ip.ContractId = t.ContractId
                                    AND LocationType IN ('Hub')
                                    GROUP BY ContractId) as HubPayments
                    OUTER APPLY (SELECT COUNT(*) AS eligible_count
                                    FROM mi.SupportAndReferralPayment as sp
                                    WHERE EligibleForPayment = 1
                                    AND MONTH(Approved) = t.Month
                                    AND YEAR(Approved) = t.Year
                                    AND sp.ContractId = t.ContractId
                                    AND SupportType IN ('Pre-Release Support')
                                    GROUP BY ContractId) as PreRelease
                    OUTER APPLY (SELECT COUNT(*) AS eligible_count
                                    FROM mi.SupportAndReferralPayment as sp
                                    WHERE EligibleForPayment = 1
                                    AND MONTH(Approved) = t.Month
                                    AND YEAR(Approved) = t.Year
                                    AND sp.ContractId = t.ContractId
                                    AND SupportType IN ('Through the Gate')
                                    GROUP BY ContractId) as ttg

                -- Support work is different we need to union two tables
                    OUTER APPLY (SELECT sum(sp.eligible_count) as [eligible_count]
                                    FROM (SELECT COUNT(*) AS eligible_count
                                        FROM mi.ActivityPayment as ap
                                        WHERE EligibleForPayment = 1
                                            AND MONTH(PaymentPeriod) = t.Month
                                            AND YEAR(PaymentPeriod) = t.Year
                                            AND ap.ContractId = t.ContractId
                                            AND ActivityType = 'Support Work'
                                        GROUP BY ContractId
                                        UNION ALL
                                        SELECT COUNT(*) AS eligible_count
                                        FROM mi.ReassessmentPayment as ap
                                        WHERE EligibleForPayment = 1
                                            AND MONTH(PaymentPeriod) = t.Month
                                            AND YEAR(PaymentPeriod) = t.Year
                                            AND ap.ContractId = t.ContractId) as sp) as SupportWork

                    OUTER APPLY (SELECT COUNT(*) AS eligible_count
                                    FROM mi.ActivityPayment as ap
                                    WHERE EligibleForPayment = 1
                                    AND MONTH(PaymentPeriod) = t.Month
                                    AND YEAR(PaymentPeriod) = t.Year
                                    AND ap.ContractId = t.ContractId
                                    AND ActivityType = 'Human Citizenship'
                                    GROUP BY ContractId) as hc

                    OUTER APPLY (SELECT COUNT(*) AS eligible_count
                                    FROM mi.ActivityPayment as ap
                                    WHERE EligibleForPayment = 1
                                    AND MONTH(PaymentPeriod) = t.Month
                                    AND YEAR(PaymentPeriod) = t.Year
                                    AND ap.ContractId = t.ContractId
                                    AND ActivityType = 'Community and Social'
                                    GROUP BY ContractId) as cas

                    OUTER APPLY (SELECT COUNT(*) AS eligible_count
                                    FROM mi.ActivityPayment as ap
                                    WHERE EligibleForPayment = 1
                                    AND MONTH(PaymentPeriod) = t.Month
                                    AND YEAR(PaymentPeriod) = t.Year
                                    AND ap.ContractId = t.ContractId
                                    AND ActivityType = 'ISW Support'
                                    GROUP BY ContractId) as isw

                    OUTER APPLY (SELECT COUNT(*) AS eligible_count
                                    FROM mi.EducationPayment as ep
                                    WHERE EligibleForPayment = 1
                                    AND MONTH(PaymentPeriod) = t.Month
                                    AND YEAR(PaymentPeriod) = t.Year
                                    AND ep.ContractId = t.ContractId
                                    GROUP BY ContractId) as ed

                    OUTER APPLY (SELECT COUNT(*) AS eligible_count
                                    FROM mi.EmploymentPayment as ep
                                    WHERE EligibleForPayment = 1
                                    AND MONTH(PaymentPeriod) = t.Month
                                    AND YEAR(PaymentPeriod) = t.Year
                                    AND ep.ContractId = t.ContractId
                                    GROUP BY ContractId) as emp)
    SELECT Region,
        CASE ContractId
            WHEN 'con_24036' THEN 'Achieve'
            WHEN 'con_24037' THEN 'Ingeus'
            WHEN 'con_24038' THEN 'The Growth Co'
            WHEN 'con_24041' THEN 'Ingeus'
            WHEN 'con_24042' THEN 'Ingeus'
            WHEN 'con_24043' THEN 'Shaw Trust'
            WHEN 'con_24044' THEN 'Reed'
            WHEN 'con_24045' THEN 'Seetec'
            WHEN 'con_24046' THEN 'Shaw Trust'
            END as [Tenant],
        Year,
        Month,
        x.[Group],
        x.SubGroup,
        x.Type,
        x.TargetValue,
        x.ActualValue
    FROM Targets t
            CROSS APPLY
        (VALUES ('Attachments', 'Enrolments', 'Prison',
                t.Prison, t.PrisonActual),

                ('Attachments', 'Enrolments', 'Community',
                t.Community, t.CommunityActual),

                ('Attachments', 'Inductions', 'Wings',
                t.Wings, t.WingActual),

                ('Attachments', 'Inductions', 'Hubs',
                t.Hubs, t.HubActual),

                ('Support & Referral', 'Support & Referral', 'Pre-Release Support',
                t.PreReleaseSupport, t.PreReleaseActual),

                ('Support & Referral', 'Support & Referral', 'Through the Gate',
                t.ThroughTheGate, t.ThroughTheGateActual),

                ('Activities', 'Activities', 'Support Work', t.SupportWork, t.SupportWorkActual),

                ('Activities', 'Activities', 'Human Citizenship', t.HumanCitizenship, t.HumanCitizenshipActual),

                ('Activities', 'Activities', 'Community and Social', t.CommunityAndSocial, t.CommunityAndSocialActual),

                ('Activities', 'Activities', 'ISW Support', t.Interventions, t.InterventionsActual),

                ('ETE', 'ETE', 'Education & Training', t.TrainingAndEducation, t.TrainingAndEductionActual),

                ('ETE', 'ETE', 'Employment', t.Employment, t.EmploymentActual)) x (
                                                                                    [Group],
                                                                                    SubGroup,
                                                                                    [Type],
                                                                                    TargetValue,
                                                                                    ActualValue
            )
    ORDER BY ContractId,
            Year,
            Month,
            CASE x.[Group]
                WHEN 'Attachments' THEN 1
                WHEN 'Support & Referral' THEN 2
                WHEN 'Activities' THEN 3
                WHEN 'ETE' THEN 4
                END,
            CASE SubGroup
                WHEN 'Enrolments' THEN 1
                WHEN 'Inductions' THEN 2
                WHEN 'Support & Referral' THEN 3
                WHEN 'Activities' THEN 3
                WHEN 'ETE' THEN 4
                END,
            CASE Type
                WHEN 'Prison' THEN 1
                WHEN 'Community' THEN 2
                WHEN 'Wings' THEN 3
                WHEN 'Hubs' THEN 4
                WHEN 'Pre-Release Support' THEN 5
                WHEN 'Through the Gate' THEN 6
                WHEN 'Support Work' THEN 7
                WHEN 'Human Citizenship' THEN 8
                WHEN 'Community and Social' THEN 9
                WHEN 'ISW Support' THEN 10
                WHEN 'Education & Training' THEN 11
                WHEN 'Employment' THEN 12
                END

END
