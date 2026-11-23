using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Offloc.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Offloc.Queries.GetSentenceDataForAdvancedSearch;

[RequestAuthorize(Policy = SecurityPolicies.ParticipantAdvancedSearch)]
public class GetSentenceDataForAdvancedSearchQuery : IQuery<Result<SentenceDataDto>>
{
    public string NomisNumber { get; set; } = string.Empty;
    public string RecordIdentifier { get; set; } = string.Empty;
    public string Justification { get; set; } = string.Empty;
    public required UserProfile CurrentUser { get; init; }
}
