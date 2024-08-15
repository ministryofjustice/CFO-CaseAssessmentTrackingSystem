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

    public class Handler : IRequestHandler<Command, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public Handler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            
            ParticipantBio? bio = await _unitOfWork.DbContext.ParticipantBios
                    .FirstOrDefaultAsync(r => r.Id == request.Bio.Id && r.ParticipantId == request.Bio.ParticipantId, cancellationToken);
                                      
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
                bio.Submit();
            }

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Bio)
                .NotNull();

            RuleFor(x => x.Bio.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, nameof(Command.Bio.ParticipantId)));
            
            RuleFor(x => x.Bio.Id)
                .NotEmpty();

        }
    }

}
