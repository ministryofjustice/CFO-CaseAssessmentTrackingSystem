using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.Caching;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Assessments.Queries.GetAssessment;

public static class GetAssessment
{
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Query : ICacheableRequest<Result<Assessment>>
    {
        public string CacheKey 
            => AssessmentsCacheKey.GetAllCacheKey;

        public MemoryCacheEntryOptions? Options 
            => AssessmentsCacheKey.MemoryCacheEntryOptions;
    
        public required string Upci { get; set; }
    }
    
    internal class Handler : IRequestHandler<Query, Result<Assessment>>
    {

        public Task<Result<Assessment>> Handle(Query request, CancellationToken cancellationToken)
        {
            Assessment a = new Assessment()
            {
                Pathways =
                [
                    new WorkingPathway(),
                    new HousingPathway(),
                ]
            };
            return Result<Assessment>.SuccessAsync(a);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Upci)
                .NotNull();

            RuleFor(x => x.Upci)
                .MinimumLength(9)
                .MaximumLength(9);
        }
    }

}


