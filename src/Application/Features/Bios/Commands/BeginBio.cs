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
        
        public Handler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;        
        }

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var oldBio = await GetPreviousBio(request);

            Bio bio = new Bio()
            {
                Id = Guid.CreateVersion7(),
                ParticipantId = request.ParticipantId,
                Pathways =
                [
                    oldBio?.Pathways[0] ?? new DiversityPathway(),
                    oldBio?.Pathways[1] ?? new ChildhoodExperiencesPathway(),                    
                    oldBio?.Pathways[2] ?? new RecentExperiencesPathway(),
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

        private async Task<Bio?> GetPreviousBio(Command request)
        {
            var oldBio = await _unitOfWork.DbContext.ParticipantBios.Where(b => b.ParticipantId == request.ParticipantId)
                           .OrderByDescending(b => b.Created)
                           .FirstOrDefaultAsync();

            if (oldBio == null)
            {
                return null;
            }

            var bio = JsonConvert.DeserializeObject<Bio>(oldBio.BioJson,
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            })!;

            return bio;
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ParticipantId)
                    .MustAsync(Exist)
                    .WithMessage("Participant not found")
                    .MustAsync(MustNotBeArchived)
                    .WithMessage("Participant is archived");
            });
        }

        private async Task<bool> MustNotBeArchived(string participantId, CancellationToken cancellationToken)
                => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId && e.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value, cancellationToken);

        private async Task<bool> Exist(string participantId, CancellationToken cancellationToken)
                => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId, cancellationToken);
    }
}