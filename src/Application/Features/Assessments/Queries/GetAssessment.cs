using System.Text.Json.Serialization;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.Caching;
using Cfo.Cats.Application.Features.Assessments.Commands;
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

namespace Cfo.Cats.Application.Features.Assessments.Queries;

public static class GetAssessment
{
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Query : ICacheableRequest<Result<Assessment>>
    {
        public required string ParticipantId { get; set; }
        public required Guid AssessmentId { get; set; }

        public string CacheKey
            => AssessmentsCacheKey.GetAllCacheKey;

        public MemoryCacheEntryOptions? Options
            => AssessmentsCacheKey.MemoryCacheEntryOptions;
    }

    internal class Handler : IRequestHandler<Query, Result<Assessment>>
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Assessment>> Handle(Query request, CancellationToken cancellationToken)
        {
            var pa = await _context.ParticipantAssessments.FirstOrDefaultAsync(a => a.ParticipantId == request.ParticipantId && a.Id == request.AssessmentId, cancellationToken: cancellationToken);

            if (pa is null)
            {
                throw new NotFoundException(nameof(Assessment), new
                {
                    request.AssessmentId,
                    request.ParticipantId
                });
            }

            Assessment assessment = JsonSerializer.Deserialize<Assessment>(pa.AssessmentJson)!;
            return await Result<Assessment>.SuccessAsync(assessment);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId)
                .NotNull();

            RuleFor(x => x.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9);
        }
    }
}