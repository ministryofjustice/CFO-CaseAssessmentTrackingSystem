using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Objectives.Commands;

public static class EditTask
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        [Description("Task Id")]
        public required Guid TaskId { get; set; }

        [Description("Objective Id")]
        public required Guid ObjectiveId { get; set; }

        [Description("Title")]
        public string? Title { get; set; }

        [Description("Due")]
        public DateTime? Due { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var objective = await unitOfWork.DbContext.Objectives.FindAsync(request.ObjectiveId);

            if (objective is null)
            {
                throw new NotFoundException("Cannot find objective", request.ObjectiveId);
            }

            var task = objective.Tasks.FirstOrDefault(x => x.Id == request.TaskId);

            if (task is null)
            {
                throw new NotFoundException("Cannot find task", request.TaskId);
            }

            if(request.Title is not null)
            {
                task.Rename(request.Title);
            }

            if(request.Due.HasValue)
            {
                task.Extend(request.Due.Value);
            }

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

            RuleFor(x => x.TaskId)
                .NotNull();

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("You must provide a title")
                .Matches(ValidationConstants.AlphabetsDigitsSpaceSlashHyphenDot)
                .WithMessage(string.Format(ValidationConstants.AlphabetsDigitsSpaceSlashHyphenDotMessage, "Title"));

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
