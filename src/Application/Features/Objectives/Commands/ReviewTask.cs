using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Objectives.Commands;

public class ReviewTask
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        [Description("Objective Id")]
        public required Guid TaskId { get; set; }

        [Description("Reoccurs")]
        public bool Reoccurs { get; set; }

        [Description("Due")]
        public DateTime? Due { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var task = await unitOfWork.DbContext.ObjectiveTasks.FindAsync(request.TaskId);

            if (task is null)
            {
                throw new NotFoundException("Cannot find task", request.TaskId);
            }

            task.Complete();

            if(request.Reoccurs)
            {

            }

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            var today = DateTime.UtcNow;

            RuleFor(x => x.TaskId)
                .NotNull();

            RuleFor(x => x.Due)
                .NotNull()
                .WithMessage("You must provide a Due date")
                .GreaterThanOrEqualTo(new DateTime(today.Year, today.Month, 1))
                .WithMessage(ValidationConstants.DateMustBeInFuture)
                .Must(x => x!.Value.Day.Equals(1))
                .WithMessage("Due date must fall on the first day of the month");
        }

    }

}
