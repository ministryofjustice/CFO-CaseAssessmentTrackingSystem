using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.Caching;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Assessments.Queries.GetAssessment;

[RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
public class GetAssessmentQuery : ICacheableRequest<Result<AssessmentDto>>
{
    public string CacheKey 
        => AssessmentsCacheKey.GetAllCacheKey;

    public MemoryCacheEntryOptions? Options 
        => AssessmentsCacheKey.MemoryCacheEntryOptions;
}

