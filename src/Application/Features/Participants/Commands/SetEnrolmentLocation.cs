using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class SetEnrolmentLocation
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command(string identifier, LocationDto currentLocation, LocationDto? enrolmentLocation, string? justificationReason)
        : ICacheInvalidatorRequest<Result<string>>
    {

        /// <summary>
        /// The identifier of the participant whose enrolment details we are changing
        /// </summary>
        public string Identifier { get; set; } = identifier;

        /// <summary>
        /// The location to assign the enrolment to
        /// </summary>
        [Description("Current Location")]
        public LocationDto CurrentLocation { get; set; } = currentLocation;

        /// <summary>
        /// The location to assign the enrolment to
        /// </summary>
        [Description("Alternative Location")]
        public LocationDto? AlternativeLocation { get; set; } = enrolmentLocation;

        /// <summary>
        /// A justification for enrolling a participant in a location
        /// other than where we think they are
        /// </summary>
        [Description("Justification reason for alternative enrolment location")]
        public string? JustificationReason { get; set; } = justificationReason;

        [Description("Enrol at an alternative location enrolment")]
        public bool EnrolFromAlternativeLocation { get; set; } 
        
        public string[] CacheKeys => [ParticipantCacheKey.GetCacheKey($"Id:{this.Identifier}")];
        public CancellationTokenSource? SharedExpiryTokenSource 
            => ParticipantCacheKey.SharedExpiryTokenSource();
    }

    public class Handler(IUnitOfWork unitOfWork) 
        : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants.FirstOrDefaultAsync(p => p.Id == request.Identifier, cancellationToken);
            if (participant == null)
            {
                throw new NotFoundException("Cannot find participant", request.Identifier);
            }

            if (participant.EnrolmentStatus == EnrolmentStatus.ApprovedStatus)
            {
                throw new ConflictException($"Participant {request.Identifier} is already enrolled");
            }

            if (participant.EnrolmentStatus == EnrolmentStatus.ArchivedStatus)
            {
                throw new ConflictException($"Participant {request.Identifier} is archived");
            }

            participant.SetEnrolmentLocation(request.AlternativeLocation?.Id ?? request.CurrentLocation.Id, request.JustificationReason);
            return participant.Id;
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CurrentLocation)
                .NotNull()
                .WithMessage("Current location is unknown or missing");

            When(x => x.EnrolFromAlternativeLocation, () => {
                RuleFor(x => x.AlternativeLocation)
                    .NotNull()
                    .WithMessage("You must provide an alternative location");

                RuleFor(x => x.AlternativeLocation)
                    .Must((x, alternativeLocation) => x.CurrentLocation.Id != alternativeLocation!.Id)
                    .When(x => x.AlternativeLocation is not null)
                    .WithMessage("You must provide a different alternative location to the current location");

                RuleFor(c => c.JustificationReason)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Justification reason is mandatory when enrolling in a different location")
                    .MaximumLength(ValidationConstants.NotesLength)
                    .Matches(ValidationConstants.Notes)
                    .WithMessage(string.Format(ValidationConstants.NotesMessage, "Justification Reason"));
            });
        }
    }
}
