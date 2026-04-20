using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Commands;

public static class SubmitActivityQa2Response
{
    [RequestAuthorize(Policy = SecurityPolicies.Qa2)]
    public class Command : IRequest<Result>
    {
        public required Guid ActivityQueueEntryId { get; init; }

        public Qa2Response? Response { get; set; }
        
        [Description("Feedback Type")]
        public FeedbackType? FeedbackType { get; set; }

        public string Message { get; set; } = null!;

        [Description("Message to provider")]
        public string MessageToProvider { get; set; } = null!;

        [Description("Feedback message to Qa1")]
        public string? MessageToQa1 { get; set; } 
        
        public UserProfile? CurrentUser { get; init; }
        
        [Description("Activity Feedback Reason")]
        public string? ActivityFeedbackReason { get; set; }
    }

    public enum Qa2Response
    {
        Accept = 0,
        Return = 1,
        Escalate = 2
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
             var entry = await unitOfWork.DbContext.ActivityQa2Queue
                .Include(x => x.Participant)
                .Include(x => x.Activity)
                .FirstOrDefaultAsync(x => x.Id == request.ActivityQueueEntryId, cancellationToken);
            
            if (entry == null)
            {
                return Result.Failure("Cannot find activity Qa2 queue item");
            }

            if (!string.IsNullOrWhiteSpace(request.Message))
            {
                entry.AddNote(request.Message, isExternal: false);
            }

            if (!string.IsNullOrWhiteSpace(request.MessageToProvider))
            {
                entry.AddNote(request.MessageToProvider, isExternal: true, feedbackType: request.FeedbackType);
            }
            
            var response = request.Response!.Value;

            switch (response)
            {
                case Qa2Response.Accept:
                    entry.Accept();
                    break;
                case Qa2Response.Return:
                    entry.Return();
                    break;
                case Qa2Response.Escalate:
                    entry.Escalate();
                    break;
                default: 
                    throw new ArgumentOutOfRangeException();
            }
            
            if (!string.IsNullOrWhiteSpace(request.MessageToQa1))
            {
                var outcome = response switch
                {
                    Qa2Response.Accept => FeedbackOutcome.Approved,
                    Qa2Response.Return => FeedbackOutcome.Returned,
                    Qa2Response.Escalate => FeedbackOutcome.Escalated,
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                var activityFeedback = ActivityFeedback.Create(
                    entry.ActivityId,
                    entry.ParticipantId!,
                    entry.CreatedBy!,
                    request.MessageToQa1,
                    outcome,
                    FeedbackStage.Qa2,
                    entry.Created!.Value,
                    request.CurrentUser!.UserId,
                    entry.TenantId,
                    entry.Activity!.Category.Name,
                    entry.Activity.Type.Name,
                    request.ActivityFeedbackReason!
                );

                unitOfWork.DbContext.ActivityFeedbacks.Add(activityFeedback);
            }
           
            return Result.Success();
        }
    }

    public class A_IsValidRequest : AbstractValidator<Command>
    {
        public A_IsValidRequest()
        {
            RuleFor(x => x.Response)
                .NotNull()
                .WithMessage("You must select a response");

            RuleFor(x => x.Message)
                .MaximumLength(ValidationConstants.NotesLength);
            
            When(x => x.Response is Qa2Response.Escalate, () => {
                RuleFor(x => x.Message)
                    .NotEmpty()
                    .WithMessage("Internal Message is required when escalating")
                    .Matches(ValidationConstants.Notes)
                    .WithMessage(string.Format(ValidationConstants.NotesMessage, "Message"));
            });                
            
            RuleFor(x => x.MessageToProvider)
                .MaximumLength(ValidationConstants.NotesLength);

            When(x => x.Response == Qa2Response.Accept, () =>
            {
                RuleFor(x => x.FeedbackType)
                    .NotNull()
                    .WithMessage("You must select a feedback type when accepting")
                    .Must(ft => ft != FeedbackType.Returned)
                    .WithMessage("FeedbackType cannot be 'Returned' when accepting");
            });

            When(x => x.Response == Qa2Response.Return, () =>
            {
                RuleFor(x => x.FeedbackType)
                    .Equal(FeedbackType.Returned)
                    .WithMessage("FeedbackType must be 'Returned' when returning");
                
                RuleFor(x => x.MessageToProvider)
                    .NotEmpty()
                    .WithMessage("External Message is required when returning")
                    .Matches(ValidationConstants.Notes)
                    .WithMessage(string.Format(ValidationConstants.NotesMessage, "External Message"));
            });

            When(x => x.Response == Qa2Response.Accept && (x.FeedbackType == FeedbackType.Advisory || x.FeedbackType == FeedbackType.AcceptedByException), () =>
            {
                RuleFor(x => x.MessageToProvider)
                    .NotEmpty()
                    .WithMessage("External Message is required for Advisory or Accepted By Exception")
                    .Matches(ValidationConstants.Notes)
                    .WithMessage(string.Format(ValidationConstants.NotesMessage, "External Message"));
            });
            
            RuleFor(x => x.MessageToQa1)
                .MaximumLength(ValidationConstants.NotesLength);
            
            When(x => !string.IsNullOrWhiteSpace(x.MessageToQa1), () =>
            {
                RuleFor(x => x.ActivityFeedbackReason)
                    .NotEmpty()
                    .WithMessage("You must select a feedback reason when providing a QA1 message");
            });
        }
    }

    public class B_EntryMustExist : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public B_EntryMustExist(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ActivityQueueEntryId)
                    .MustAsync(MustExist)
                    .WithMessage("Queue item does not exist");
            });
        }

        private async Task<bool> MustExist(Guid identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.ActivityQa2Queue.AnyAsync(e => e.Id == identifier, cancellationToken);
    }

    public class C_ShouldNotBeComplete : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public C_ShouldNotBeComplete(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ActivityQueueEntryId)
                    .MustAsync(MustBeOpen)
                    .WithMessage("Queue item is already completed.");
            });
        }

        private async Task<bool> MustBeOpen(Guid id, CancellationToken cancellationToken)
        {
            var entry = await _unitOfWork.DbContext.ActivityQa2Queue
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken: cancellationToken);

            return entry is { IsCompleted: false };
        }
    }

    public class D_ShouldBeAtSubmittedToAuthorityStatus : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public D_ShouldBeAtSubmittedToAuthorityStatus(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ActivityQueueEntryId)
                    .MustAsync(MustBeAtQa)
                    .WithMessage("Queue item is not a PQA stage");
            });
        }

        private async Task<bool> MustBeAtQa(Guid id, CancellationToken cancellationToken)
        {
            var entry = await _unitOfWork.DbContext.ActivityQa2Queue.Include(c => c.Activity)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken: cancellationToken);

            return entry != null && entry.Activity!.Status== ActivityStatus.SubmittedToAuthorityStatus;
        }
    }

    public class E_OwnerShouldNotBeApprover : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public E_OwnerShouldNotBeApprover(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c)
                    .MustAsync(OwnerMustNotBeApprover)
                    .WithMessage("This Activity assessment is created by you hence must not be processed at QA2 stage by you");
            });
        }

        private async Task<bool> OwnerMustNotBeApprover(Command c, CancellationToken cancellationToken)
        {
            var entry = await _unitOfWork.DbContext.ActivityQa2Queue.Include(c => c.Activity)
                .FirstOrDefaultAsync(a => a.Id == c.ActivityQueueEntryId, cancellationToken: cancellationToken);

            return entry != null && entry.Activity!.OwnerId!.Equals(c.CurrentUser!.UserId) == false;
        }
    }
}