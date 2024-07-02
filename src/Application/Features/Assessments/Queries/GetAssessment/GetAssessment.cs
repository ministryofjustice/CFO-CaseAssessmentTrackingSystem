using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.Caching;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Education;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Money;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.ThoughtsAndBehaviours;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.WellbeingAndMentalHealth;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Assessments.Queries.GetAssessment;

public static class GetAssessment
{
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Query : ICacheableRequest<Result<Assessment>>
    {
        public required string Upci { get; set; }

        public string CacheKey
            => AssessmentsCacheKey.GetAllCacheKey;

        public MemoryCacheEntryOptions? Options
            => AssessmentsCacheKey.MemoryCacheEntryOptions;
    }

    internal class Handler : IRequestHandler<Query, Result<Assessment>>
    {
        public Task<Result<Assessment>> Handle(Query request, CancellationToken cancellationToken)
        {
            var a = new Assessment
            {
                Pathways =
                [
                    new WorkingPathway(),
                    new HousingPathway(),
                    new MoneyPathway(),
                    new EducationPathway(),
                    new HealthAndAddictionPathway(),
                    new RelationshipsPathway(),
                    new ThoughtsAndBehavioursPathway(),
                    new WellbeingAndMentalHealthPathway(),
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