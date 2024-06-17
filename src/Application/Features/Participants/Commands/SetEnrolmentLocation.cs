using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class SetEnrolmentLocation
{
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Command : ICacheInvalidatorRequest<Result<string>>
    {
        public Command(string identifier, int currentLocationId, int enrolmentLocationId, string? justificationReason)
        {
            this.Identifier = identifier;
            this.CurrentLocationId = currentLocationId;
            this.EnrolmentLocationId = enrolmentLocationId;
            this.JustificationReason = justificationReason;
        }

        /// <summary>
        /// The identifier of the participant whose enrolment details we are changing
        /// </summary>
        public string Identifier { get; set; }
        
        /// <summary>
        /// The location to assign the enrolment to
        /// </summary>
        public int CurrentLocationId { get; set; }
        
        /// <summary>
        /// The location to assign the enrolment to
        /// </summary>
        public int EnrolmentLocationId { get; set; }
        
        /// <summary>
        /// A justification for enrolling a participant in a location
        /// other than where we think they are
        /// </summary>
        public string? JustificationReason { get; set; }
        
        
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

            participant.SetEnrolmentLocation(request.EnrolmentLocationId, request.JustificationReason);
            await context.SaveChangesAsync(cancellationToken);
            return participant.Id;
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CurrentLocationId)
                .GreaterThan(0);

            RuleFor(x => x.EnrolmentLocationId)
                .GreaterThan(0);
            
            When(x => x.CurrentLocationId != x.EnrolmentLocationId, () => {
                RuleFor(x => x.JustificationReason)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Justification reason is mandatory when enrolling in a different location");
            });
        }
    }
}
