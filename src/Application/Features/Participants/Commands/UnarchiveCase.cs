using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class UnarchiveCase
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : IRequest<Result>
    {
        public required string ParticipantId { get; set; }
        [Description("Reason for Unarchive")] public UnarchiveReason UnarchiveReason { get; set; } = UnarchiveReason.CaseloadManageable;
        [Description("Additional Information")] public string? AdditionalInformation { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants.FindAsync(request.ParticipantId);

            //need to get previous status
            var previousStatus = await unitOfWork.DbContext.ParticipantEnrolmentHistories
                    .Where(eh => eh.ParticipantId == request.ParticipantId)
                    .OrderByDescending(eh => eh.Created)
                    .Skip(1)
                    .Select(eh => (int?)eh.EnrolmentStatus)
                    .FirstOrDefaultAsync();

            if (previousStatus == null)
            {
                throw new Exception("The participant does not have enough history to unarchive.");
            }

            EnrolmentStatus enrolmentStatus = EnrolmentStatus.FromValue(previousStatus.GetValueOrDefault());

            //need to check been archived more than 6 to reset risk due
            var archivedCount = await unitOfWork.DbContext.ParticipantEnrolmentHistories
                    .Where(eh => eh.ParticipantId == request.ParticipantId && eh.EnrolmentStatus == 4)
                    .CountAsync();
                        
            var dateOfArchive = await unitOfWork.DbContext.ParticipantEnrolmentHistories
                    .Where(eh => eh.ParticipantId == request.ParticipantId && eh.EnrolmentStatus == 4)
                    .OrderByDescending(eh => eh.Created)
                    .Select(eh => eh.Created)
                    .FirstOrDefaultAsync();

            if ((archivedCount > 0 && archivedCount % 7 == 0)
                || (dateOfArchive.HasValue && dateOfArchive.Value < DateTime.UtcNow.AddMonths(-6)))
            {
                participant!.SetRiskDue(DateTime.UtcNow, RiskDueReason.RemovedFromArchive);
            }

            participant!.TransitionTo(enrolmentStatus, request.UnarchiveReason.Name, request.AdditionalInformation);

            // ReSharper disable once MethodHasAsyncOverload
            return Result.Success();
        }
    }

    public class A_ParticipantMustExistValidator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;
        public A_ParticipantMustExistValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .MinimumLength(9)
                .MaximumLength(9)
                .WithMessage("Invalid Participant Id");                

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ParticipantId)
                    .MustAsync(MustExist)
                    .WithMessage("Participant does not exist")
                    .MustAsync(MustBeArchived)
                    .WithMessage("Participant is not archived");
            });
        }

        private async Task<bool> MustExist(string identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);

        private async Task<bool> MustBeArchived(string participantId, CancellationToken cancellationToken)
           => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId && e.EnrolmentStatus == EnrolmentStatus.ArchivedStatus.Value);
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.AdditionalInformation)
                .NotEmpty()
                .When(c => c.UnarchiveReason.RequiresFurtherInformation)
                .WithMessage("You must provide additional information for the selected unarchive reason")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Addtional Information"));
        }
    }
}