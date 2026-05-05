using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Initiatives.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Queries;

public static class GetPathwayPlanByParticipantId
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<PathwayPlanDto?>
    {
        public required string ParticipantId {  get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, PathwayPlanDto?>
    {
        public async Task<PathwayPlanDto?> Handle(Query request, CancellationToken cancellationToken)
        {
            var pathwayPlan = await unitOfWork.DbContext.PathwayPlans
                .Include(p => p.PathwayPlanReviews)
                .Where(p => p.ParticipantId == request.ParticipantId)
                .ProjectTo<PathwayPlanDto>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);

            if (pathwayPlan is null)
            {
                return null;
            }

            var linkedInitiatives = await unitOfWork.DbContext.InitiativeObjectives
                .Where(io => io.ParticipantId == request.ParticipantId)
                .Select(io => new
                {
                    io.ObjectiveId,
                    io.Initiative.Id,
                    io.Initiative.Code,
                    io.Initiative.Description
                })
                .ToArrayAsync(cancellationToken);

            if (linkedInitiatives.Length > 0)
            {
                foreach (var objective in pathwayPlan.Objectives)
                {
                    var link = linkedInitiatives.FirstOrDefault(l => l.ObjectiveId == objective.Id);
                    if (link is not null)
                    {
                        objective.LinkedInitiative = new InitiativeSummaryDto
                        {
                            Id = link.Id,
                            Code = link.Code,
                            Description = link.Description
                        };
                    }
                }
            }

            return pathwayPlan;
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
            RuleFor(x => x.ParticipantId)
                .NotNull();

            RuleFor(x => x.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9)
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