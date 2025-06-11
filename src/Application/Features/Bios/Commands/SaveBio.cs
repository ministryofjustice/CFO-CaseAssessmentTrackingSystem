using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Bios.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Bios;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Bios.Commands;

public static class SaveBio
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        public bool Submit { get; set; } = false;
        
        public required Bio Bio { get; set; } 
    }

    public class Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            ParticipantBio? bio = await unitOfWork.DbContext.ParticipantBios
                    .FirstOrDefaultAsync(r => r.Id == request.Bio.Id, cancellationToken);
                                      
            if(bio == null)
            {
                return Result.Failure("Bio not found");
            }
            
            bio.UpdateJson(JsonConvert.SerializeObject(request.Bio, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            }));

            bio.UpdateStatus(BioStatus.InProgress);

            if (request.Submit)
            {
                bio.UpdateStatus(BioStatus.Complete);
                bio.Submit(currentUserService.UserId!);
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

            RuleFor(x => x.Bio)
                .NotNull();
          
            RuleFor(x => x.Bio.ParticipantId)
                .MinimumLength(9)
                .WithMessage("Invalid Participant Id")
                .MaximumLength(9)
                .WithMessage("Invalid Participant Id")
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, nameof(Command.Bio.ParticipantId)))
                .MustAsync(MustNotBeArchived)
                .WithMessage("Participant is archived");

            RuleFor(x => x.Bio.Id)
                .NotEmpty()
                .MustAsync(NotBeCompleted)
                .WithMessage("Bio already complete");
        }

        private async Task<bool> NotBeCompleted(Guid bioId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.ParticipantBios.AnyAsync(b => b.Id == bioId && b.Completed == null, cancellationToken);

        private async Task<bool> MustNotBeArchived(string participantId, CancellationToken cancellationToken)
             => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId && e.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value, cancellationToken);
    }
}