using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;

[RequestAuthorize(Policy = SecurityPolicies.ParticipantAdvancedSearch)]
public class ParticipantAdvancedSearchQuery : IQuery<Result<ParticipantAdvancedSearchResult>>
{
    /// <summary>
    /// A CATS Id, NOMIS number or NDelius CRN.
    /// </summary>
    public string? Identifier { get; set; }

    /// <summary>
    /// Optional. Required together with <see cref="DateOfBirth"/> to search DMS.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Optional. Required together with <see cref="LastName"/> to search DMS.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    public required UserProfile CurrentUser { get; init; }
}
