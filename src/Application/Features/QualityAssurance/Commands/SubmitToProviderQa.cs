using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Commands;

public static class SubmitToProviderQa
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : IRequest<Result>
    {
        public required string ParticipantId { get; set; }
        public string? JustificationReason { get;set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants.FindAsync(request.ParticipantId);
            participant!.TransitionTo(EnrolmentStatus.SubmittedToProviderStatus)
                .SetAssessmentJustification(request.JustificationReason);
            return Result.Success();
        }
    }

    public class A_ParticipantMustExistValidator : AbstractValidator<Command> 
    {
        private IUnitOfWork _unitOfWork;
        public A_ParticipantMustExistValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .MinimumLength(9)
                .MaximumLength(9)
                .WithMessage("Invalid Participant Id")
                .Matches(ValidationConstants.AlphaNumeric).WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"))
                .MustAsync(MustExist)
                .WithMessage("Participant does not exist");                

            RuleFor(c => c.JustificationReason)
                .Matches(x => ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Justification Reason"));
        }

        private async Task<bool> MustExist(string identifier, CancellationToken cancellationToken)
                => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);
    }

    public class B_ParticipantAssessmentShouldExist : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public B_ParticipantAssessmentShouldExist(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .MustAsync(MustExist)
                .WithMessage($"No assessment found for participant.");
        }

        private async Task<bool> MustExist(string identifier, CancellationToken cancellationToken)
              => await _unitOfWork.DbContext.ParticipantAssessments.AnyAsync(e => e.ParticipantId == identifier, cancellationToken);
    }
    
    public class C_ParticipantAssessmentShouldBeSubmitted : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public C_ParticipantAssessmentShouldBeSubmitted(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .MustAsync(MustBeScored)
                .WithMessage("Assessment has not been submitted and scored.");
        }

        private async Task<bool> MustBeScored(string identifier, CancellationToken cancellationToken)
        {
            var assessments = await  _unitOfWork.DbContext.ParticipantAssessments
                .Include(pa => pa.Scores)
                .Where(pa => pa.ParticipantId == identifier)
                .ToArrayAsync(cancellationToken);

            var latest = assessments.MaxBy(a => a.Created);

            return latest is not null
                   && latest.Scores.All(s => s.Score >= 0);
        }
    }

    public class D_AssessmentMustHaveJustification : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public D_AssessmentMustHaveJustification(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c)
                .MustAsync(MustBeJustified)
                .WithMessage("Eligibility for the programme requires the assessment to have a minimum of one red and one amber. Participants with at least 2 reds do not require justification");
        }

        private async Task<bool> MustBeJustified(Command command, CancellationToken cancellationToken)
        {
            var latest = await _unitOfWork.DbContext.ParticipantAssessments
                .Include(pa => pa.Scores)
                .Where(pa => pa.ParticipantId == command.ParticipantId)
                .OrderByDescending(a => a.Created)
                .FirstOrDefaultAsync(cancellationToken);

            if (latest is null)
            {
                return false;
            }
            
            // we have zero reds
            if (latest.Scores.Count(s => s.Score is >= 0 and < 10) == 0)
            {
                return false;
            }
            
            // we have two or more reds
            if (latest.Scores.Count(s => s.Score is >= 0 and < 10) >= 2)
            {
                return true;
            }
            
            // ok if we get here, we have at LEAST 1 red. So we just need an amber AND a justification
            if (latest.Scores.Count(s => s.Score is < 25) >= 2 
                && string.IsNullOrEmpty(command.JustificationReason) == false)
            {
                return true;
            }

            // we do not have enough to pass.
            return false;
        }
    }

    public class E_ParticipantStatusShouldBeEnrollingStatus : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public E_ParticipantStatusShouldBeEnrollingStatus(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .MustAsync(MustBeInEnrollingStatus)
                .WithMessage("Participant must be in Enrolling status");
        }

        private async Task<bool> MustBeInEnrollingStatus(string participantId, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.DbContext.Participants.SingleOrDefaultAsync(p => p.Id == participantId);
            return result?.EnrolmentStatus == EnrolmentStatus.EnrollingStatus;
        }
    }

    public class F_ParticipantConsentDateCantBeMoreThanThreeMonthsAgo : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public F_ParticipantConsentDateCantBeMoreThanThreeMonthsAgo(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .MustAsync(HaveGivenConsentWithinLastThreeMonths)
                .WithMessage("Participant must have given consent within the last 3 months, up to date consent documentation is required");
        }

        private async Task<bool> HaveGivenConsentWithinLastThreeMonths(string participantId, CancellationToken cancellationToken)
        {
            var participant = await _unitOfWork.DbContext
                .Participants.SingleAsync(x => x.Id == participantId, cancellationToken);

            var consentDate = participant.CalculateConsentDate();

            return consentDate >= DateTime.Today.AddMonths(-3);                
        }
    }
}