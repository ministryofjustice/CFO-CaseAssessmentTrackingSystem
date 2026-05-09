using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class MarkEnrolmentFeedbackAsRead
{
    [RequestAuthorize(Policy = SecurityPolicies.Qa1)]
    public class Command : IRequest<Result>
    {
        public Guid EnrolmentFeedbackId { get; init; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var feedback = await unitOfWork.DbContext.EnrolmentFeedbacks
                .FirstOrDefaultAsync(x => x.Id == request.EnrolmentFeedbackId, cancellationToken);

            if (feedback == null)
            {
                return Result.Failure("Enrolment feedback not found");
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
                RuleFor(x => x.EnrolmentFeedbackId)
                    .Must(id => _unitOfWork.DbContext.EnrolmentFeedbacks.Any(f => f.Id == id))
                    .WithMessage("Enrolment feedback does not exist");
            });
        }
    }
}