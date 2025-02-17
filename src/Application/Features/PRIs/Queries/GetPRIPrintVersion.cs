using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.Features.PRIs.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;
using static Cfo.Cats.Application.Features.PRIs.Commands.AddPRI;


namespace Cfo.Cats.Application.Features.PRIs.Queries;

public class GetPRIPrintVersion
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<PRIPrintVersionDto>>
    {
        public required Guid Id { get; set; }
    }
    class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<PRIPrintVersionDto>>
    {
        public async Task<Result<PRIPrintVersionDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            PRIPrintVersionDto priPrintVersionDto = new();
            await Task.CompletedTask;

            var pri = await unitOfWork.DbContext.PRIs
                .Include(x => x.ExpectedReleaseRegion)
                .Include(x => x.CustodyLocation)
                .Include(x => x.Participant)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (pri is null)
            {
                return Result<PRIPrintVersionDto>.Failure(["Pri not found."]);
            }
            else
            {
                priPrintVersionDto = mapper.Map<PRIPrintVersionDto>(pri);
                priPrintVersionDto.PriDto = mapper.Map<PRIDto>(pri);

                //Future proofing, in case there are multiple Pathway plans per Participant
                var mandatoryObjective = await unitOfWork.DbContext.PathwayPlans
                    .Where(p => p.ParticipantId == pri.ParticipantId)
                    .Select(p => p.Objectives.Single(o => o.Id == pri.ObjectiveId))
                    .AsNoTracking()
                    .FirstAsync(cancellationToken);
                if (mandatoryObjective is not null)
                {
                    //Using mandatoryObjective to retrieve the Pathway plan which is assicated with the PRI,
                    //includes Objectives which created after the PRI is created
                    //filters out first 2 mandatory Tasks of the mandatory Objective
                    var priRelevantPathwayPlan = await unitOfWork.DbContext.PathwayPlans
                        .Include(p => p.Objectives.Where(o => o.Created >= pri.Created))
                            .ThenInclude(o => o.Tasks.Where(t => (t.ObjectiveId == mandatoryObjective.Id && t.Index > 2) || t.ObjectiveId != mandatoryObjective.Id))
                        .AsNoTracking()
                        .FirstAsync(p => p.Id == mandatoryObjective.PathwayPlanId, cancellationToken);

                    if (priRelevantPathwayPlan is not null)
                    {
                        priPrintVersionDto.PathwayPlanDto = mapper.Map<PathwayPlanDto>(priRelevantPathwayPlan);
                    }
                }
            }

            return mapper.Map<PRIPrintVersionDto>(priPrintVersionDto);
        }
    }
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(ValidationConstants.AlphaNumericMessage);

        }
    }
}
