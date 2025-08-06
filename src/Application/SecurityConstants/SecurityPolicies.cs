namespace Cfo.Cats.Application.SecurityConstants;

public static class SecurityPolicies
{
    /// <summary>
    /// The user is permitted to export data from the system.
    /// </summary>
    public const string Export = nameof(Export);
    
    /// <summary>
    /// The user is permitted to search for candidates.
    /// </summary>
    public const string CandidateSearch = nameof(CandidateSearch);

    public const string Pqa= nameof(Pqa);

    public const string Qa1 = nameof(Qa1);

    public const string Qa2 = nameof(Qa2);

    public const string Internal = nameof(Internal);

    public const string SeniorInternal = nameof(SeniorInternal);
    
    /// <summary>
    /// The user is allowed to upload files.
    /// </summary>
    public const string DocumentUpload = nameof(DocumentUpload);
    
    /// <summary>
    /// Any authorized user is permitted to perform this action
    /// </summary>
    public const string AuthorizedUser = nameof(AuthorizedUser);

    /// <summary>
    /// The user is permitted to import data into the system.
    /// </summary>
    public const string Import = nameof(Import);

    public const string Enrol = nameof(Enrol);

    /// <summary>
    /// 
    /// </summary>
    public const string SystemFunctionsRead = nameof(SystemFunctionsRead);
    
    public const string SystemFunctionsWrite = nameof(SystemFunctionsWrite);

    /// <summary>
    /// The used anywhere any user > support worker can do it
    /// </summary>
    public const string UserHasAdditionalRoles = nameof(UserHasAdditionalRoles);

    /// <summary>
    /// Used to allow users to view the audits
    /// </summary>
    public const string ViewAudit = nameof(ViewAudit);

    public const string SystemSupportFunctions = nameof(SystemSupportFunctions);
    
    public const string OutcomeQualityDipChecks = nameof(OutcomeQualityDipChecks);

    public const string OutcomeQualityDipReview = nameof(OutcomeQualityDipReview);

    public const string OutcomeQualityDipVerification = nameof(OutcomeQualityDipVerification);

}
