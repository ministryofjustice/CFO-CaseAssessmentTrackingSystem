using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetPreviousJustification
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<string>>
    {
        public required string ParticipantId { get; init; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<string>>
    {
        public async Task<Result<string>> Handle(Query request, CancellationToken cancellationToken)
        {
            var justification = await unitOfWork.DbContext.Participants
                .Where(p => p.Id == request.ParticipantId 
                            && p.EnrolmentStatus == EnrolmentStatus.EnrollingStatus.Value
                            && p.ConsentStatus==ConsentStatus.GrantedStatus.Value)
                .Select(p => p.AssessmentJustification)
                .SingleOrDefaultAsync(cancellationToken);
           
           if (string.IsNullOrWhiteSpace(justification))
           { 
               return  Result<string>.Failure( "Assessment justification was not found for this participant.");
           }

           return  Result<string>.Success(justification);
        }
    }  

    public class Validator : AbstractValidator<Query>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .Length(9)
                .WithMessage("Invalid Participant Id")
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));        

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ParticipantId)
                    .MustAsync(Exist)
                    .WithMessage("Participant does not exist");
            });
        }

        private async Task<bool> Exist(string identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);
    }
}
