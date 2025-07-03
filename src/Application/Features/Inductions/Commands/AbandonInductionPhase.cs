using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Inductions;

namespace Cfo.Cats.Application.Features.Inductions.Commands;

public static class AbandonInductionPhase
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        public Guid WingInductionId { get; set; }
        
        [Description("Date Abandoned")]
        public DateTime? CompletionDate { get; set; }

        [Description("Abandoned By")]
        public UserProfile? CurrentUser { get; set; }

        [Description("Abandoned Justification")]
        public string? AbandonJustification { get; set; }

        [Description("Reason Abandoned")]
        public WingInductionPhaseAbandonReason? AbandonReason { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            WingInduction element = await unitOfWork.DbContext.WingInductions
                .Include(wi => wi.Phases)
                .FirstAsync(wi => wi.Id == request.WingInductionId, cancellationToken);

            try
            {
                element.AbandonCurrentPhase(request.CompletionDate!.Value, request.AbandonJustification, request.AbandonReason, request.CurrentUser?.UserId!);
            }
            catch (Exception e)
            {
                return Result.Failure(e.Message);
            }

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

            RuleFor(c => c.CompletionDate)
                .NotNull()
                .LessThan(DateTime.Today.AddDays(1).Date)
                .WithMessage("Phase Abandon date cannot be in the future");

            RuleFor(c => c.CurrentUser)
                .NotNull();
            
            RuleFor(c => c.AbandonJustification)
                .NotEmpty()
                .When(c => c.AbandonReason!.RequiresJustification)
                .WithMessage("You must provide a justification for the selected abandon reason")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Justification"));

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.WingInductionId)
                    .MustAsync(ParticipantMustNotBeArchived)
                    .WithMessage("Participant is archived");

                RuleFor(c => c.WingInductionId)
                    .MustAsync(MustExist)
                    .WithMessage("No wing induction found");

                RuleFor(x => x.WingInductionId)
                    .MustAsync(MustHaveOpenPhase)
                    .WithMessage("No open phases to abandon.");

                RuleFor(x => x)
                    .MustAsync(CompletionMustBeAfterStartDate)
                    .WithMessage("Abandon must be after the start date");
            });
        }

        private async Task<bool> MustExist(Guid id, CancellationToken cancellationToken)
        {
            var element = await _unitOfWork.DbContext.WingInductions
                .Include(wi => wi.Phases)
                .FirstOrDefaultAsync(wi => wi.Id == id, cancellationToken);

            return element != null;
        }
        
        private async Task<bool> MustHaveOpenPhase(Guid id, CancellationToken cancellationToken)
        {
            // as this is called second we can be sure this will not return null
            WingInduction element = await _unitOfWork.DbContext.WingInductions
                .Include(wi => wi.Phases)
                .FirstAsync(wi => wi.Id == id, cancellationToken);

            int count = element.Phases.Count(x => x.CompletedDate is null);

            return count == 1;
        }
        
        private async Task<bool> CompletionMustBeAfterStartDate(Command command, CancellationToken cancellationToken)
        {
            // as this is called second we can be sure this will not return null
            WingInduction element = await _unitOfWork.DbContext.WingInductions
                .Include(wi => wi.Phases)
                .FirstAsync(wi => wi.Id == command.WingInductionId, cancellationToken);

            var openPhase = element.GetOpenPhase();

            return openPhase.StartDate < command.CompletionDate;
        }

        private async Task<bool> ParticipantMustNotBeArchived(Guid id, CancellationToken cancellationToken)
        {
            var participantId =  await (from wi in _unitOfWork.DbContext.WingInductions
                                 join p in _unitOfWork.DbContext.Participants on wi.ParticipantId equals p.Id
                                 where (wi.Id == id
                                        && p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value)
                                 select p.Id)
                            .AsNoTracking()
                            .FirstOrDefaultAsync();

            return participantId != null;
        }       
    }
}