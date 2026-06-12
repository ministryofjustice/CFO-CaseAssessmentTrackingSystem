using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantNotes
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : ParticipantNotesFilter, IRequest<PaginatedData<ParticipantNoteDto>>
    {
        public ParticipantNotesSpecification Specification => new();
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, PaginatedData<ParticipantNoteDto>>
    {
        public async Task<PaginatedData<ParticipantNoteDto>> Handle(Query request, CancellationToken cancellationToken) =>
            await unitOfWork.DbContext.Participants
                .Where(p => p.Id == request.ParticipantId)
                .SelectMany(p => p.Notes)
                .OrderByDescending(n => n.Created)
                .ProjectToPaginatedDataAsync<Domain.ValueObjects.Note, ParticipantNoteDto>(
                    request.Specification,
                    request.PageNumber,
                    request.PageSize,
                    n => new ParticipantNoteDto
                    {
                        Id = n.CreatedBy ?? string.Empty,
                        Message = n.Message,
                        CallReference = n.CallReference,
                        ParticipantId = request.ParticipantId,
                        CreatedBy = n.CreatedByUser!.DisplayName!,
                        CreatedByEmail = n.CreatedByUser!.Email!,
                        Created = n.Created ?? DateTime.UtcNow
                    },
                    cancellationToken);
    }
    
    public class Validator : AbstractValidator<Query>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .Length(9)
                .WithMessage("Invalid Participant Id")
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ParticipantId)
                    .MustAsync(Exist)
                    .WithMessage("Participant does not exist");
            });
        }

        private async Task<bool> Exist(string identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);
    }
}