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

namespace Cfo.Cats.Application.Features.Assessments.Commands;

public static class BeginAssessment
{
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Command : ICacheInvalidatorRequest<Result<Assessment>>
    {
        public required string ParticipantId { get; set; }
        
        //TODO: this could be done at a per participant level
        public string CacheKey => AssessmentsCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource 
            => AssessmentsCacheKey.SharedExpiryTokenSource();
    }

    public class Handler : IRequestHandler<Command, Result<Assessment>>
    {
        public Task<Result<Assessment>> Handle(Command request, CancellationToken cancellationToken)
        {
            Assessment assessment = new Assessment()
            {
                ParticipantId = request.ParticipantId,
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
            // Todo: store in the database
            return Result<Assessment>.SuccessAsync(assessment);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9);
        }
    }

}
