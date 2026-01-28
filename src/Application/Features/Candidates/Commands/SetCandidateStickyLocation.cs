using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.Candidates.Commands;

public static class SetCandidateStickyLocation
{
    [RequestAuthorize(Policy = SecurityPolicies.Qa1)]
    public class Command : IRequest<Result>
    {
        public string? ParticipantId { get; set; }
        public string? Region { get; set; }
        
        [Description("Justification")]
        public string? Justification { get;set; }
        
        [Description("Call Reference")]
        public string? CallReference { get; set; }
    }

    public class Handler(ICandidateService candidateService, IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = unitOfWork.DbContext.Participants.FirstOrDefault(x => x.Id == request.ParticipantId!);
            
            if(participant is not null)
            {
                var callResult = await candidateService.SetStickyLocation(request.ParticipantId!, request.Region!);
                if(callResult is { Succeeded: true, Data: true })
                {
                    participant.AddNote(new Note()
                    {
                        Message = request.Justification!,
                        CallReference = request.CallReference!
                    });
                    return Result.Success();
                }
                return callResult;
            }
            return Result.Failure();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.ParticipantId)
                .NotNull()
                .MaximumLength(9)
                .MinimumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));                

            RuleFor(x => x.Region)
                .NotNull()
                .MaximumLength(4)
                .MinimumLength(4)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Region"));

            RuleFor(x => x.Justification)
                .NotEmpty()
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Justification"));
            
            RuleFor(x => x.CallReference)
                .NotEmpty()
                .Matches(ValidationConstants.Numbers)
                .WithMessage(string.Format(ValidationConstants.NumbersMessage, "Call Reference"));

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(x => x.ParticipantId)
                    .MustAsync(Exist)
                    .WithMessage("Participant not found")
                    .MustAsync(MustNotBeArchived)
                    .WithMessage("Participant is archived");
            });
        }

        private async Task<bool> Exist(string? participantId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId, cancellationToken);

        private async Task<bool> MustNotBeArchived(string? participantId, CancellationToken cancellationToken)
                => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId && e.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value, cancellationToken);
    }
}