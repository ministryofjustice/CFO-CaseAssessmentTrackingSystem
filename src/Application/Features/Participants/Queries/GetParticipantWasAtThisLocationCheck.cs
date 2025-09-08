using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

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
            var dateAtLocation = request.DateAtLocation!.Value.Date;

            var location = await unitOfWork.DbContext.Locations
                .AsNoTracking()
                .Where(l => l.Id == request.LocationId)
                .Select(l => l.LocationType)
                .FirstOrDefaultAsync(cancellationToken);

            return location switch
            {
                null => Result<bool>.Failure("Invalid location"),
                var t when t.IsHub => await CheckHub(request),
                _ => await CheckNonHub(request)
            };
        }

        /// <summary>
        /// Checks that the participant has a hub induction on the given date.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<Result<bool>> CheckHub(Query request)
        {
            var query = from hi in unitOfWork.DbContext.HubInductions.AsNoTracking()
                        where hi.ParticipantId == request.ParticipantId
                            && hi.LocationId == request.LocationId
                            && hi.InductionDate <= request.DateAtLocation!.Value.Date
                        select hi.InductionDate;

            var result = await query.ToListAsync();

            return result is null ? Result<bool>.Failure("No hub induction recorded for this location on this date") : true;
        }

        private async Task<Result<bool>> CheckNonHub(Query request)
        {
            var dateAtLocation = request.DateAtLocation!.Value.Date;

            var locationOnDate = await unitOfWork.DbContext.ParticipantLocationHistories.AsNoTracking()
                .Where(plh => plh.ParticipantId == request.ParticipantId &&
                              plh.LocationId == request.LocationId &&
                              plh.From.Date <= dateAtLocation &&
                              (!plh.To.HasValue || plh.To.Value.Date >= dateAtLocation))
                .OrderByDescending(x => x.From)
                .FirstOrDefaultAsync();

            return locationOnDate is null ? Result<bool>.Failure("Participant was not at location on date") : true;
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

            RuleFor(x => x.DateAtLocation)
                .NotNull();

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