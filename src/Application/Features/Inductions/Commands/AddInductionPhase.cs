using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Inductions;

namespace Cfo.Cats.Application.Features.Inductions.Commands;

public static class AddInductionPhase
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        public Guid WingInductionId { get; set; }

        [Description("Start Date")]
        public DateTime? StartDate { get; set; }

        public UserProfile? CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var induction = await unitOfWork.DbContext.WingInductions
                .Include(wi => wi.Phases)
                .FirstAsync(wi => wi.Id == request.WingInductionId, cancellationToken);

            induction.AddPhase(request.StartDate!.Value);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.WingInductionId)
                .NotEmpty()
                .WithMessage("Wing Induction should not be empty");
                
            RuleFor(c => c.StartDate)
                .NotNull()
                .LessThan(DateTime.Today.AddDays(1).Date)
                .WithMessage("Phase start date cannot be in the future");

            RuleFor(c => c.CurrentUser)
                .NotNull();

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.WingInductionId)      
                    .MustAsync(MustExist)
                    .WithMessage("No wing induction found")
                    .MustAsync(MustHaveNoOpenPhases)
                    .WithMessage("Cannot add a new phase while an existing one is open")
                    .MustAsync(ParticipantMustNotBeArchived)
                    .WithMessage("Participant is archived");

                RuleFor(x => x)
                    .MustAsync(MustBeAfterPrecedingPhaseClosures)
                    .WithMessage("Phase cannot commence before other phases were completed");
            });
        }

        private async Task<bool> MustExist(Guid id, CancellationToken cancellationToken)
        {
            var element = await _unitOfWork.DbContext.WingInductions
                .Include(wi => wi.Phases)
                .FirstOrDefaultAsync(wi => wi.Id == id, cancellationToken);

            return element != null;
        }

        private async Task<bool> MustHaveNoOpenPhases(Guid id, CancellationToken cancellationToken)
        {
            // as this is called second we can be sure this will not return null
            WingInduction element = await _unitOfWork.DbContext.WingInductions
                .Include(wi => wi.Phases)
                .FirstAsync(wi => wi.Id == id, cancellationToken);

            int count = element.Phases.Count(x => x.CompletedDate is null);

            return count == 0;
        }

        private async Task<bool> MustBeAfterPrecedingPhaseClosures(Command command, CancellationToken cancellationToken)
        {
            // as this is called second we can be sure this will not return null
            WingInduction element = await _unitOfWork.DbContext.WingInductions
                .Include(wi => wi.Phases)
                .FirstAsync(wi => wi.Id == command.WingInductionId, cancellationToken);

            if (element.Phases is { Count: 0 })
            {
                // doesn't matter we have no phases
                return true;
            }

            return element.Phases.Max(e => e.CompletedDate)?.Date <= command.StartDate;
        }

        private async Task<bool> ParticipantMustNotBeArchived(Guid id, CancellationToken cancellationToken)
        {
            var participantId = await (from pp in _unitOfWork.DbContext.WingInductions
                                       join p in _unitOfWork.DbContext.Participants on pp.ParticipantId equals p.Id
                                       where (pp.Id == id
                                       && p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value)
                                       select p.Id
                                       )
                            .AsNoTracking()
                            .FirstOrDefaultAsync();

            return participantId != null;
        }
    }
}