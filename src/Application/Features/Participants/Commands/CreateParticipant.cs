using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Candidates;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class CreateParticipant
{
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Command: ICacheInvalidatorRequest<Result<string>>
    {
        /// <summary>
        /// The CATS identifier
        /// </summary>
        public string? Identifier { get; set; }
    
        public string? ReferralSource { get; set; }
    
        public string? ReferralComments { get; set; }
    
        public UserProfile? CurrentUser { get; set; }
    
        public string CacheKey => ParticipantCacheKey.GetCacheKey($"{this}");

        public CancellationTokenSource? SharedExpiryTokenSource 
            => ParticipantCacheKey.SharedExpiryTokenSource();

        public override string ToString()
        {
            return $"Id:{Identifier}";
        }
    }

    public class Handler(IApplicationDbContext dbContext, ICurrentUserService currentUserService) 
        : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            Candidate candidate = await dbContext.Candidates.FirstAsync(c => c.Id == request.Identifier, cancellationToken);
            Participant participant = Participant.CreateFrom(candidate, request.ReferralSource!, request.ReferralComments);
            participant.AssignTo(currentUserService.UserId);
        
            dbContext.Participants.Add(participant);
            await dbContext.SaveChangesAsync(cancellationToken);

            return request.Identifier!;
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IApplicationDbContext _dbContext;
        
        public Validator(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.Identifier).NotNull()
                .NotEmpty()
                .MinimumLength(9)
                .MaximumLength(9)
                .WithMessage("Invalid Cats Identifier")
                .MustAsync(NotAlreadyExist)
                .WithMessage("Participant is already enrolled");

            RuleFor(x => x.ReferralSource)
                .NotNull()
                .NotEmpty();

            When(x => x.ReferralSource is "Other" or "Healthcare", () => {
                RuleFor(x => x.ReferralComments)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Comments are mandatory with this referral source");
            });

 
        }
        private async Task<bool> NotAlreadyExist(string identifier, CancellationToken cancellationToken) => await _dbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken) == false;

    }
}
