using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Objectives.Commands;

public static class ReviewObjective
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        [Description("Objective Id")]
        public required Guid ObjectiveId { get; set; }

        public CompletionStatus Reason { get; set; } = CompletionStatus.Done;
        
        public string? Justification { get; set; }
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

            objective.Review(request.Reason, request.Justification);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ObjectiveId)
                .NotNull();

            When(x => x.Reason.RequiresJustification, () =>
            {
                RuleFor(x => x.Justification)
                    .NotEmpty()
                    .WithMessage("Justification is required for the selected reason");
            });
        }

    }
}
