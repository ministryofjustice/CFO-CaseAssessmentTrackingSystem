BEGIN 
    INSERT INTO [Configuration].[Label] (
    [Id], [Name], [Description], [Colour], [Variant], [ContractId],
    [AppIcon], [Scope]
)
SELECT NewLabels.*
FROM (
         VALUES
             (CAST('01A09FD9-09E2-7CC3-85D1-21C842871F54' AS UNIQUEIDENTIFIER), 'AP Resident', 'Participant is currently residing in an Approved Premises', 4, 0, CAST(NULL AS NVARCHAR(12)), 14, 0),
             (CAST('01A09FD9-09E2-7CC3-85D1-21C842871F56' AS UNIQUEIDENTIFIER), 'CSCS Required', 'Participants requires CSCS card to further employment opportunities ', 5, 0, NULL, 4, 0),
             (CAST('01A09FD7-EBDD-7496-A81B-6B7705B9EA45' AS UNIQUEIDENTIFIER), 'AP Required', 'Participant is seeking Approved Premises', 5, 0, NULL, 14, 0),
             (CAST('01A09FD7-EBDD-7496-A81B-6B7705B9EA47' AS UNIQUEIDENTIFIER), 'CSCS Cardholder', 'Participant holds a CSCS card', 4, 0, NULL, 4, 0),
             (CAST('01A09FD7-EBDD-7496-A81B-6B7705B9EA48' AS UNIQUEIDENTIFIER), 'Disengaged', 'Repeated attempts to engage a participant have been unsuccessful ', 5, 0, NULL, 23, 0),
             (CAST('01A09FD7-EBDD-7496-A81B-6B7705B9EA4A' AS UNIQUEIDENTIFIER), 'DRR', 'Drug Rehabilitation Requirement - A court ordered requirement for treatment of drug dependency in the UK', 6, 0, NULL, 16, 0),
             (CAST('01A09FD7-EBDD-7496-A81B-6B7705B9EA4B' AS UNIQUEIDENTIFIER), 'Employed', 'Participant is employed, which may impact availability and support required', 5, 0, NULL, 12, 0),
             (CAST('01A09FD7-EBDD-7496-A81B-6B7705B9EA4C' AS UNIQUEIDENTIFIER), 'Exclusion Requirement', 'Participant is subject to an Exclusion Requirement which might incorporate specific zones, areas or premises', 6, 0, NULL, 16, 0),
             (CAST('01A09FD7-EBDD-7496-A81B-6B7705B9EA4D' AS UNIQUEIDENTIFIER), 'FLT Training Required', 'Forklift Truck Training course attendance required to improved employment opportunities', 5, 0, NULL, 13, 0),
             (CAST('01A09FD7-EBDD-7496-A81B-6B7705B9EA4E' AS UNIQUEIDENTIFIER), 'FLT Certified', 'Certified to operate a Forklift Truck', 4, 0, NULL, 13, 0),
             (CAST('01A09FD7-EBDD-7496-A81B-6B7705B9EA51' AS UNIQUEIDENTIFIER), 'IPP', 'Subject to Imprisonment for Public Protection (IPP)', 6, 0, NULL, 16, 0),
             (CAST('01A09FD7-EBDD-7496-A81B-6B7705B9EA52' AS UNIQUEIDENTIFIER), 'ISWS In Progress', 'Interventions and Services Wraparound Support currently in progress', 4, 0, NULL, 19, 0),
             (CAST('01A09FD7-EBDD-7496-A81B-6B7705B9EA53' AS UNIQUEIDENTIFIER), 'Job Ready', 'Suitable and prepared to take up employment opportunities', 4, 0, NULL, 12, 0),
             (CAST('01A09FD7-EBDD-7496-A81B-6B7705B9EA54' AS UNIQUEIDENTIFIER), 'Life Sentence', 'Participant is serving a life sentence', 6, 0, NULL, 16, 0),
             (CAST('01A09FD7-EBDD-7496-A81B-6B7705B9EA56' AS UNIQUEIDENTIFIER), 'PTS Required', 'Personal Track Safety course attendance required to improve employment opportunities', 5, 0, NULL, 13, 0),
             (CAST('01A09FD7-EBDD-7496-A81B-6B7705B9EA57' AS UNIQUEIDENTIFIER), 'PTS Certified', 'Successfully passed Personal Track Safety training', 4, 0, NULL, 13, 0),
             (CAST('01A09FD7-EBDD-7496-A81B-6B7705B9EA58' AS UNIQUEIDENTIFIER), 'Recall', 'Participant has experience of being recalled into prison during the lifetime of CFO Evolution', 5, 0, NULL, 7, 0),
             (CAST('01A09FD7-EBDD-7496-A81B-6B7705B9EA5A' AS UNIQUEIDENTIFIER), 'Unpaid Work requirement', 'Subject to a court-ordered Unpaid Work requirement to be completed', 5, 0, NULL, 16, 0)
  ) AS NewLabels([Id], [Name], [Description], [Colour], [Variant], [ContractId], [AppIcon], [Scope])
WHERE NOT EXISTS (
    SELECT 1
    FROM [Configuration].[Label] ExistingLabels
    WHERE ExistingLabels.Id = NewLabels.Id
);
END;