using Cfo.Cats.Application.Features.Assessments.Caching;
using Cfo.Cats.Application.Features.Assessments.DTOs;

namespace Cfo.Cats.Application.Features.Assessments.Queries.GetAssessment;


public class GetAssessmentQuery : ICacheableRequest<Result<AssessmentDto>>
{
    public string CacheKey 
        => AssessmentsCacheKey.GetAllCacheKey;

    public MemoryCacheEntryOptions? Options 
        => AssessmentsCacheKey.MemoryCacheEntryOptions;
}

