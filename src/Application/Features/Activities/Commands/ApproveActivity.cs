using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

//This not required as handled by approved activity after QA2 or Escalation approval 
public static class ApproveActivity
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : IRequest<Result>
    {
        public Guid? ActivityId { get; set; }

        [Description("Completed By")]
        public required string? CompletedBy { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = await unitOfWork.DbContext.Activities
                 .Include(a => a.Owner)
                 .SingleAsync(a => a.Id == request.ActivityId, cancellationToken);

            if (activity == null)
            {
                return Result.Failure("Activity not found");
            }

            activity.Approve(request.CompletedBy);                        

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
         {
             When(a => a.ActivityId is not null, () =>
             {
                 RuleFor(a => a.ActivityId)
                 .Must(MustExist)
                 .WithMessage("Activity does not exist");

                 RuleFor(a => a.ActivityId)
                     .Must(BeInPendingStatus)
                     .WithMessage("Activity mut be pending");

                 RuleFor(a => a.ActivityId)
                 .Must(ParticipantMustNotBeArchived)
                 .WithMessage("Participant is archived");

             });
         });
        }

        private bool MustExist(Guid? identifier)
            => unitOfWork.DbContext.Activities.Any(e => e.Id == identifier);

        private bool BeInPendingStatus(Guid? activityId)
        {
            var activity = unitOfWork.DbContext.Activities.Single(a => a.Id == activityId);
            return activity.Status == ActivityStatus.PendingStatus;
        }

        private bool ParticipantMustNotBeArchived(Guid? activityId)
        {
            var entry = unitOfWork.DbContext.Activities.Include(c => c.Participant)
                .FirstOrDefault(a => a.Id == activityId);

            return entry != null && entry.Participant!.EnrolmentStatus != EnrolmentStatus.ArchivedStatus;
        }
    }
}