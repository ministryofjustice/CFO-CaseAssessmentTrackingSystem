using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Administration;
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
        public string Identifier => Candidate.Identifier;

        public required CandidateDto Candidate { get; set; }

        [Description("Referral Source")]
        public string? ReferralSource { get; set; }

        [Description("Referral Comments")]
        public string? ReferralComments { get; set; }
    
        public UserProfile? CurrentUser { get; set; }

        [Description("I confirm the details above reflect the CFO Consent form.")]
        public bool Confirmation { get; set; }

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

            Participant participant = Participant.CreateFrom(
                id: candidate.Identifier,
                firstName: candidate.FirstName,
                middleName: candidate.SecondName,
                lastName: candidate.LastName,
                gender: candidate.Gender,
                dateOfBirth: candidate.DateOfBirth,
                referralSource: request.ReferralSource!,
                referralComments: request.ReferralComments,
                locationId: candidate.MappedLocationId);

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

            RuleFor(x => x.Identifier)
                .NotEmpty()
                .Length(9)
                .WithMessage("Invalid Cats Identifier")
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Identifier"))
                .MustAsync(NotAlreadyExist)
                .WithMessage("Participant is already enrolled");

            // Establishment Code is required for Prison (NOMIS) records.
            When(x => x.Candidate.Primary is "NOMIS", () =>
            {
                RuleFor(x => x.Candidate.EstCode)
                    .NotEmpty()
                    .Length(3)
                    .WithMessage("Invalid establishment code")
                    .Matches(ValidationConstants.AlphaNumeric)
                    .WithMessage("Invalid establishment code");
                    //.MustAsync((string estCode, CancellationToken cancellationToken) => MappedLocation(estCode, "Prison", cancellationToken))
                    //.WithMessage("Unknown establishment location");
            });

            // Organisation Code is otherwise required for Probation (DELIUS) records.
            When(x => x.Candidate.Primary is "DELIUS", () =>
            {
                RuleFor(x => x.Candidate.OrgCode)
                    .NotEmpty()
                    .Length(3)
                    .WithMessage("Invalid organisation code")
                    .Matches(ValidationConstants.AlphaNumeric)
                    .WithMessage("Invalid organisation code");
                    //.MustAsync((string estCode, CancellationToken cancellationToken) => MappedLocation(estCode, "Probation", cancellationToken))
                    //.WithMessage("Unknown organisation location");
            });

            RuleFor(x => x.ReferralSource)
                .NotEmpty()
                .WithMessage("Referral source is required")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Referral source"));

            When(x => x.ReferralSource is "Other" or "Healthcare", () => {
                RuleFor(x => x.ReferralComments)
                    .NotEmpty()
                    .WithMessage("Comments are mandatory with this referral source")
                    .Matches(ValidationConstants.Notes).WithMessage(string.Format(ValidationConstants.NotesMessage, "Referral source comments"));
            });
 
        }

        //private async Task<bool> MappedLocation(string code, string type,  CancellationToken cancellationToken)
        //    => await _unitOfWork.DbContext.LocationMappings.AnyAsync(l => l.Code == code && l.CodeType == type, cancellationToken);
       
        private async Task<bool> NotAlreadyExist(string identifier, CancellationToken cancellationToken) 
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken) == false;
    }
}
