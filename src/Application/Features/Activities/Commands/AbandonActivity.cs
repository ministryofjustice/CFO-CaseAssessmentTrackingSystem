using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Activities.Commands;

public static class AbandonActivity
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]

    public class Command : IRequest<Result>
    {
        public Guid? ActivityId { get; set; }

        [Description("Reason Abandoned")]
        public required ActivityAbandonReason AbandonReason { get; set; } = ActivityAbandonReason.Other;

        [Description("Abandoned Justification")]
        public string? AbandonJustification { get; set; }

        [Description("Abandoned By")]
        public required string AbandonedBy { get; set; }
    }

    private class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = await unitOfWork.DbContext.Activities
                .SingleOrDefaultAsync(a => a.Id == request.ActivityId
                && a.Status == ActivityStatus.PendingStatus, cancellationToken);

            activity!.Abandon(request.AbandonReason, request.AbandonJustification, request.AbandonedBy);

            return Result.Success();
        }
    }

    public class A_ActivityMustExist : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public A_ActivityMustExist(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(a => a.ActivityId)
                    .Must(MustExist)
                    .WithMessage("Activity does not exist");
            });
        }

        private bool MustExist(Guid? identifier)
            => _unitOfWork.DbContext.Activities.Any(e => e.Id == identifier);
    }

    public class B_ActivityMustBePending : AbstractValidator<Command>
    {
        private readonly IUnitOfWork unitOfWork;

        public B_ActivityMustBePending(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                When(c => c.ActivityId is not null, () =>
                {
                    RuleFor(c => c.ActivityId)
                        .Must(BeInPendingStatus)
                        .WithMessage("Activity mut be pending");
                });
            });
        }

        private bool BeInPendingStatus(Guid? activityId)
        {
            var activity = unitOfWork.DbContext.Activities.Single(a => a.Id == activityId);
            return activity.Status == ActivityStatus.PendingStatus;
        }
    }

    public class C_ParticipantMustBeActive : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public C_ParticipantMustBeActive(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ActivityId)
                    .Must(MustNotBeArchived)
                    .WithMessage("Participant is archived"); ;
            });
        }

        private bool MustNotBeArchived(Guid? activityId)
                => _unitOfWork.DbContext.Activities
                    .Where(a => a.Id == activityId)
                    .Join(
                        _unitOfWork.DbContext.Participants,
                        activity => activity.ParticipantId,
                        participant => participant.Id,
                        (activity, participant) => new { activity, participant }
                    )
                    .Any(ap => ap.participant.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value);
    }
}