using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantSummary
{
    [RequestAuthorize(Policy = SecurityPolicies.CandidateSearch)]
    public class Query : IAuditableRequest<Result<ParticipantSummaryDto>>
    {
        public required string ParticipantId { get; set; } 
        public required UserProfile CurrentUser { get; set; }

        public string Identifier() => ParticipantId;
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper, IRightToWorkSettings rtwSettings) : IRequestHandler<Query, Result<ParticipantSummaryDto>>
    {
        public async Task<Result<ParticipantSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = from c in unitOfWork.DbContext.Participants
                where c.Id == request.ParticipantId
                select c;

            var summary = await query.ProjectTo<ParticipantSummaryDto>(mapper.ConfigurationProvider)
                .AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken);

            if (summary == null)
            {
                throw new NotFoundException(nameof(ParticipantSummaryDto), request.ParticipantId);
            }

            summary.Assessments = await unitOfWork.DbContext.ParticipantAssessments
                .Where(pa => pa.ParticipantId == request.ParticipantId)
                .ProjectTo<AssessmentSummaryDto>(mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);

            summary.LatestRisk = await unitOfWork.DbContext.Risks
                .OrderByDescending(x => x.Created)
                .ProjectTo<RiskSummaryDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.ParticipantId == request.ParticipantId, cancellationToken);

            summary.PathwayPlan = await unitOfWork.DbContext.PathwayPlans
                .IgnoreAutoIncludes()
                .Include(x => x.ReviewHistories)
                .Where(x => x.ParticipantId == request.ParticipantId)
                .ProjectTo<PathwayPlanSummaryDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            var bio = await unitOfWork.DbContext.ParticipantBios
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync(x => x.ParticipantId == request.ParticipantId, cancellationToken);

            summary.BioSummary = mapper.Map<BioSummaryDto>(bio);

            summary.HasActiveRightToWork = await unitOfWork.DbContext.Participants
                .Where(x => x.Id == request.ParticipantId)
                .SelectMany(p => p.RightToWorks)
                .AnyAsync(x => x.Lifetime.EndDate >= DateTime.Now.Date, cancellationToken);

            summary.IsRightToWorkRequired = rtwSettings.NationalitiesExempted.Any(s => s.Equals(summary.Nationality!, StringComparison.OrdinalIgnoreCase)) == false;

            summary.LatestPri = await unitOfWork.DbContext.PRIs
                .OrderByDescending(x => x.Created)
                .ProjectTo<PriSummaryDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.ParticipantId == request.ParticipantId, cancellationToken);

            if (summary.LatestPri is not null)
            {
                summary.LatestPri.ObjectiveTasks = await unitOfWork.DbContext.PathwayPlans
                        .AsNoTracking()
                        .SelectMany(p => p.Objectives)
                        .SelectMany(o => o.Tasks)
                        .Where(t => t.ObjectiveId == summary.LatestPri.ObjectiveId)
                        .ProjectTo<ObjectiveTaskDto>(mapper.ConfigurationProvider)
                        .ToArrayAsync(cancellationToken);
            }
            return Result<ParticipantSummaryDto>.Success(summary);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .Length(9)
                .WithMessage("Invalid Participant Id")
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));        

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ParticipantId)
                    .MustAsync(Exist)
                    .WithMessage("Participant does not exist");
            });
        }

        private async Task<bool> Exist(string identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);
    }
}