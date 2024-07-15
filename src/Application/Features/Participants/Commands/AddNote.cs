using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class AddNote
{
    [RequestAuthorize(Policy = PolicyNames.AuthorizedUser)]
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
                .WithMessage("Invalid Participant Id")
                .MustAsync(Exist)
                .WithMessage("Participant does not exist");

            RuleFor(c => c.Message)
                .NotEmpty()
                .MaximumLength(255);
        }

        private async Task<bool> Exist(string identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);

    }

}
