using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantDetails
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : ParticipantDetailsQuery<ParticipantDetails>
    {

    }
    
    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<ParticipantDetails>>
    {

        public async Task<Result<ParticipantDetails>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = from p in unitOfWork.DbContext.Participants.AsNoTracking()
                where p.Id == request.ParticipantId
                select new ParticipantDetails
                {
                    DateOfBirth = p.DateOfBirth,
                    EnrolmentLocation = p.EnrolmentLocation!.Name,
                    Nationality = p.Nationality,
                    DateOfFirstConsent = p.DateOfFirstConsent,
                    LastSync = p.LastSyncDate!.Value
                };

            var result = await query.FirstOrDefaultAsync(cancellationToken);

            if (result == null)
            {
                return Result<ParticipantDetails>.NotFound();
            }

            return Result<ParticipantDetails>.Success(result);
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
    
    public record ParticipantDetails
    {
        public required DateOnly DateOfBirth { get; init; }
        public required string EnrolmentLocation { get; init; }
        public required string? Nationality { get; init; }
        public required DateOnly? DateOfFirstConsent { get; init; }
        public required DateTime LastSync { get; init; }
    }
    
}