using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Candidates.Caching;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Commands.Transistion;

public static class SubmitToProviderQa
{
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Command : ICacheInvalidatorRequest<Result>
    {
        public required string ParticipantId { get; set; }
        
        public string[] CacheKeys => [
            ParticipantCacheKey.GetCacheKey(ParticipantId),
            ParticipantCacheKey.GetSummaryCacheKey(ParticipantId)
        ];
        
        public CancellationTokenSource? SharedExpiryTokenSource => CandidatesCacheKey.SharedExpiryTokenSource();
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

    public class ParticipantMustExistValidator : AbstractValidator<Command> 
    {
        private IUnitOfWork _unitOfWork;
        public ParticipantMustExistValidator(IUnitOfWork unitOfWork)
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

    public class ParticipantAssessmentShouldExist : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ParticipantAssessmentShouldExist(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .MinimumLength(9)
                .MaximumLength(9)
                .WithMessage("Invalid Participant Id")
                .MustAsync(MustExist)
                .WithMessage("Participant must have an assessment");

        }

        private async Task<bool> MustExist(string identifier, CancellationToken cancellationToken)
              => await _unitOfWork.DbContext.ParticipantAssessments.AnyAsync(e => e.ParticipantId == identifier, cancellationToken);

    }

    public class ParticipantShouldHaveAtLeastTwoReds : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public ParticipantShouldHaveAtLeastTwoReds(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .MinimumLength(9)
                .MaximumLength(9)
                .WithMessage("Invalid Participant Id")
                .MustAsync(MustHaveTwoReds)
                .WithMessage("Assessment should have two reds");
        }

        private async Task<bool> MustHaveTwoReds(string identifier, CancellationToken cancellationToken)
        {
            var assessments = await  _unitOfWork.DbContext.ParticipantAssessments
                                    .Include(pa => pa.Scores)
                                    .ToArrayAsync(cancellationToken);

            var latest = assessments.OrderByDescending(a => a.Created).FirstOrDefault();

            return latest is not null
                && latest.Scores.Where(s => s.Score < 10).Count() > 1;
        }
    }

}
