using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantLocationHistory
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<IEnumerable<ParticipantLocationHistoryDto>>
    {
        public required string ParticipantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, IEnumerable<ParticipantLocationHistoryDto>>
    {
        public async Task<IEnumerable<ParticipantLocationHistoryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

            var query = 
                from plh in db.ParticipantLocationHistories
                join l in db.Locations on plh.LocationId equals l.Id
                join u in db.Users on plh.CreatedBy equals u.Id into userJoin
                from u in userJoin.DefaultIfEmpty()
                
                where plh.ParticipantId == request.ParticipantId
                select new ParticipantLocationHistoryDto
                {
                    Id = plh.Id,
                    MoveDate = plh.From,
                    LocationName = l.Name
                };

            var locationHistories = await query
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            
            if (locationHistories.Any() is false)
                return locationHistories;

            var last = locationHistories.Last();
                        
            for (int i = 0; i < locationHistories.Count; i++)
            {            
                var current = locationHistories[i];

                var next = current == last
                    ? DateTime.UtcNow
                    : locationHistories[i + 1].MoveDate;

                var diff = (next - current.MoveDate) ?? TimeSpan.Zero;
                current.DaysSincePrevious = diff.Days;
            }

            return locationHistories;
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

        }

        private async Task<bool> Exist(string identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);
    }
}