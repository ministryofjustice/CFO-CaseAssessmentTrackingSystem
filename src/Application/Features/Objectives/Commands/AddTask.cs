
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Objectives.Commands;

public static class AddTask
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
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
            var objective = await unitOfWork.DbContext.Objectives.FindAsync(request.ObjectiveId);

            if (objective is null)
            {
                throw new NotFoundException("Cannot find objective", request.ObjectiveId);
            }

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

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("You must provide a title")
                .Matches(ValidationConstants.AlphabetsDigitsSpaceSlashHyphenDot)
                .WithMessage(string.Format(ValidationConstants.AlphabetsDigitsSpaceSlashHyphenDotMessage, "Title"));

            RuleFor(x => x.Due)
                .NotNull()
                .WithMessage("You must provide a Due date")
                .GreaterThanOrEqualTo(new DateTime(today.Year, today.Month, 1))
                .WithMessage(ValidationConstants.DateMustBeInFuture);
        }

    }

}
