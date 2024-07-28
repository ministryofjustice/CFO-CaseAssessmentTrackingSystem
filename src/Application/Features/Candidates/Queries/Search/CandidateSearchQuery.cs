using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Candidates.Caching;
using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Candidates.Queries.Search;

[RequestAuthorize(Policy = SecurityPolicies.CandidateSearch)]
public class CandidateSearchQuery : ICacheableRequest<IEnumerable<CandidateDto>>
{
    public required string ExternalIdentifier { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; } 
    public required DateTime? DateOfBirth { get; set; } 
    public required UserProfile CurrentUser { get; set; }
    
    public string CacheKey => CandidatesCacheKey.GetCandidateCacheKey($"{this}");

    public MemoryCacheEntryOptions? Options => CandidatesCacheKey.MemoryCacheEntryOptions;

    public override string ToString() 
        => $"Id:{ExternalIdentifier},U:{CurrentUser!.UserId},FN:{FirstName},LN{LastName},DOB:{DateOfBirth}";
}