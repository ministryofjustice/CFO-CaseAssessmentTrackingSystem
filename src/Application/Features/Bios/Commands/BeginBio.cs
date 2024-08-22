using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Bios.DTOs;
using Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.Diversity;
using Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.ChildhoodExperiences;
using Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.RecentExperiences;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Bios;
using Newtonsoft.Json;
using Cfo.Cats.Application.Common.Validators;


namespace Cfo.Cats.Application.Features.Bios.Commands;

public static class BeginBio
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result<Guid>>
    {
        public required string ParticipantId { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            Bio bio = new Bio()
            {
                Id = Guid.NewGuid(),
                ParticipantId = request.ParticipantId,
                Pathways =
                [
                    new ChildhoodExperiencesPathway(),
                    new DiversityPathway(),
                    new RecentExperiencesPathway(),
                ]
            };

            string json = JsonConvert.SerializeObject(bio, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            
            ParticipantBio bioSurvey = ParticipantBio.Create(bio.Id, request.ParticipantId, bioJson: json);

            await _unitOfWork.DbContext.ParticipantBios.AddAsync(bioSurvey);

            return Result<Guid>.Success(bio.Id);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, nameof(Command.ParticipantId)));
        }
    }

}
