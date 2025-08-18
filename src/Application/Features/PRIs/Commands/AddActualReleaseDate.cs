using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PRIs.Commands;

public static class AddActualReleaseDate
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]

    public class Command : IRequest<Result>
    {
        [Description("Participant Id")]
        public required string ParticipantId { get; set; }

        [Description("Actual Release Date")]
        public DateTime? ActualReleaseDate { get; set; }         
    }

    private class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {               
            var pri = await unitOfWork.DbContext.PRIs
                .SingleOrDefaultAsync(p => p.ParticipantId == request.ParticipantId 
                && p.ActualReleaseDate == null 
                && PriStatus.ActiveList.Contains(p.Status), cancellationToken);

            if (pri == null)
            {
                throw new NotFoundException("Cannot find PRI", request.ParticipantId);
            }

            pri.SetActualReleaseDate(DateOnly.FromDateTime(request.ActualReleaseDate!.Value));
            
            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.ParticipantId)
                .Length(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

            RuleFor(c => c.ActualReleaseDate)
                .NotNull()
                .WithMessage("Actual Release Date is required")
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage(ValidationConstants.DateMustBeInPast);
        }
    }
    public class A_ParticipantMustBeActive : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public A_ParticipantMustBeActive(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(a => a.ParticipantId)
                    .Must(MustNotBeArchived)
                    .WithMessage("Participant is archived");
            });
        }

        private bool MustNotBeArchived(string participantId)
                => _unitOfWork.DbContext.Participants.Any(e => e.Id == participantId && e.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value);
    }

    public class B_ParticipantMustExist : AbstractValidator<Command>
    {
        private IUnitOfWork _unitOfWork;

        public B_ParticipantMustExist(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(b => b.ParticipantId)
                    .Must(Exist)
                    .WithMessage("Participant does not exist"); 
            });
        }

        private bool Exist(string identifier) => _unitOfWork.DbContext.Participants.Any(e => e.Id == identifier);
    }
}