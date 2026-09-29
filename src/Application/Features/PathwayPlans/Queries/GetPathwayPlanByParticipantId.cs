using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Initiatives.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Queries;

public static class GetPathwayPlanByParticipantId
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IQuery<PathwayPlanDto?>
    {
        public required string ParticipantId {  get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<Query, PathwayPlanDto?>
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
                    io.Initiative.Description,
                    io.StartDate,
                    io.EndDate,
                    // Contract.Tenant is nullable in the domain model, so this may legitimately be null
                    // (e.g. a contract not yet assigned to a tenant) — must not be dereferenced without
                    // a null check below.
                    TenantId = io.Initiative.Contract == null ? null : io.Initiative.Contract.Tenant == null ? null : io.Initiative.Contract.Tenant.Id
                })
                .ToArrayAsync(cancellationToken);

            var objectiveIdsWithActivities = await unitOfWork.DbContext.Activities
                .Where(a => a.ParticipantId == request.ParticipantId)
                .Select(a => a.ObjectiveId)
                .Distinct()
                .ToArrayAsync(cancellationToken);

            foreach (var objective in pathwayPlan.Objectives)
            {
                objective.HasActivities = objectiveIdsWithActivities.Contains(objective.Id);
            }

            if (linkedInitiatives.Length > 0)
            {
                // Compare against the case's own tenant (the participant's owner), not the acting user's
                // tenant: CMPSM/internal staff sit under a national, top-level tenant that is a prefix of
                // every regional tenant, and provider/subcontractor users can sit deeper than the contract
                // they deliver under. Comparing against the case's tenant with a bidirectional hierarchy
                // check correctly identifies whether the initiative is genuinely local to the case,
                // regardless of who is viewing it.
                var caseTenantId = await unitOfWork.DbContext.Participants
                    .Where(p => p.Id == request.ParticipantId)
                    .Select(p => p.Owner!.TenantId)
                    .FirstOrDefaultAsync(cancellationToken);

                foreach (var objective in pathwayPlan.Objectives)
                {
                    var link = linkedInitiatives.FirstOrDefault(l => l.ObjectiveId == objective.Id);
                    if (link is not null)
                    {
                        objective.LinkedInitiative = new InitiativeSummaryDto
                        {
                            Id = link.Id,
                            Code = link.Code,
                            Description = link.Description,
                            InitiativeStartDate = link.StartDate,
                            InitiativeEndDate = link.EndDate,
                            // If either tenant is unknown (null/empty), treat the initiative as NOT within
                            // the case's tenant so the safer, elevated (CMPSM+ with justification) path is
                            // required rather than silently allowing an unverified same-tenant unlink.
                            IsWithinCaseTenant = string.IsNullOrEmpty(caseTenantId) is false
                                                 && string.IsNullOrEmpty(link.TenantId) is false
                                                 && (caseTenantId.StartsWith(link.TenantId, StringComparison.Ordinal)
                                                     || link.TenantId.StartsWith(caseTenantId, StringComparison.Ordinal))
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
            
            RuleSet(ValidationConstants.RuleSet.Mediator, () =>
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