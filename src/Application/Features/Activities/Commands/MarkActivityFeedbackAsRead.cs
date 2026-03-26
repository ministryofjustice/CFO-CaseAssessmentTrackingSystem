using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Activities.Commands;

public static class MarkActivityFeedbackAsRead
{
    [RequestAuthorize(Policy = SecurityPolicies.Qa1)]
    public class Command : IRequest<Result>
    {
        public Guid ActivityFeedbackId { get; init; }
    }
    
    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var feedback = await unitOfWork.DbContext.ActivityFeedbacks
                .FirstOrDefaultAsync(x => x.Id == request.ActivityFeedbackId, cancellationToken);

            if (feedback == null)
            {
                return Result.Failure("Activity feedback not found");
            }

            feedback.MarkAsRead();

            return Result.Success();
        }
    }
    
    public class A_FeedbackMustExist : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public A_FeedbackMustExist(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(x => x.ActivityFeedbackId)
                    .Must(id => _unitOfWork.DbContext.ActivityFeedbacks.Any(f => f.Id == id))
                    .WithMessage("Activity feedback does not exist");
            });
        }
    }
}