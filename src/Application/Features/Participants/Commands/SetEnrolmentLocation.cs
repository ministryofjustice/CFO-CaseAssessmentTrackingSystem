using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class SetEnrolmentLocation
{
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Command(string identifier, LocationDto currentLocation, LocationDto enrolmentLocation, string? justificationReason)
        : ICacheInvalidatorRequest<Result<string>>
    {

        /// <summary>
        /// The identifier of the participant whose enrolment details we are changing
        /// </summary>
        public string Identifier { get; set; } = identifier;

        /// <summary>
        /// The location to assign the enrolment to
        /// </summary>
        public LocationDto CurrentLocation { get; set; } = currentLocation;

        /// <summary>
        /// The location to assign the enrolment to
        /// </summary>
        public LocationDto EnrolmentLocation { get; set; } = enrolmentLocation;

        /// <summary>
        /// A justification for enrolling a participant in a location
        /// other than where we think they are
        /// </summary>
        public string? JustificationReason { get; set; } = justificationReason;

        public bool EnrolFromOtherLocation { get; set; } = enrolmentLocation.Id != currentLocation.Id;
        
        public string CacheKey => ParticipantCacheKey.GetCacheKey($"Id:{this.Identifier}");
        public CancellationTokenSource? SharedExpiryTokenSource 
            => ParticipantCacheKey.SharedExpiryTokenSource();
    }

    public class Handler(IApplicationDbContext context, ICurrentUserService currentUserService) 
        : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            Participant? participant = await context.Participants.FirstOrDefaultAsync(p => p.Id == request.Identifier, cancellationToken);
            if (participant == null)
            {
                throw new NotFoundException("Cannot find participant", request.Identifier);
            }

            if (participant.EnrolmentStatus == EnrolmentStatus.ApprovedStatus)
            {
                throw new ConflictException($"Participant {request.Identifier} is already enrolled");
            }

            participant.SetEnrolmentLocation(request.EnrolmentLocation.Id, request.JustificationReason);
            await context.SaveChangesAsync(cancellationToken);
            return participant.Id;
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CurrentLocation)
                .NotNull();

            RuleFor(x => x.EnrolmentLocation)
                .NotNull();
            
            When(x => x.CurrentLocation != x.EnrolmentLocation, () => {
                RuleFor(x => x.JustificationReason)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Justification reason is mandatory when enrolling in a different location");
            });
        }
    }
}
