using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantDetails
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : ParticipantDetailsQuery<ParticipantHeaderDetailsDto>
    {

    }
    
    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<ParticipantHeaderDetailsDto>>
    {

        public async Task<Result<ParticipantHeaderDetailsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            IReadOnlyList<ConsentStatus> granted = [ConsentStatus.GrantedStatus];
            var query = from p in unitOfWork.DbContext.Participants.AsNoTracking()
                where p.Id == request.ParticipantId
                select new ParticipantHeaderDetailsDto
                {
                    FullName = p.FirstName + " " + p.LastName,
                    EnrolmentStatus = p.EnrolmentStatus!,
                    DateOfBirth = granted.Contains(p.ConsentStatus!) ? p.DateOfBirth : null,
                    EnrolmentLocation = p.EnrolmentLocation!.Name,
                    EnrolmentJustification = p.EnrolmentLocationJustification,
                    AssessmentJustification = p.AssessmentJustification,
                    Nationality = granted.Contains(p.ConsentStatus!) ? p.Nationality : null,
                    IsConsentGranted = granted.Contains(p.ConsentStatus!),
                    DateOfFirstConsent = p.DateOfFirstConsent,
                    LastSync = p.LastSyncDate ?? p.Created!.Value
                };

            var result = await query.FirstOrDefaultAsync(cancellationToken);

            if (result == null)
            {
                return Result<ParticipantHeaderDetailsDto>.NotFound();
            }

            return Result<ParticipantHeaderDetailsDto>.Success(result);
        }
    }
    
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId)
                .NotNull();

            RuleFor(x => x.ParticipantId)
                .Length(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id is not valid"));

            RuleFor(x => x.CurrentUser)
                .NotNull()
                .WithMessage("Current user details required");
        }
    }
}