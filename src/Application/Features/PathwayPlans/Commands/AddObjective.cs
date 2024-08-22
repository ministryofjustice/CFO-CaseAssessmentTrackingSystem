using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Commands;

public static class AddObjective
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        [Description("Pathway Plan Id")]
        public required Guid PathwayPlanId { get; set; }

        public string? Title { get; set; }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Command, Objective>(MemberList.None)
                    .ConstructUsing(dto => Objective.Create(dto.Title!, dto.PathwayPlanId));
            }
        }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var objective = mapper.Map<Objective>(request);
            var pathwayPlan = await unitOfWork.DbContext.PathwayPlans.FindAsync(request.PathwayPlanId);

            if (pathwayPlan is null)
            {
                throw new NotFoundException("Cannot find pathway plan", request.PathwayPlanId);
            }

            pathwayPlan.AddObjective(objective);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.PathwayPlanId)
                .NotNull();

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("You must provide a title")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Title"));
        }

    }
}
