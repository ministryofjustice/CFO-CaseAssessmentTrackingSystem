using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.ManagementInformation;

namespace Cfo.Cats.Application.Features.QualityAssurance.Commands;

public static class SubmitEscalationResponse
{
    [RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
    public class Command : IRequest<Result>
    {
        public required Guid QueueEntryId { get; init; }
        
        public EscalationResponse? Response { get; set; }
        
        [Description("Feedback Type")]
        public FeedbackType? FeedbackType { get; set; } 

        public string Message { get; set; } = null!;
        
        [Description("Message to provider")]
        public string MessageToProvider { get; set; } = null!;

        public UserProfile? CurrentUser { get; init; }
        
        [Description("Feedback message to Qa1")]
        public string MessageToQa1 { get; set; } = null!;
        
        [Description("Enrolment Feedback Reason")]
        public string EnrolmentFeedbackReason { get; set; } = null!;
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
            var entry = await unitOfWork.DbContext.EnrolmentEscalationQueue
                .Include(pqa => pqa.Participant)
                .FirstOrDefaultAsync(x => x.Id == request.QueueEntryId, cancellationToken: cancellationToken);

            if (entry == null)
            {
                return Result.Failure("Cannot find enrolment escalation queue item");
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
                    _ => throw new ArgumentOutOfRangeException($"Cannot generate feedback for {response}")
                };

                // get most recent QA1 for this enrolment
                var qa1 = await unitOfWork.DbContext
                    .EnrolmentQa1Queue
                    .Where(x => x.ParticipantId == entry.ParticipantId)
                    .Where(x => x.IsCompleted)
                    .OrderByDescending(x => x.LastModified)
                    .Select(x =>
                        new
                        {
                            Qa1User = x.LastModifiedBy,
                            Qa1Submitted = x.LastModified,
                            x.IsAccepted
                        })
                    .FirstAsync(cancellationToken);

                var enrolmentFeedback = EnrolmentFeedback.Create(
                    entry.ParticipantId,
                    qa1.Qa1User!,
                    request.MessageToQa1,
                    qa1.IsAccepted ? FeedbackOutcome.Approved : FeedbackOutcome.Returned,
                    outcome,
                    FeedbackStage.Escalation,
                    qa1.Qa1Submitted!.Value,
                    request.EnrolmentFeedbackReason
                );

                unitOfWork.DbContext.EnrolmentFeedbacks.Add(enrolmentFeedback);
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
            
            When(x => x.Response is EscalationResponse.Comment, () =>
            {
                RuleFor(x => x.Message)
                    .NotEmpty()
                    .WithMessage("Internal Message is required for this response.")
                    .MaximumLength(ValidationConstants.NotesLength)
                    .WithMessage(string.Format(ValidationConstants.NotesMessage, "Message"));
            });                
            
            RuleFor(x => x.MessageToProvider)
                .MaximumLength(ValidationConstants.NotesLength);

            When(x => x.Response == EscalationResponse.Accept, () =>
            {
                RuleFor(x => x.FeedbackType)
                    .NotNull()
                    .WithMessage("You must select a feedback type when accepting")
                    .Must(ft => ft != FeedbackType.Returned)
                    .WithMessage("FeedbackType cannot be 'Returned' when accepting");
            });

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

            When(x => x.Response == EscalationResponse.Accept && (x.FeedbackType == FeedbackType.Advisory || x.FeedbackType == FeedbackType.AcceptedByException), () =>
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
                RuleFor(x => x.EnrolmentFeedbackReason)
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
                RuleFor(c => c.QueueEntryId)
                    .MustAsync(MustExist)
                    .WithMessage("Queue item does not exist");
            });
        }

        private async Task<bool> MustExist(Guid identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.EnrolmentEscalationQueue.AnyAsync(e => e.Id == identifier, cancellationToken);
    }

    public class C_ShouldNotBeComplete : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public C_ShouldNotBeComplete(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.QueueEntryId)
                    .MustAsync(MustBeOpen)
                    .WithMessage("Queue item is already completed.");
            });
        }

        private async Task<bool> MustBeOpen(Guid id, CancellationToken cancellationToken)
        {
            var entry = await _unitOfWork.DbContext.EnrolmentEscalationQueue.Include(c => c.Participant)
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
                RuleFor(c => c.QueueEntryId)
                    .MustAsync(MustBeAtSubmittedToAuthority)
                    .WithMessage("Queue item is not at Submitted to Authority stage");
            });
        }

        private async Task<bool> MustBeAtSubmittedToAuthority(Guid id, CancellationToken cancellationToken)
        {
            var entry = await _unitOfWork.DbContext.EnrolmentEscalationQueue.Include(c => c.Participant)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken: cancellationToken);

            return entry != null && entry.Participant!.EnrolmentStatus == EnrolmentStatus.SubmittedToAuthorityStatus;
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
                    .WithMessage("This enrolment was assigned to you hence must not be processed at Escalation stage by you");
            });
        }

        private async Task<bool> OwnerMustNotBeApprover(Command c, CancellationToken cancellationToken)
        {
            var entry = await _unitOfWork.DbContext.EnrolmentEscalationQueue
                .FirstOrDefaultAsync(a => a.Id == c.QueueEntryId, cancellationToken: cancellationToken);

            return entry != null && entry.SupportWorkerId.Equals(c.CurrentUser!.UserId) == false;
        }
    }
}