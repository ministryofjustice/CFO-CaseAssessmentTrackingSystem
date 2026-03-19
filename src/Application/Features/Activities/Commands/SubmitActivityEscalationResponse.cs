using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Commands;

public static class SubmitActivityEscalationResponse
{
    [RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
    public class Command : IRequest<Result>
    {
        public required Guid ActivityQueueEntryId { get; init; }

        public EscalationResponse? Response { get; set; }
        public FeedbackType? FeedbackType { get; set; }        

        public string Message { get; set; } = null!;
        public string MessageToProvider { get; set; } = null!;
        public string MessageToQa1 { get; set; } = null!;
        public UserProfile? CurrentUser { get; init; }
    }

    public enum EscalationResponse
    {
        Accept = 0,
        Return = 1,
        Comment = 2
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var entry = await unitOfWork.DbContext.ActivityEscalationQueue
                .Include(pqa => pqa.Participant)
                .Include(x => x.Activity)
                .FirstOrDefaultAsync(x => x.Id == request.ActivityQueueEntryId, cancellationToken: cancellationToken);
               
            if (entry == null)
            {
                return Result.Failure("Cannot find activity escalation queue item");
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
                case EscalationResponse.Accept:
                    entry.Accept();
                    break;
                case EscalationResponse.Return:
                    entry.Return();
                    break;
                case EscalationResponse.Comment:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!string.IsNullOrWhiteSpace(request.MessageToQa1))
            {
                var outcome = response switch
                {
                    EscalationResponse.Accept => FeedbackOutcome.Approved,
                    EscalationResponse.Return => FeedbackOutcome.Returned,
                    EscalationResponse.Comment => FeedbackOutcome.EscalatedComment,
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                var activityFeedback = ActivityFeedback.Create(
                    entry.ActivityId,
                    entry.ParticipantId!,
                    entry.CreatedBy!,
                    request.MessageToQa1,
                    outcome,
                    FeedbackStage.Escalation,
                    request.CurrentUser!.UserId,
                    entry.TenantId
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
            
            RuleFor(x => x.MessageToProvider)
                .MaximumLength(ValidationConstants.NotesLength);

            // Accept response: FeedbackType is required
            When(x => x.Response == EscalationResponse.Accept, () =>
            {
                RuleFor(x => x.FeedbackType)
                    .NotNull()
                    .WithMessage("You must select a feedback type when accepting")
                    .Must(ft => ft != FeedbackType.Returned)
                    .WithMessage("FeedbackType cannot be 'Returned' when accepting");
            });

            // Return response: FeedbackType must be Returned and MessageToProvider is mandatory
            When(x => x.Response == EscalationResponse.Return, () =>
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

            // Accept response: MessageToProvider required if FeedbackType is Advisory or AcceptedByException
            When(x => x.Response == EscalationResponse.Accept && (x.FeedbackType == FeedbackType.Advisory || x.FeedbackType == FeedbackType.AcceptedByException), () =>
            {
                RuleFor(x => x.MessageToProvider)
                    .NotEmpty()
                    .WithMessage("External Message is required for Advisory or Accepted By Exception")
                    .Matches(ValidationConstants.Notes)
                    .WithMessage(string.Format(ValidationConstants.NotesMessage, "External Message"));
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
                    .WithMessage("Activity Queue item does not exist");
            });
        }
        private async Task<bool> MustExist(Guid identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.ActivityEscalationQueue.AnyAsync(e => e.Id == identifier, cancellationToken);
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
                    .WithMessage("Activity Queue item is already completed.");
            });
        }

        private async Task<bool> MustBeOpen(Guid id, CancellationToken cancellationToken)
        {
            var entry = await _unitOfWork.DbContext.ActivityEscalationQueue
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken: cancellationToken);

            return entry is { IsCompleted: false };
        }
    }

    public class D_ShouldNotBeAtPqaStatus : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public D_ShouldNotBeAtPqaStatus(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ActivityQueueEntryId)
                    .MustAsync(MustBeAtSubmittedToAuthority)
                    .WithMessage("Activity Queue item is not at Submitted to Authority stage");
            });
        }

        private async Task<bool> MustBeAtSubmittedToAuthority(Guid id, CancellationToken cancellationToken)
        {
            var entry = await _unitOfWork.DbContext.ActivityEscalationQueue.Include(c => c.Activity)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken: cancellationToken);

            return entry != null && entry.Activity!.Status== ActivityStatus.SubmittedToAuthorityStatus;
        }
    }
}