using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantWasAtThisLocationCheck
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<bool>>
    {
        public required string ParticipantId { get; set; }
        public required int LocationId { get; set; }

        public required DateTime? DateAtLocation { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Query request, CancellationToken cancellationToken)
        {
            var dateAtLocation = request.DateAtLocation?.Date;

            if (dateAtLocation == null)
            {
                return Result<bool>.Failure("Location date required");
            }

            var db = unitOfWork.DbContext;

            var isHubAndHasInduction = await db.HubInductions
                                                .Where(hi => hi.ParticipantId == request.ParticipantId
                                                    && hi.LocationId == request.LocationId
                                                    && hi.InductionDate >= request.DateAtLocation)
                                                .AnyAsync(cancellationToken);

            if (isHubAndHasInduction is false)
            {
                var locationOnDate = await db.ParticipantLocationHistories
                                               .Where(plh =>
                                                   plh.ParticipantId == request.ParticipantId &&
                                                   plh.LocationId == request.LocationId &&
                                                   plh.From.Date <= dateAtLocation &&
                                                   (!plh.To.HasValue || plh.To.Value.Date >= dateAtLocation)
                                               )
                                               .OrderByDescending(x => x.From)
                                               .FirstOrDefaultAsync(cancellationToken);

                if (locationOnDate is null)
                {
                    return Result<bool>.Failure("Participant was not at location on date");
                }
            }

            return Result<bool>.Success(true);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.ParticipantId)
                .NotNull();

            RuleFor(x => x.ParticipantId)
                .Length(9)
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