﻿using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class AddNote
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : IRequest<Result>
    {
        [Description("Participant Id")]
        public required string ParticipantId { get; set; }

        [Description("Message")]
        public required string Message { get; set; }

        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Command, Note>(MemberList.None);
            }
        }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants.FindAsync(request.ParticipantId, cancellationToken);

            if (participant == null)
            {
                throw new NotFoundException("Cannot find participant", request.ParticipantId);
            }

            var note = mapper.Map<Note>(request);

            participant.AddNote(note);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .Length(9)
                .WithMessage("Invalid Participant Id");
                
            RuleFor(c => c.Message)
                .NotEmpty()
                .MaximumLength(ValidationConstants.NotesLength)
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Message"));

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ParticipantId)     
                    .MustAsync(Exist)
                    .WithMessage("Participant does not exist")
                    .MustAsync(MustNotBeArchived)
                    .WithMessage("Participant is archived");
            });
        }

        private async Task<bool> Exist(string identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);
 
        private async Task<bool> MustNotBeArchived(string participantId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId && e.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value, cancellationToken);
    }
}