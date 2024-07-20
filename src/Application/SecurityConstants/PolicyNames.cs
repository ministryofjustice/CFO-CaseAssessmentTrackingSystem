namespace Cfo.Cats.Application.SecurityConstants;

public static class PolicyNames
{
    /// <summary>
    /// The user is permitted to export data from the system.
    /// </summary>
    public const string AllowExport = nameof(AllowExport);
    
    /// <summary>
    /// The user is permitted to search for candidates.
    /// </summary>
    public const string AllowCandidateSearch = nameof(AllowCandidateSearch);
    
    /// <summary>
    /// The user is allowed to upload files.
    /// </summary>
    public const string AllowDocumentUpload = nameof(AllowDocumentUpload);
    
    /// <summary>
    /// Any authorized user is permitted to perform this action
    /// </summary>
    public const string AuthorizedUser = nameof(AuthorizedUser);

    /// <summary>
    /// The user is permitted to import data into the system.
    /// </summary>
    public const string AllowImport = nameof(AllowExport);

    public const string AllowEnrol = nameof(AllowEnrol);

    /// <summary>
    /// 
    /// </summary>
    public const string SystemFunctionsRead = nameof(SystemFunctionsRead);
    
    public const string SystemFunctionsWrite = nameof(SystemFunctionsWrite);

    public const string CanSubmitToQA = nameof(CanSubmitToQA);
}
