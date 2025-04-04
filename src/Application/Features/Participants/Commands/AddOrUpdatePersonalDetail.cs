using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public class AddOrUpdatePersonalDetail
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        public required string ParticipantId { get; set; }
        public ParticipantPersonalDetailDto PersonalDetails { get; set; } = new();
    }

    class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Command, PersonalDetail>(MemberList.None)
                .ConstructUsing(command => PersonalDetail.CreateFrom(
                    command.PersonalDetails.PreferredNames,
                    command.PersonalDetails.PreferredPronouns,
                    command.PersonalDetails.PreferredTitle,
                    command.PersonalDetails.AdditionalNotes)
                )
                .AfterMap((_, destination) => destination.ClearDomainEvents());

            CreateMap<PersonalDetail, ParticipantPersonalDetailDto>(MemberList.None);
        }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants
                .SingleAsync(s => s.Id == request.ParticipantId, cancellationToken);

            var personalDetail = mapper.Map<PersonalDetail>(request);

            participant.UpdatePersonalDetail(personalDetail);

            return Result.Success();
        }
    }

    public class A_IsValid : AbstractValidator<Command>
    {
        public A_IsValid()
        {
            RuleFor(c => c.PersonalDetails.PreferredNames)
                .MaximumLength(128)
                .WithMessage("Maximum length of 128 characters exceeded");

            RuleFor(c => c.PersonalDetails.PreferredPronouns)
                .MaximumLength(16)
                .WithMessage("Maximum length of 16 characters exceeded");

            RuleFor(c => c.PersonalDetails.PreferredTitle)
                .MaximumLength(6)
                .WithMessage("Maximum length of 6 characters exceeded");

            RuleFor(c => c.PersonalDetails.AdditionalNotes)
                .MaximumLength(256)
                .WithMessage("Maximum length of 256 characters exceeded")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Additional Notes"));
        }
    }

    public class B_ParticipantMustExist : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public B_ParticipantMustExist(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .Must(Exist);
        }

        private bool Exist(string identifier) => _unitOfWork.DbContext.Participants.Any(e => e.Id == identifier);
    }
}
