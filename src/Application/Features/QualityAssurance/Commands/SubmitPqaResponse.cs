using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Commands;

public static class SubmitPqaResponse
{
    [RequestAuthorize(Policy = SecurityPolicies.Pqa)]
    public class Command : IRequest<Result>
    {
        public required Guid QueueEntryId { get; set; }
        
        public PqaResponse? Response { get; set; }

        public string Message { get; set; } = default!;
        public UserProfile? CurrentUser { get; set; }
    }

    public enum PqaResponse
    {
        Accept = 0,
        Return = 1,
        Comment = 2
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var entry = await unitOfWork.DbContext.EnrolmentPqaQueue
                .Include(pqa => pqa.Participant)
                .FirstOrDefaultAsync(x => x.Id == request.QueueEntryId, cancellationToken: cancellationToken);

            if (entry == null)
            {
                return Result.Failure("Cannot find queue item");
            }

            entry.AddNote(request.Message);

            switch (request.Response)
            {
                case PqaResponse.Accept:
                    entry.Accept();
                    break;
                case PqaResponse.Return:
                    entry.Return();
                    break;
                case PqaResponse.Comment:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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

            When(x => x.Response is PqaResponse.Return or PqaResponse.Comment, () =>
            {
                RuleFor(x => x.Message)
                    .NotEmpty()
                    .WithMessage("A message is required for this response.")
                    .MaximumLength(ValidationConstants.NotesLength)
                    .WithMessage(string.Format(ValidationConstants.NotesMessage, "Message"));
            });
        }
    }

    public class B_EntryMustExist : AbstractValidator<Command> 
    {
        private readonly IUnitOfWork _unitOfWork;

        public B_EntryMustExist(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () => {
                RuleFor(c => c.QueueEntryId)
                    .Must(MustExist)
                    .WithMessage("Enrolment queue item does not exist");
            });            
        }

        private bool MustExist(Guid identifier)
            =>  _unitOfWork.DbContext.EnrolmentPqaQueue.Any(e => e.Id == identifier);
    }

    public class C_ShouldNotBeComplete : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public C_ShouldNotBeComplete(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () => {
                RuleFor(c => c.QueueEntryId)
                    .Must(MustBeOpen)
                    .WithMessage("Enrolment queue item is already completed.");    
            });
        }

        private bool MustBeOpen(Guid id)
        {
            var entry = _unitOfWork.DbContext.EnrolmentPqaQueue.Include(c => c.Participant)
                .FirstOrDefault(a => a.Id == id);

            return entry is { IsCompleted: false };
        }
    }

    public class D_OwnerShouldNotBeApprover : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public D_OwnerShouldNotBeApprover(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
            RuleSet(ValidationConstants.RuleSet.MediatR, () => {
                RuleFor(c => c)
                    .Must(OwnerMustNotBeApprover)
                    .WithMessage("This enrolment was assigned to you hence must not be processed at PQA stage by you");    
            });
        }

        private bool OwnerMustNotBeApprover(Command c)
        {
            var entry = _unitOfWork.DbContext.EnrolmentPqaQueue
                .FirstOrDefault(a => a.Id == c.QueueEntryId);

            return entry != null && entry.SupportWorkerId.Equals(c.CurrentUser!.UserId) == false;
        }
    }

    public class E_EnrolmentOccurredWithin3Months : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public E_EnrolmentOccurredWithin3Months(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () => {
                RuleFor(f => f)
                    .Must(EnrolmentOccurredWithin3Months)
                    .WithMessage($"The enrolment consent date is over 3 months ago")
                    .When(f => f.Response == PqaResponse.Accept);
            });
        }

        private bool EnrolmentOccurredWithin3Months(Command c)
        {
            var entry = _unitOfWork.DbContext.EnrolmentPqaQueue
                       .Include(c => c.Participant)
                       .Include(d => d.Participant!.Consents)
                       .First(a => a.Id == c.QueueEntryId);
                                  
            return entry.Participant!.ConsentStatus == ConsentStatus.GrantedStatus || entry.Participant!.CalculateConsentDate() >= DateTime.Today.AddMonths(-3);
        }
    }
  
    public class F_ParticipantMustNotBeArchived : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public F_ParticipantMustNotBeArchived(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () => {
                When(g => g.Response is PqaResponse.Accept, () =>
                {
                    RuleFor(g => g.QueueEntryId)
                        .Must(ParticipantMustNotBeArchived)
                        .WithMessage("Participant is archived");
                });
            });
        }

        private bool ParticipantMustNotBeArchived(Guid queueEntryId)
        {

            var entry = _unitOfWork.DbContext.EnrolmentPqaQueue.Include(c => c.Participant)
                .FirstOrDefault(a => a.Id == queueEntryId);

            return entry != null && entry.Participant!.EnrolmentStatus != EnrolmentStatus.ArchivedStatus;
        }  
    }

    public class G_ParticipantNotDeactivatedInFeedOver30DaysAgo : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public G_ParticipantNotDeactivatedInFeedOver30DaysAgo(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                When(g => g.Response is PqaResponse.Accept, () =>
                {
                    RuleFor(g => g.QueueEntryId)
                        .Must(ParticipantNotDeactivatedInFeedOver30DaysAgo)
                        .WithMessage("Cannot submit to CFO QA as post-licence case closure period has lapsed");
                });
            });
        }

        private bool ParticipantNotDeactivatedInFeedOver30DaysAgo(Guid queueEntryId)
        {
            var thirtyDaysAgo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));

            var entry = _unitOfWork.DbContext.EnrolmentPqaQueue.Include(c => c.Participant)
                        .FirstOrDefault(a => a.Id == queueEntryId);

            return entry != null && (entry.Participant!.DeactivatedInFeed == null 
                                    || entry.Participant.DeactivatedInFeed >= thirtyDaysAgo);
        }
    }
}