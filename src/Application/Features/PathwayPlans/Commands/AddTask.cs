using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Commands;

public static class AddTask
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        [Description("Pathway Plan Id")]
        public required Guid PathwayPlanId { get; set; }

        [Description("Objective Id")]
        public required Guid ObjectiveId { get; set; }

        public string? Title { get; set; }

        public DateTime? Due { get; set; }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Command, ObjectiveTask>(MemberList.None)
                    .ConstructUsing(dto => ObjectiveTask.Create(dto.Title!, dto.Due!.Value));
            }
        }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var pathwayPlan = await unitOfWork.DbContext.PathwayPlans.FindAsync(request.PathwayPlanId)
                ?? throw new NotFoundException("Cannot find pathway plan", request.PathwayPlanId);

            var objective = pathwayPlan.Objectives.FirstOrDefault(o => o.Id == request.ObjectiveId)
                ?? throw new NotFoundException("Cannot find objective", request.ObjectiveId);

            var task = mapper.Map<ObjectiveTask>(request);

            objective.AddTask(task);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            var today = DateTime.UtcNow;

            RuleFor(x => x.ObjectiveId)
                .NotNull();

            RuleFor(x => x.PathwayPlanId)
                .NotNull();

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("You must provide a title")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Title"));

            RuleFor(x => x.Due)
                .Must(x => x.HasValue)
                .WithMessage("You must provide a Due date");

            When(x => x.Due.HasValue, () =>
            {
                RuleFor(x => x.Due)
                    .GreaterThanOrEqualTo(new DateTime(today.Year, today.Month, 1))
                    .WithMessage(ValidationConstants.DateMustBeInFuture)
                    .Must(x => x!.Value.Day.Equals(1))
                    .WithMessage("Due date must fall on the first day of the month");
            });
        }

    }

}
