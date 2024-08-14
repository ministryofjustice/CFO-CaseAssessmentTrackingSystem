using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class CreateParticipant
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command: IRequest<Result<string>>
    {
        /// <summary>
        /// The CATS identifier
        /// </summary>
        public string? Identifier => Candidate.Identifier;

        public required CandidateDto Candidate { get; set; }
    
        public string? ReferralSource { get; set; }
    
        public string? ReferralComments { get; set; }
    
        public UserProfile? CurrentUser { get; set; }

        public string[] CacheKeys => [ ParticipantCacheKey.GetCacheKey($"{this}") ];

        public CancellationTokenSource? SharedExpiryTokenSource 
            => ParticipantCacheKey.SharedExpiryTokenSource();

        public override string ToString()
        {
            return $"Id:{Identifier}";
        }
    }

    public class Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) 
        : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var candidate = request.Candidate;

            var location = await unitOfWork.DbContext.LocationMappings
                .Include(l => l.Location)
                .SingleAsync(l => l.Code == candidate.EstCode, cancellationToken);

            Participant participant = Participant.CreateFrom(candidate.Identifier, candidate.FirstName, candidate.LastName, candidate.DateOfBirth, 
            request.ReferralSource!, request.ReferralComments, location.Location?.Id ?? 0);
            participant.AssignTo(currentUserService.UserId);
        
            await unitOfWork.DbContext.Participants.AddAsync(participant, cancellationToken);
            return request.Identifier!;
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Identifier).NotNull()
                .NotEmpty()
                .MinimumLength(9)
                .MaximumLength(9)
                .WithMessage("Invalid Cats Identifier")
                .Matches(ValidationConstants.AlphaNumeric).WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Identifier"))
                .MustAsync(NotAlreadyExist)
                .WithMessage("Participant is already enrolled");
            
            RuleFor(x => x.Candidate.EstCode)
                .NotNull()
                .MaximumLength(3)
                .WithMessage("Invalid establishment code")
                .MinimumLength(3)
                .WithMessage("Invalid establishment code")
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage("Invalid establishment code")
                .MustAsync(MappedLocation)
                .WithMessage("Unknown establishment location");
                
            RuleFor(x => x.ReferralSource)
                .NotNull()
                .NotEmpty()
                .Matches(ValidationConstants.Notes).WithMessage(string.Format(ValidationConstants.NotesMessage, "Referral source"));

            When(x => x.ReferralSource is "Other" or "Healthcare", () => {
                RuleFor(x => x.ReferralComments)
                    .NotEmpty()
                    .WithMessage("Comments are mandatory with this referral source")
                    .Matches(ValidationConstants.Notes).WithMessage(string.Format(ValidationConstants.NotesMessage, "Referral source comments"));
            });

 
        }
        private async Task<bool> MappedLocation(string estCode, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.LocationMappings.AnyAsync(l => l.Code == estCode, cancellationToken);
       
        private async Task<bool> NotAlreadyExist(string identifier, CancellationToken cancellationToken) 
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken) == false;

    }
}
