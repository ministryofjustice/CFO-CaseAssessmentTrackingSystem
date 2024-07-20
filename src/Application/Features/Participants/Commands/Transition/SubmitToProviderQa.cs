using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Commands.Transition;

public static class SubmitToProviderQa
{
    [RequestAuthorize(Policy = PolicyNames.AuthorizedUser)]
    public class Command : IRequest<Result>
    {
        public required string ParticipantId { get; set; }

    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants.FindAsync(request.ParticipantId);
            participant!.TransitionTo(EnrolmentStatus.SubmittedToProviderStatus);
            // ReSharper disable once MethodHasAsyncOverload
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
                .MustAsync(MustExist)
                .WithMessage("Participant does not exist");
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
                .ToArrayAsync(cancellationToken);

            var latest = assessments.MaxBy(a => a.Created);

            return latest is not null
                   && latest.Scores.All(s => s.Score >= 0);
        }
    }

    public class D_ParticipantShouldHaveAtLeastTwoReds : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public D_ParticipantShouldHaveAtLeastTwoReds(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .MustAsync(MustHaveTwoReds)
                .WithMessage("Assessment does not have two reds. Justification must be provided");
        }

        private async Task<bool> MustHaveTwoReds(string identifier, CancellationToken cancellationToken)
        {
            var assessments = await  _unitOfWork.DbContext.ParticipantAssessments
                                    .Include(pa => pa.Scores)
                                    .ToArrayAsync(cancellationToken);

            var latest = assessments.MaxBy(a => a.Created);

            return latest is not null
                && latest.Scores.Count(s => s.Score is > 0 and < 10) > 1;
        }
    }

}
